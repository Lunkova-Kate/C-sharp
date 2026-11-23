using System.Text.Json;
using MasterOfSkipping.ProfessorService.Contracts.Messaging;
using RabbitMQ.Client;

namespace MasterOfSkipping.ProfessorService.Services;

public sealed class LessonResultPublisher : IHostedService, IAsyncDisposable
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly string _queueName;
    private readonly SemaphoreSlim _publishLock = new(1, 1);
    private IConnection? _connection;
    private IChannel? _channel;

    public LessonResultPublisher(IConfiguration configuration)
    {
        string hostName = configuration["RabbitMq:HostName"]
            ?? throw new InvalidOperationException(
                "RabbitMq:HostName was not configured.");

        string userName = configuration["RabbitMq:UserName"]
            ?? throw new InvalidOperationException(
                "RabbitMq:UserName was not configured.");

        string password = configuration["RabbitMq:Password"]
            ?? throw new InvalidOperationException(
                "RabbitMq:Password was not configured.");

        int port = configuration.GetValue<int?>("RabbitMq:Port")
            ?? throw new InvalidOperationException(
                "RabbitMq:Port was not configured.");

        _queueName = configuration["RabbitMq:QueueName"]
            ?? throw new InvalidOperationException(
                "RabbitMq:QueueName was not configured.");

        _connectionFactory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password,
            AutomaticRecoveryEnabled = true
        };
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _connection = await _connectionFactory.CreateConnectionAsync(
            cancellationToken);

        var channelOptions = new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true);

        _channel = await _connection.CreateChannelAsync(
            channelOptions,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
    }

    public async Task PublishAsync(
        LessonResultMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        IChannel channel = _channel
            ?? throw new InvalidOperationException(
                "RabbitMQ publisher has not been started.");

        byte[] body = JsonSerializer.SerializeToUtf8Bytes(message);

        var properties = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            MessageId = message.MessageId.ToString()
        };

        await _publishLock.WaitAsync(cancellationToken);

        try
        {
            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _queueName,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);
        }
        finally
        {
            _publishLock.Release();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        _publishLock.Dispose();
    }
}
