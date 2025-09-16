# Motorcycle Rental API

A RESTful API for managing motorcycle rentals and couriers, built with .NET 8, PostgreSQL, and RabbitMQ.

## Features

- Register, update, list, and remove motorcycles (with unique plate validation)
- Register couriers (with unique CNPJ and CNH, CNH type validation)
- Upload courier CNH images (PNG/BMP only, stored on disk)
- Create and manage rentals with pricing rules and penalties
- Publish and consume motorcycle created events via RabbitMQ
- Filter motorcycles by plate
- Fully documented with Swagger

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/)
- [RabbitMQ](https://www.rabbitmq.com/)
- (Optional) [Docker](https://www.docker.com/) and [Docker Compose](https://docs.docker.com/compose/)

## Getting Started

### 1. Clone the repository

```sh
git clone https://github.com/victorteixeira/motorcycle-rental-api.git
cd motorcycle-rental-api
```

### 2. Configure environment variables

Create a file named `appsettings.Development.json` in `src/Moto.Rentals.Api` with your connection strings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=Moto_rentals;Username=postgres;Password=yourpassword"
  },
  "Rabbit": {
    "Host": "localhost",
    "User": "guest",
    "Pass": "guest",
    "Exchange": "Moto.motorcycles",
    "Queue": "Moto.motorcycles.created"
  },
  "Storage": {
    "CnhRoot": "./storage/cnh"
  }
}
```

### 3. Start dependencies (Postgres & RabbitMQ)

#### With Docker Compose (recommended)

```sh
docker-compose up -d
```

#### Or manually

- Start PostgreSQL on port 5432
- Start RabbitMQ on port 5672 (default user/pass: guest/guest)

### 4. Run database migrations

```sh
cd src/Moto.Rentals.Api
dotnet ef database update
```

### 5. Run the API

```sh
dotnet run
```

The API will be available at [http://localhost:5221/swagger](http://localhost:5221/swagger).

---

## Running Tests

```sh
cd tests/Moto.Rentals.Tests
dotnet test
```

---

## API Reference

See the [Swagger documentation](http://localhost:5221/swagger) after running the API.

The API follows the contract:  
https://app.swaggerhub.com/apis-docs/Moto/Moto_desafio_backend/1.0.0

---

## Project Structure

- `src/Moto.Rentals.Api/` - Main API project
- `tests/Moto.Rentals.Tests/` - Unit and integration tests

---

## Notes

- CNH images are stored on disk, not in the database.
- Only couriers with CNH type A or A+B can rent motorcycles.
- Motorcycles with active rentals cannot be deleted.
- All business rules and penalties are implemented as per the challenge description.

---

## License

MIT