
# 🧠 SalesAPI

SalesAPI é uma aplicação modular construída em ASP.NET Core 8, que simula um ecossistema de e-commerce com os módulos:

- **Users**
- **Products**
- **Carts**
- **Sales**
- **Shared**

Projetada com foco em escalabilidade, separação de responsabilidades e aplicação de princípios de arquitetura moderna como DDD, CQRS, Event Sourcing (parcial) e comunicação via eventos de domínio e integração.

## 🧱 Arquitetura

O sistema adota uma **arquitetura modular monolítica**, onde cada módulo possui:

- Application Layer
- Domain Layer
- Infrastructure Layer
- Presentation Layer (Opcional)

Comunicação entre módulos:
- **Eventos de Domínio** (`INotification`)
- **Eventos de Integração** com **MassTransit + RabbitMQ**

## ⚙️ Tecnologias Utilizadas

- ASP.NET Core 8
- Entity Framework Core (InMemory / PostgreSQL)
- FluentValidation
- MediatR
- AutoMapper
- MassTransit + RabbitMQ
- Gridify
- xUnit + Bogus (Faker) + NSubstitute
- Swagger / OpenAPI

## 📁 Estrutura de Pastas

```bash
├── Host/
│   ├── Api/
├── Modules/
│   ├── Users/
│   ├── Products/
│   ├── Carts/
│   └── Sales/
├── Shared/
│   ├── Shared.Application/
│   ├── Shared.Domain/
│   ├── Shared.Infrastructure/
│   ├── Shared.IntegrationEvents/
│   └── Shared.Presentation/
```

## 🚀 Execução Local

### Requisitos

- .NET 8 SDK
- Docker (PostgreSQL e RabbitMQ)

### Subir os serviços auxiliares

```bash
docker-compose up -d
```

### Rodar o projeto

```bash
dotnet run --project Api
```

Acesse o Swagger:
```
https://localhost:{porta}/swagger
```


## 🐳 Docker

O projeto possui um `docker-compose.yml` na raiz para facilitar a execução dos serviços externos:

- PostgreSQL (banco de dados)
- RabbitMQ (mensageria)

```bash
docker-compose up -d
```

## 👤 Seed de Usuário

Ao iniciar o sistema, um usuário de teste será criado automaticamente:

- **Email:** `test@email.com`
- **Senha:** `admin123@`

## 🧪 Testes

```bash
dotnet test
```

## 🧾 Regras de Negócio

- Um usuário só pode ter **um carrinho ativo**
- Máximo de **20 unidades por produto**
- Carrinho é **finalizado** ao criar a venda
- Ao **deletar um produto**, remove de:
    - Carrinhos ativos
    - Vendas ativas

## 📡 Eventos

- Eventos de **domínio**: `INotificationHandler`
- Eventos de **integração**: `MassTransit` + `RabbitMQ`

```csharp
await _publishEndpoint.Publish(new ProductDeletedIntegrationEvent(productId));
await _publishEndpoint.Publish(new FinalizedCartIntegrationEvent(cartId));
```

## 👤 Autor

Feito por **Lacir Junior**

---


# 🧠 SalesAPI

SalesAPI is a modular ASP.NET Core 8 application simulating an e-commerce ecosystem with the following modules:

- **Users**
- **Products**
- **Carts**
- **Sales**
- **Shared**

Built with focus on scalability, separation of concerns, and modern architectural practices like DDD, CQRS, partial Event Sourcing, and messaging via domain and integration events.

## 🧱 Architecture

Follows a **modular monolith** design, where each module contains:

- Application Layer
- Domain Layer
- Infrastructure Layer
- Presentation Layer (optional)

Inter-module communication:
- **Domain Events** (`INotification`)
- **Integration Events** via **MassTransit + RabbitMQ**

## ⚙️ Tech Stack

- ASP.NET Core 8
- EF Core (InMemory / PostgreSQL)
- FluentValidation
- MediatR
- AutoMapper
- MassTransit + RabbitMQ
- Gridify
- xUnit + Bogus (Faker) + NSubstitute
- Swagger / OpenAPI

## 📁 Folder Structure

```bash
├── Host/
│   ├── Api/
├── Modules/
│   ├── Users/
│   ├── Products/
│   ├── Carts/
│   └── Sales/
├── Shared/
│   ├── Shared.Application/
│   ├── Shared.Domain/
│   ├── Shared.Infrastructure/
│   ├── Shared.IntegrationEvents/
│   └── Shared.Presentation/
```

## 🚀 Running Locally

### Requirements

- .NET 8 SDK
- Docker (PostgreSQL & RabbitMQ)

### Start Dependencies

```bash
docker-compose up -d
```

### Run the API

```bash
dotnet run --project Api
```

Access Swagger:
```
https://localhost:{port}/swagger
```

## 🧪 Testing

```bash
dotnet test
```

## 🧾 Business Rules

- A user can only have **one active cart**
- Max **20 units per product**
- Cart is **finalized** when creating a sale
- Deleting a product removes it from:
    - Active carts
    - Active sales

## 📡 Events

- **Domain Events** via `INotificationHandler`
- **Integration Events** via `MassTransit` + `RabbitMQ`

```csharp
await _publishEndpoint.Publish(new ProductDeletedIntegrationEvent(productId));
await _publishEndpoint.Publish(new FinalizedCartIntegrationEvent(cartId));
```

## 👤 Author

Made by **Lacir Junior**
