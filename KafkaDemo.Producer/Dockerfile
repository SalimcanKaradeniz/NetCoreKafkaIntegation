FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY KafkaDemo.Constracts/KafkaDemo.Constracts.csproj KafkaDemo.Constracts/
COPY KafkaDemo.Producer/KafkaDemo.Producer.csproj KafkaDemo.Producer/
RUN dotnet restore KafkaDemo.Producer/KafkaDemo.Producer.csproj

COPY KafkaDemo.Constracts/ KafkaDemo.Constracts/
COPY KafkaDemo.Producer/ KafkaDemo.Producer/
RUN dotnet publish KafkaDemo.Producer/KafkaDemo.Producer.csproj \ -c Relase -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet","KafkaDemo.Producer.dll"]