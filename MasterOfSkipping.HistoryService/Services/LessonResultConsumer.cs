using System.Text.Json;
using MasterOfSkipping.HistoryService.Contracts.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MasterOfSkipping.HistoryService.Services;

public sealed class LessonResultConsumer : BackgroundService
{
    private readonly ConnectionFactory _connectionFactory;
    private readonly string _queueName;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LessonResultConsumer> _logger;

    public LessonResultConsumer(
        IConfiguration configuration,
        IServiceScopeFactory scopeFactory,
        ILogger<LessonResultConsumer> logger)
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

        _scopeFactory = scopeFactory;
        _logger = logger;

        _connectionFactory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password,
            AutomaticRecoveryEnabled = true
        };
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        await using IConnection connection =
            await _connectionFactory.CreateConnectionAsync(stoppingToken);

        await using IChannel channel =
            await connection.CreateChannelAsync(
                cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            await HandleMessageAsync(
                channel,
                eventArgs,
                stoppingToken);
        };

        await channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
        }
    }

    private async Task HandleMessageAsync(
        IChannel channel,
        BasicDeliverEventArgs eventArgs,
        CancellationToken cancellationToken)
    {
        try
        {
            LessonResultMessage message =
                JsonSerializer.Deserialize<LessonResultMessage>(
                    eventArgs.Body.Span)
                ?? throw new JsonException(
                    "The lesson result message was empty.");

            await using AsyncServiceScope scope =
                _scopeFactory.CreateAsyncScope();

            LessonResultWriter writer = scope.ServiceProvider
                .GetRequiredService<LessonResultWriter>();

            await writer.SaveAsync(message, cancellationToken);

            await channel.BasicAckAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken);

            _logger.LogInformation(
                "Lesson result {MessageId} was saved and acknowledged.",
                message.MessageId);
        }
        catch (JsonException exception)
        {
            _logger.LogError(
                exception,
                "Invalid JSON message was rejected.");

            await channel.BasicNackAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            _logger.LogError(
                exception,
                "Invalid lesson result message was rejected.");

            await channel.BasicNackAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                requeue: false,
                cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Lesson result processing failed. The message will be requeued.");

            await channel.BasicNackAsync(
                eventArgs.DeliveryTag,
                multiple: false,
                requeue: true,
                cancellationToken);
        }
    }
}
