# CQRS Learning API

This repository is a hands-on CQRS learning project for ASP.NET Core.

It intentionally contains two implementations of the same Products API:

- `CqrsLearning.Direct.Api`: CQRS without MediatR.
- `CqrsLearning.MediatR.Api`: CQRS with MediatR.

The goal is to make the pattern easy to compare, not to build a production product catalog.

## What This Demonstrates

CQRS means separating read operations from write operations:

- Queries read data and do not change state.
- Commands change state and return small results.

This repository shows that CQRS does not require:

- MediatR
- separate databases
- event sourcing
- microservices

Those can be added later, but the basic pattern is just command/query separation.

## Solution Structure

```text
CqrsLearning.sln
|
|-- CqrsLearning.Direct.Api
|   |-- Controllers
|   |-- Features
|   |   |-- Products
|   |       |-- Commands
|   |       |-- Queries
|   |-- Domain
|   |-- Data
|
|-- CqrsLearning.Direct.Api.Tests
|
|-- CqrsLearning.MediatR.Api
|   |-- Controllers
|   |-- Features
|   |   |-- Products
|   |       |-- Commands
|   |       |-- Queries
|   |-- Domain
|   |-- Data
|
|-- CqrsLearning.MediatR.Api.Tests
```

Both APIs expose the same endpoints:

```text
GET    /products
GET    /products/{id}
POST   /products
PUT    /products/{id}/price
DELETE /products/{id}
```

`DELETE /products/{id}` performs a soft delete by deactivating the product.

## Direct CQRS

In the direct CQRS project, controllers call handlers directly.

```text
Controller -> Query/Command Handler -> DbContext
```

Example:

```csharp
var products = await _getProductsQueryHandler.Handle(
    new GetProductsQuery(),
    cancellationToken);
```

This version is useful for learning because the flow is explicit. There is no dispatcher between the controller and the handler.

## MediatR CQRS

In the MediatR project, controllers depend on `IMediator`.

```text
Controller -> IMediator -> Query/Command Handler -> DbContext
```

Example:

```csharp
var products = await _mediator.Send(
    new GetProductsQuery(),
    cancellationToken);
```

The request declares its response type:

```csharp
public sealed record GetProductsQuery
    : IRequest<IReadOnlyList<ProductListItemDto>>;
```

The handler declares the request it handles:

```csharp
public sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductListItemDto>>
```

MediatR is registered like this:

```csharp
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
```

That tells MediatR to scan the API assembly for `IRequestHandler<TRequest, TResponse>` implementations. When `_mediator.Send(...)` is called, MediatR asks the DI container for the matching handler and then calls `Handle(...)`.

MediatR is not CQRS. MediatR is a dispatcher that can be used to implement CQRS.

## Commands And Queries

A query describes a read operation:

```text
GetProductsQuery -> GetProductsQueryHandler -> ProductListItemDto
```

A command describes a write operation:

```text
CreateProductCommand -> CreateProductCommandHandler -> CreateProductResult
```

Commands use business-oriented names where possible:

```text
UpdateProductPriceCommand
DeactivateProductCommand
```

This is intentional. In CQRS, commands should usually describe intent, not just generic CRUD operations.

## Read Models

Queries return DTOs/read models instead of EF entities.

Examples:

```text
ProductListItemDto
ProductDetailsDto
```

This keeps API response shape separate from the write-side entity.

## Persistence

Both APIs use:

- EF Core
- SQLite
- EF Core migrations

The direct API database is:

```text
CqrsLearning.Direct.Api/App_Data/products.db
```

The MediatR API database is:

```text
CqrsLearning.MediatR.Api/App_Data/mediatr-products.db
```

SQLite is used to keep setup simple. The CQRS lessons are about request flow and responsibility separation, not the database engine.

## Tests

Both implementations have endpoint integration tests.

The tests call the real HTTP endpoints through ASP.NET Core's test host and use SQLite in-memory instead of the file database.

They verify that the direct and MediatR versions have the same external API behavior.

## Build And Test

Build:

```powershell
dotnet build CqrsLearning.sln
```

Test:

```powershell
dotnet test CqrsLearning.sln --no-restore
```

On the current machine, test execution may require:

```powershell
$env:DOTNET_ROLL_FORWARD='LatestMajor'
dotnet test CqrsLearning.sln --no-restore
```

That workaround is only needed because the local x64 .NET 9 runtime patch is missing.

## Current Learning Status

Implemented:

- Direct CQRS API
- MediatR CQRS API
- SQLite persistence
- EF Core migrations
- Product list query
- Product details query
- Create product command
- Update product price command
- Deactivate product command
- Endpoint integration tests for both APIs

Planned next topics:

- MediatR pipeline behaviors
- validation pipeline
- logging/timing behavior
- separate read/write models
- separate read/write databases
- projections
- outbox pattern
- event sourcing

For detailed step-by-step learning notes, see:

```text
CQRS_LEARNING_CONTEXT.md
```

