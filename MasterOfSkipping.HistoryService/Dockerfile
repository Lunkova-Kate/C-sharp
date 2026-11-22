FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY MasterOfSkipping.HistoryService/MasterOfSkipping.HistoryService.csproj \
     MasterOfSkipping.HistoryService/

RUN dotnet restore \
    MasterOfSkipping.HistoryService/MasterOfSkipping.HistoryService.csproj

COPY . .

WORKDIR /src/MasterOfSkipping.HistoryService

RUN dotnet publish \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT [ "dotnet", "MasterOfSkipping.HistoryService.dll"]