FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY KafkaDemo.Contracts/KafkaDemo.Contracts.csproj KafkaDemo.Contracts/
COPY KafkaDemo.Consumer/KafkaDemo.Consumer.csproj KafkaDemo.Consumer/
RUN dotnet restore KafkaDemo.Consumer/KafkaDemo.Consumer.csproj

COPY KafkaDemo.Contracts/ KafkaDemo.Contracts/
COPY KafkaDemo.Consumer/ KafkaDemo.Consumer/
RUN dotnet publish KafkaDemo.Consumer/KafkaDemo.Consumer.csproj \
    -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "KafkaDemo.Consumer.dll"]