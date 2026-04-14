# KafkaDemo — Apache Kafka ile .NET 10

Apache Kafka'nın temel kavramlarını (.NET 10) ile uygulamalı olarak gösteren örnek proje. Producer API, Consumer BackgroundService ve paylaşılan Contracts katmanından oluşur.

---

## Proje Yapısı

```
KafkaDemo/
├── KafkaDemo.sln
├── docker-compose.yml
├── KafkaDemo.Contracts/          # Paylaşılan modeller
│   └── Models/
│       └── OrderCreatedEvent.cs
├── KafkaDemo.Producer/           # Sipariş oluşturan Web API
│   ├── Program.cs
│   └── KafkaDemo.Producer.csproj
└── KafkaDemo.Consumer/           # Sipariş işleyen Worker Service
    ├── Program.cs
    ├── Services/
    │   └── OrderConsumerService.cs
    └── KafkaDemo.Consumer.csproj
```

---

## Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

---

## Kurulum ve Çalıştırma

### 1. Kafka'yı başlat

```bash
# docker-compose.yml
version: '3.8'
services:
  kafka:
    image: confluentinc/cp-kafka:7.6.0
    ports:
      - "9092:9092"
    environment:
      KAFKA_NODE_ID: 1
      KAFKA_PROCESS_ROLES: broker,controller
      KAFKA_LISTENERS: PLAINTEXT://0.0.0.0:9092,CONTROLLER://0.0.0.0:9093
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://localhost:9092
      KAFKA_CONTROLLER_QUORUM_VOTERS: 1@kafka:9093
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT,CONTROLLER:PLAINTEXT
      KAFKA_CONTROLLER_LISTENER_NAMES: CONTROLLER
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
      CLUSTER_ID: "MkU3OEVBNTcwNTJENDM2Qk"
```

```bash
# Proje yapısı
dotnet new sln -n KafkaDemo
dotnet new webapi -n KafkaDemo.Producer
dotnet new worker  -n KafkaDemo.Consumer
dotnet sln add KafkaDemo.Producer KafkaDemo.Consumer
```

```bash
# Her iki projeye da Confluent.Kafka paketini ekle
dotnet add KafkaDemo.Producer package Confluent.Kafka
dotnet add KafkaDemo.Consumer package Confluent.Kafka
```
```bash
# Contracts projesini oluştur
dotnet new classlib -n KafkaDemo.Contracts
dotnet sln add KafkaDemo.Contracts

# Her iki projeye referans ekle
dotnet add KafkaDemo.Producer reference KafkaDemo.Contracts
dotnet add KafkaDemo.Consumer reference KafkaDemo.Contracts
```


```bash
# Terminal 1 - Kafka'yı başlat
docker-compose up -d

# Terminal 2 - Consumer'ı başlat
cd KafkaDemo.Consumer && dotnet run

# Terminal 3 - Producer API'yi başlat
cd KafkaDemo.Producer && dotnet run

# Terminal 4 - Test et
**PowerShell:**
```powershell
Invoke-WebRequest -Uri "http://localhost:5278/orders" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{"customerId": "ahmet-123", "amount": 299.99}'
```

Kafka'nın ayağa kalktığını doğrulamak için:

```bash
docker ps
```

Consumer `orders-topic`'i dinlemeye başlar ve terminalde şunu görürsün:

```
Sipariş Consumer Başladı...
```

Başarılı yanıt:
```json
{
  "orderId": "199675f0-de43-41e2-9016-ab1a01f386d7",
  "partition": 0,
  "offset": 0
}
```

Consumer terminalinde:
```
Sipariş İşleniyor: 199675f0-... | Müşteri: ahmet-123 | Tutar: 299.99
Sipariş 199675f0-... işlendi
```

---

## Kaynaklar

- [Apache Kafka Dokümantasyonu](https://kafka.apache.org/documentation/)
- [Confluent .NET Client](https://github.com/confluentinc/confluent-kafka-dotnet)
- [Kafka Nedir? .NET Core İle Kafka Kullanımı — Medium Makale](#)
