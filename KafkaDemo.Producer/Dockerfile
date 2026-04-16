FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY KafkaDemo.Contracts/KafkaDemo.Contracts.csproj KafkaDemo.Contracts/
COPY KafkaDemo.Producer/KafkaDemo.Producer.csproj KafkaDemo.Producer/
RUN dotnet restore KafkaDemo.Producer/KafkaDemo.Producer.csproj

COPY KafkaDemo.Contracts/ KafkaDemo.Contracts/
COPY KafkaDemo.Producer/ KafkaDemo.Producer/
RUN dotnet publish KafkaDemo.Producer/KafkaDemo.Producer.csproj \
    -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "KafkaDemo.Producer.dll"]