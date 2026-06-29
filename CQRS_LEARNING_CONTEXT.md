# CQRS Learning Project Context

This file captures the project plan and decisions so the work can continue in a new chat without losing context.

## User Context

- The user has around 8 years of .NET experience.
- The user has not used CQRS before and wants to learn it properly.
- The user wants a hands-on .NET API project, built step by step.
- The assistant must not create everything in one go.
- For each step, the assistant should first explain what it is going to do.
- Changes should only be made after the user explicitly prompts for that step.

## Project Goal

Create a .NET Web API learning solution that demonstrates CQRS progressively:

- CQRS without MediatR
- CQRS with MediatR
- CQRS with a single database
- CQRS with separate read/write databases
- CQRS with projections
- CQRS with the outbox pattern
- CQRS with event sourcing

The goal is learning, not just producing code.

## Current Decisions

- Target framework: latest stable LTS .NET version available at the time.
- As of June 24, 2026, Microsoft lists .NET 10 as the latest LTS version.
- API style: ASP.NET Core Controllers.
- First database: SQLite.
- Domain: Products.
- First implementation style: CQRS without MediatR.
- Second implementation style: CQRS with MediatR.
- Build one stage fully before moving to the next.

## Important Clarification

Using SQLite will not hurt CQRS learning.

CQRS is mainly about separating command and query responsibilities. The database engine is secondary. SQLite keeps the setup simple and lets the learning focus stay on the pattern.

## High-Level Teaching Plan

### 1. Direct CQRS, Single Database

Goal: learn the core CQRS pattern with minimal indirection.

Build:

- Solution
- Web API project
- Products controller
- Product domain/entity
- SQLite with EF Core
- `CreateProductCommand`
- `CreateProductCommandHandler`
- `GetProductsQuery`
- `GetProductsQueryHandler`
- Query DTOs/read models
- Command result types

Learn:

- Commands vs queries
- Commands change state
- Queries fetch data
- Queries should generally return DTOs/read models, not EF entities
- Commands usually return small results such as IDs or status
- `AsNoTracking()` for query paths
- Basic validation placement
- Handler responsibilities

### 2. CQRS With MediatR, Single Database

Goal: learn the common production-style implementation.

Build a second API project, likely:

- `CqrsLearning.MediatR.Api`

Build:

- Commands implementing `IRequest<TResponse>`
- Queries implementing `IRequest<TResponse>`
- Handlers implementing `IRequestHandler<TRequest, TResponse>`
- Controllers using `IMediator.Send(...)`
- Possibly validation pipeline behavior later

Learn:

- What MediatR removes
- What MediatR hides
- Handler discovery through dependency injection
- Pipeline behaviors
- When MediatR helps
- When MediatR becomes unnecessary ceremony

### 3. Separate Read/Write Models, Same Database

Goal: separate conceptual models before using separate databases.

Build:

- Write model/entity: `Product`
- Read models/DTOs such as `ProductListItemDto` and `ProductDetailsDto`
- Query-specific shaping

Learn:

- Write model vs read model
- Domain model vs API contract
- Why read models are shaped for consumers
- Why write models protect rules/invariants

### 4. CQRS With Separate Read/Write Databases

Goal: learn architectural separation.

Build:

- Write SQLite database
- Read SQLite database
- Commands write to the write database
- Queries read from the read database
- Projection mechanism to update read database

Start simple:

- Command handler may update both databases directly first, only to make the idea visible.

Then improve:

- Write DB changes first
- Emit an event/message
- Projection handler updates read DB

Learn:

- Separate read/write persistence
- Projections
- Stale reads
- Eventual consistency
- Failure cases between write DB and read DB
- Why direct dual-write is risky
- Why the outbox pattern exists

### 5. Outbox Pattern

Goal: understand reliable propagation from the write side to the read side.

Build:

- `OutboxMessages` table in the write database
- Command handler writes product change and outbox message in one transaction
- Background worker reads unprocessed outbox messages
- Worker updates the read database
- Worker marks messages as processed

Learn:

- Why distributed transactions are avoided
- Reliable event/message publishing
- Retry behavior
- Idempotent projections
- Duplicate handling
- Failure recovery

### 6. CQRS With Event Sourcing

Goal: learn event sourcing separately from normal CRUD persistence.

Likely create another project or clearly separated module:

- `CqrsLearning.EventSourcing.Api`

Instead of storing only current product state, store events:

- `ProductCreated`
- `ProductNameChanged`
- `ProductPriceChanged`
- `ProductDeactivated`

Build:

- Event store table
- Append-only event persistence
- Aggregate rehydration by replaying events
- Optimistic concurrency using stream version
- Read model projection from events

Learn:

- Event store
- Append-only events
- Aggregate rehydration
- Stream version
- Optimistic concurrency
- Event replay
- Projections
- Snapshots
- Event schema versioning
- Why event sourcing is powerful but costly

### 7. Advanced Production Nuances

Discuss and optionally implement:

- Validation in commands
- Authorization at controller level vs handler level
- Transactions
- Concurrency conflicts
- Idempotent commands
- Duplicate messages
- Retries
- Cancellation tokens
- Logging
- Correlation IDs
- Domain events vs integration events
- API contracts vs internal commands
- Error handling/result pattern
- Vertical slice architecture
- Testing handlers without controllers
- Testing projections
- When CQRS is useful
- When CQRS is overengineering
- When MediatR is overused
- When separate databases are justified
- When event sourcing is justified

## CQRS Nuances To Emphasize

- CQRS does not require separate databases.
- CQRS does not require event sourcing.
- Event sourcing is a separate pattern that is often combined with CQRS.
- Commands should represent business intent, not generic CRUD when behavior matters.
- Queries should not change state.
- Commands should not return large read models.
- Query handlers should usually use `AsNoTracking()` with EF Core.
- Command handlers may need transactions and concurrency checks.
- Read models may be stale when using separate read/write stores.
- Separate databases introduce eventual consistency.
- Directly writing to both write and read databases is simple but risky.
- The outbox pattern helps avoid losing messages after a successful write.
- Projection handlers should be idempotent where possible.
- Event sourcing requires careful event design and versioning.
- Event sourcing should not be introduced just because CQRS is used.

## First Proposed Coding Step

Step 1: Create the clean solution and first API project.

Planned actions:

1. Check installed .NET SDKs using `dotnet --list-sdks`.
2. Create solution `CqrsLearning`.
3. Create first API project `CqrsLearning.Direct.Api`.
4. Use Controllers.
5. Add the project to the solution.
6. Run the app once to verify the clean baseline.

No CQRS code should be added in step 1.

The user should prompt with something like:

```text
Do step 1
```

Only then should the assistant make the step 1 changes.

## Current Status

Step 1 has been completed.

Created:

- `global.json`
- `CqrsLearning.sln`
- `CqrsLearning.Direct.Api`

The workspace is pinned to .NET SDK `9.0.308` because the machine only had a .NET 10 preview SDK, not stable .NET 10.

The first API project:

- Targets `net9.0`
- Uses ASP.NET Core Controllers
- Was created without OpenAPI to keep the baseline simple
- Was added to the solution
- Builds successfully with `dotnet build CqrsLearning.sln`

Verification note:

- The app builds successfully.
- Running with the default x64 `dotnet` host currently fails because x64 `Microsoft.NETCore.App 9.0.12` is missing.
- Installed x64 runtimes include `Microsoft.NETCore.App 9.0.11`, while x64 `Microsoft.AspNetCore.App 9.0.12` is present.
- The app starts under the x86 .NET host because x86 has `Microsoft.NETCore.App 9.0.12`.
- Clean fix: install the missing x64 .NET 9 runtime patch, or update/repair the x64 .NET 9 SDK/runtime installation.

Next proposed step:

Step 2: Replace the template weather forecast sample with the initial Products domain and SQLite setup, still without CQRS handlers.

## Step 2 Status

Step 2 has been completed.

Changed:

- Removed the generated weather forecast controller and model.
- Added a simple `HealthController` at `GET /health`.
- Added `Domain/Product.cs`.
- Added `Data/AppDbContext.cs`.
- Added EF Core SQLite package reference.
- Added `ConnectionStrings:ProductsDb` to `appsettings.json`.
- Configured `AppDbContext` in `Program.cs`.
- Configured SQLite to use a relative database path under `App_Data/products.db`.
- Added startup `EnsureCreated()` for learning convenience.
- Updated the `.http` file to call `/health`.

Important teaching note:

- This step intentionally does not add CQRS handlers yet.
- `Product` is currently the write-side entity.
- Query DTOs/read models will be introduced when the first query is added.
- `EnsureCreated()` is acceptable for this early learning step, but migrations should be introduced later before the project becomes more realistic.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Local machine note:

- The x64 .NET 9 runtime patch mismatch still affects normal `dotnet run`.
- The project code builds correctly.
- Install or repair x64 `Microsoft.NETCore.App 9.0.12` to run normally with the default x64 host.

Next proposed step:

Step 3: Add the first query path without MediatR.

Planned actions:

1. Add `Features/Products/Queries/GetProducts`.
2. Add a `GetProductsQuery` request type.
3. Add a `ProductListItemDto` read model.
4. Add a `GetProductsQueryHandler`.
5. Add a `ProductsController`.
6. Wire the handler with dependency injection.
7. Add `GET /products`.

Teaching focus:

- A query reads data and does not change state.
- The controller should not contain query logic.
- The query should return a read model, not the EF entity.
- EF queries should use `AsNoTracking()` when no update is needed.

## Step 3 Status

Step 3 has been completed.

Changed:

- Added `Features/Products/Queries/GetProducts/GetProductsQuery.cs`.
- Added `Features/Products/Queries/GetProducts/ProductListItemDto.cs`.
- Added `Features/Products/Queries/GetProducts/GetProductsQueryHandler.cs`.
- Added `ProductsController` with `GET /products`.
- Registered `GetProductsQueryHandler` in dependency injection.
- Updated the `.http` file with a `/products` request.

Important teaching notes:

- This is the first CQRS read path.
- `GetProductsQuery` represents the request/intent to read products.
- `ProductListItemDto` is the read model returned to API callers.
- The query handler projects from EF entities into DTOs.
- The query handler uses `AsNoTracking()` because it does not modify products.
- The controller delegates query work to the handler instead of containing data-access logic.
- The `GetProductsQuery` currently has no properties, but keeping the type is useful because filters, paging, or sorting can be added later without changing the handler shape.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Next proposed step:

Step 4: Add the first command path without MediatR.

Planned actions:

1. Add `Features/Products/Commands/CreateProduct`.
2. Add `CreateProductCommand`.
3. Add `CreateProductResponse`.
4. Add `CreateProductCommandHandler`.
5. Add a `POST /products` endpoint.
6. Do basic command validation before saving.
7. Save a new product through EF Core.

Teaching focus:

- A command changes state.
- Commands model intent.
- Command handlers usually return small results.
- Validation must be close to the command path.
- EF tracking is useful on command paths because entities are being persisted.

## Step 4 Status

Step 4 has been completed.

Changed:

- Added `Features/Products/Commands/CreateProduct/CreateProductCommand.cs`.
- Added `Features/Products/Commands/CreateProduct/CreateProductResponse.cs`.
- Added `Features/Products/Commands/CreateProduct/CreateProductResult.cs`.
- Added `Features/Products/Commands/CreateProduct/CreateProductCommandHandler.cs`.
- Added `POST /products` to `ProductsController`.
- Registered `CreateProductCommandHandler` in dependency injection.
- Updated the `.http` file with a sample create-product request.

Important teaching notes:

- This is the first CQRS write path.
- `CreateProductCommand` represents the caller's intent to create a product.
- `CreateProductCommandHandler` owns validation and persistence for that command.
- The handler returns a small result containing the created product ID, not a full product list or large read model.
- Validation failures return a result that the controller maps to HTTP 400 validation problem details.
- Normal user-input validation should not become an unhandled 500 error.
- The command path uses EF tracking because a new entity is being added and saved.
- The query path uses `AsNoTracking()` because it only reads.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Next proposed step:

Step 5: Add `GET /products/{id}` as a second query.

Planned actions:

1. Add `GetProductByIdQuery`.
2. Add `ProductDetailsDto`.
3. Add `GetProductByIdQueryHandler`.
4. Register the handler in DI.
5. Add `GET /products/{id}` to `ProductsController`.
6. Update `POST /products` to use `CreatedAtAction` pointing to the new get-by-id route.

Teaching focus:

- A list read model and details read model are often different.
- A query handler should return `null` or an optional result when data is not found.
- Controllers translate application results into HTTP status codes like 200, 201, 400, and 404.

## Step 5 Status

Step 5 has been completed.

Changed:

- Added `Features/Products/Queries/GetProductById/GetProductByIdQuery.cs`.
- Added `Features/Products/Queries/GetProductById/ProductDetailsDto.cs`.
- Added `Features/Products/Queries/GetProductById/GetProductByIdQueryHandler.cs`.
- Added `GET /products/{id}` to `ProductsController`.
- Registered `GetProductByIdQueryHandler` in dependency injection.
- Updated `POST /products` to return `CreatedAtAction(...)` pointing to `GET /products/{id}`.
- Updated the `.http` file with a get-by-id sample request.

Important teaching notes:

- This is a second query path, separate from the list query.
- `ProductDetailsDto` is intentionally separate from `ProductListItemDto`, even though they currently have the same fields.
- In real systems, list rows and detail screens often diverge.
- The handler returns `ProductDetailsDto?` because not finding a product is a normal query outcome.
- The controller maps `null` to HTTP 404.
- `POST /products` now returns a proper HTTP 201 with a `Location` header generated from the get-by-id action.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Next proposed step:

Step 6: Add a command to update product price.

Planned actions:

1. Add `UpdateProductPriceCommand`.
2. Add `UpdateProductPriceResult`.
3. Add `UpdateProductPriceCommandHandler`.
4. Add `PUT /products/{id}/price`.
5. Return 404 when the product does not exist.
6. Return 400 for invalid price.

Teaching focus:

- Commands should express business intent.
- `UpdateProductPriceCommand` is more intention-revealing than a generic `UpdateProductCommand`.
- Command handlers often need to load tracked entities, change them through domain methods, and save.
- This will introduce a domain behavior method on `Product`.

## Step 6 Status

Step 6 has been completed.

Changed:

- Added `Product.UpdatePrice(decimal price)`.
- Added `Features/Products/Commands/UpdateProductPrice/UpdateProductPriceCommand.cs`.
- Added `Features/Products/Commands/UpdateProductPrice/UpdateProductPriceRequest.cs`.
- Added `Features/Products/Commands/UpdateProductPrice/UpdateProductPriceResult.cs`.
- Added `Features/Products/Commands/UpdateProductPrice/UpdateProductPriceCommandHandler.cs`.
- Added `PUT /products/{id}/price` to `ProductsController`.
- Registered `UpdateProductPriceCommandHandler` in dependency injection.
- Updated the `.http` file with an update-price sample request.

Important teaching notes:

- `UpdateProductPriceCommand` is intentionally specific.
- This is more intention-revealing than a generic `UpdateProductCommand`.
- The command handler loads the product as a tracked EF entity using `FindAsync`.
- The handler calls a domain method, `product.UpdatePrice(...)`, instead of setting properties from the controller.
- The controller maps command outcomes to HTTP status codes:
  - validation failure -> 400
  - missing product -> 404
  - successful update -> 204
- This command returns no response body because the operation succeeded and the client can query the resource if it needs the latest representation.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Next proposed step:

Step 7: Add delete/deactivate behavior.

Recommended choice:

- Prefer `DeactivateProductCommand` over hard delete.

Teaching focus:

- Commands should model business language.
- Many real systems do not physically delete records immediately.
- Queries can choose whether to include inactive records.
- This will introduce a product status flag and query filtering.

## Step 7 Status

Step 7 has been completed.

Changed:

- Added `Product.IsActive`.
- Added `Product.Deactivate()`.
- Configured `IsActive` in `AppDbContext`.
- Updated product list and get-by-id queries to return only active products.
- Added `Features/Products/Commands/DeactivateProduct/DeactivateProductCommand.cs`.
- Added `Features/Products/Commands/DeactivateProduct/DeactivateProductResult.cs`.
- Added `Features/Products/Commands/DeactivateProduct/DeactivateProductCommandHandler.cs`.
- Added `DELETE /products/{id}` to `ProductsController`.
- Registered `DeactivateProductCommandHandler` in dependency injection.
- Updated the `.http` file with a delete/deactivate sample request.

Important teaching notes:

- The HTTP endpoint uses `DELETE`, but the internal command is `DeactivateProductCommand`.
- This keeps the external API familiar while preserving business language inside the application.
- The command soft-deletes by setting `IsActive = false`.
- Query handlers filter inactive products out.
- A deactivated product returns 404 from the normal get-by-id query.
- This is a common real-world CQRS split: write-side state may contain more information than read-side responses expose.

Schema note:

- The project still uses `EnsureCreated()` for learning convenience.
- `EnsureCreated()` creates the database only if it does not already exist.
- It does not apply schema changes to an existing database.
- Since `IsActive` was added after a local `products.db` had already been created, delete the local database before running this version, or move to proper EF Core migrations in the next database-focused step.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Next proposed step:

Step 8: Introduce EF Core migrations.

Planned actions:

1. Add `Microsoft.EntityFrameworkCore.Design`.
2. Add the first migration.
3. Replace startup `EnsureCreated()` with `Migrate()`.
4. Explain why migrations are different from `EnsureCreated()`.
5. Reset or migrate the local SQLite database cleanly.

Teaching focus:

- `EnsureCreated()` is only for simple demos.
- Migrations track schema evolution.
- Real projects should not rely on `EnsureCreated()` once schema changes begin.

## Step 8 Status

Step 8 has been completed.

Changed:

- Added `Microsoft.EntityFrameworkCore.Design` to the API project.
- Added a local .NET tool manifest at `.config/dotnet-tools.json`.
- Installed local `dotnet-ef` version `9.0.0`.
- Added initial EF Core migration:
  - `Data/Migrations/20260624184413_InitialCreate.cs`
  - `Data/Migrations/20260624184413_InitialCreate.Designer.cs`
  - `Data/Migrations/AppDbContextModelSnapshot.cs`
- Replaced startup `dbContext.Database.EnsureCreated()` with `dbContext.Database.Migrate()`.
- Applied the migration with `dotnet ef database update`.
- Created migration-managed SQLite database at `CqrsLearning.Direct.Api/App_Data/products.db`.

Important teaching notes:

- `EnsureCreated()` is a quick demo shortcut.
- `EnsureCreated()` does not use migrations and does not update an existing schema.
- `Migrate()` applies pending migrations and records them in `__EFMigrationsHistory`.
- Migrations are the normal way to evolve relational database schema in EF Core projects.
- The migration creates the `Products` table with `Id`, `Name`, `Price`, `IsActive`, and `CreatedAtUtc`.

Local machine note:

- The machine still has an x64 .NET 9 runtime patch mismatch.
- EF tooling required `DOTNET_ROLL_FORWARD=LatestMajor` during migration generation/update because x64 `Microsoft.NETCore.App 9.0.12` is missing.
- A normal `dotnet build CqrsLearning.sln` succeeds after restore.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.
- `dotnet ef database update` succeeded.
- `CqrsLearning.Direct.Api/App_Data/products.db` exists.

Next proposed step:

Step 9: Clean up dependency registration and introduce simple CQRS handler interfaces.

Planned actions:

1. Add `ICommandHandler<TCommand, TResult>`.
2. Add `IQueryHandler<TQuery, TResult>`.
3. Implement these interfaces in existing handlers.
4. Optionally move handler registrations into an extension method.
5. Keep using direct handler injection, still without MediatR.

Teaching focus:

- Direct CQRS does not require MediatR.
- Interfaces make command/query roles explicit.
- MediatR later becomes easier to understand because its `IRequestHandler<TRequest, TResponse>` is conceptually similar.

## Step 9 Status

Step 9 has been completed.

Changed:

- Added `Application/Abstractions/ICommandHandler.cs`.
- Added `Application/Abstractions/IQueryHandler.cs`.
- Updated command handlers to implement `ICommandHandler<TCommand, TResult>`.
- Updated query handlers to implement `IQueryHandler<TQuery, TResult>`.
- Added `Application/DependencyInjection.cs`.
- Moved handler DI registrations from `Program.cs` into `services.AddApplication()`.

Important teaching notes:

- The interfaces are not required by .NET.
- The interfaces are not required for CQRS either.
- They are useful here because they make handler roles explicit.
- A command handler has this shape:
  - input: command
  - output: result
  - purpose: change state
- A query handler has this shape:
  - input: query
  - output: read model/result
  - purpose: read state
- This prepares the mental model for MediatR later.
- MediatR's `IRequestHandler<TRequest, TResponse>` is basically a generalized handler interface.
- The current controller still injects concrete handlers directly to keep the direct CQRS version easy to follow.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Next proposed step:

Step 10: Improve validation with a reusable validation/result pattern, or pause and review the current direct CQRS architecture before adding more features.

Recommended next action:

- Review the current direct CQRS architecture before adding more code.

Review focus:

- Request flow for query
- Request flow for command
- Why controllers stay thin
- Why DTOs are separate from entities
- Why command result shapes differ from query result shapes
- What will change when MediatR is introduced later

## Step 10 Status

Step 10 has been completed.

Changed:

- Added `CqrsLearning.Direct.Api.Tests`.
- Added the test project to `CqrsLearning.sln`.
- Added a project reference from tests to `CqrsLearning.Direct.Api`.
- Added `Microsoft.AspNetCore.Mvc.Testing`.
- Added `ProductsApiFactory`.
- Added endpoint integration tests in `ProductsEndpointsTests`.
- Added `public partial class Program;` to the API `Program.cs` so `WebApplicationFactory<Program>` can boot the API.

Test coverage added:

- `POST /products` creates a product and returns 201.
- `GET /products/{id}` returns the created product.
- `GET /products` includes the active product.
- `PUT /products/{id}/price` updates the product price and returns 204.
- `DELETE /products/{id}` deactivates the product and returns 204.
- `GET /products/{id}` returns 404 after deactivation.
- `GET /products` excludes the deactivated product.
- Invalid create input returns 400.

Important teaching notes:

- These are endpoint/integration tests, not unit tests.
- They exercise the real controller routes and ASP.NET Core model binding.
- The test host replaces the normal file-based SQLite database with SQLite in-memory.
- The test database is isolated and does not touch `App_Data/products.db`.
- The app still runs migrations during startup, so tests also verify migration-based schema creation.
- Logging providers are cleared in the test host to avoid Windows Event Log permission issues in the sandbox.

Verification:

- `dotnet test CqrsLearning.sln --no-restore` succeeds when run with `DOTNET_ROLL_FORWARD=LatestMajor`.
- Result: 2 passed, 0 failed.

Local machine note:

- The x64 .NET 9 runtime patch mismatch still requires `DOTNET_ROLL_FORWARD=LatestMajor` for test execution.
- Installing/repairing x64 `Microsoft.NETCore.App 9.0.12` should remove that workaround.

Next proposed phase:

Start the MediatR implementation.

Recommended next step:

Step 11: Create `CqrsLearning.MediatR.Api`.

Planned actions:

1. Create a second controller-based Web API project.
2. Add it to the solution.
3. Add EF Core SQLite and MediatR packages.
4. Keep the same Product domain/API behavior.
5. Rebuild features using MediatR request/handler types.

Teaching focus:

- Compare direct handler injection with `IMediator.Send(...)`.
- Understand what MediatR changes and what stays the same.
- See that MediatR is a dispatcher, not CQRS itself.

## Step 11 Status

Step 11 has been completed.

Created:

- `CqrsLearning.MediatR.Api`

Changed:

- Added the MediatR API project to `CqrsLearning.sln`.
- Added EF Core SQLite package.
- Added EF Core Design package.
- Added MediatR package version `12.4.1`.
- Removed the generated weather forecast sample.
- Added `HealthController` with `GET /health`.
- Added `Domain/Product.cs`.
- Added `Data/AppDbContext.cs`.
- Configured SQLite in `Program.cs`.
- Configured MediatR in `Program.cs`.
- Added `public partial class Program;` for future integration tests.
- Added initial migration under `CqrsLearning.MediatR.Api/Data/Migrations`.
- Applied the migration.
- Created SQLite database at `CqrsLearning.MediatR.Api/App_Data/mediatr-products.db`.
- Updated `CqrsLearning.MediatR.Api.http` to call `/health`.

Important teaching notes:

- The MediatR project currently has the same persistence foundation as the direct CQRS project.
- Product endpoints have not been added yet.
- MediatR is registered with:

```csharp
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
```

- This tells MediatR to scan the API assembly for request handlers.
- In the direct CQRS project, controllers inject handlers directly.
- In the MediatR project, controllers will inject `IMediator` and call `Send(...)`.
- MediatR is not CQRS by itself. It is a dispatcher that helps route command/query request objects to handlers.
- MediatR version `12.4.1` was pinned intentionally as a stable learning version.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.
- `dotnet ef database update` succeeded for the MediatR project.

Local machine note:

- EF migration commands still need `DOTNET_ROLL_FORWARD=LatestMajor` because x64 `Microsoft.NETCore.App 9.0.12` is missing locally.

Next proposed step:

Step 12: Add the first MediatR query endpoint, `GET /products`.

Planned actions:

1. Add `GetProductsQuery : IRequest<IReadOnlyList<ProductListItemDto>>`.
2. Add `ProductListItemDto`.
3. Add `GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductListItemDto>>`.
4. Add `ProductsController`.
5. Inject `IMediator` into the controller.
6. Use `_mediator.Send(new GetProductsQuery(), cancellationToken)`.

Teaching focus:

- Compare direct handler injection to mediator dispatch.
- In direct CQRS: controller knows the exact handler class.
- In MediatR CQRS: controller knows only `IMediator`.
- The query object and handler still exist, so CQRS did not disappear.

## Step 12 Status

Step 12 has been completed.

Changed:

- Added `Features/Products/Queries/GetProducts/GetProductsQuery.cs`.
- Added `Features/Products/Queries/GetProducts/ProductListItemDto.cs`.
- Added `Features/Products/Queries/GetProducts/GetProductsQueryHandler.cs`.
- Added `ProductsController`.
- Added `GET /products`.
- Updated `CqrsLearning.MediatR.Api.http` with a `/products` request.

Important teaching notes:

- In the direct CQRS project, the controller injected `GetProductsQueryHandler`.
- In the MediatR project, the controller injects only `IMediator`.
- The controller sends the query like this:

```csharp
var products = await _mediator.Send(new GetProductsQuery(), cancellationToken);
```

- The query declares its response type by implementing:

```csharp
IRequest<IReadOnlyList<ProductListItemDto>>
```

- The handler implements:

```csharp
IRequestHandler<GetProductsQuery, IReadOnlyList<ProductListItemDto>>
```

- MediatR matches the request type to the handler type.
- CQRS is still present: there is still a query object and a query handler.
- MediatR only removes the controller's direct dependency on the specific handler class.

How MediatR knows where to send a request:

- MediatR is registered in dependency injection in `Program.cs`:

```csharp
builder.Services.AddMediatR(configuration =>
{
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly);
});
```

- `typeof(Program).Assembly` means MediatR scans the `CqrsLearning.MediatR.Api` assembly.
- During scanning, MediatR finds classes that implement `IRequestHandler<TRequest, TResponse>`.
- A request declares its expected response type by implementing `IRequest<TResponse>`.
- Example request:

```csharp
public sealed record GetProductsQuery
    : IRequest<IReadOnlyList<ProductListItemDto>>;
```

- Example handler:

```csharp
public sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, IReadOnlyList<ProductListItemDto>>
```

- When the controller calls:

```csharp
await _mediator.Send(new GetProductsQuery(), cancellationToken);
```

- MediatR looks for this handler service in DI:

```csharp
IRequestHandler<GetProductsQuery, IReadOnlyList<ProductListItemDto>>
```

- The DI container creates `GetProductsQueryHandler`.
- Any dependencies of the handler, such as `AppDbContext`, are also resolved from DI.
- MediatR then calls the handler's `Handle(...)` method.
- MediatR does not bypass DI and does not manually create handlers with `new`.

Mental model:

```text
Controller
  -> IMediator.Send(request)
  -> MediatR finds IRequestHandler<TRequest, TResponse>
  -> DI creates the handler and its dependencies
  -> handler.Handle(...)
  -> result returns to controller
```

Comparison:

```text
Direct CQRS:
Controller -> GetProductsQueryHandler

MediatR CQRS:
Controller -> IMediator -> GetProductsQueryHandler
```

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Next proposed step:

Step 13: Add MediatR `GET /products/{id}`.

Planned actions:

1. Add `GetProductByIdQuery : IRequest<ProductDetailsDto?>`.
2. Add `ProductDetailsDto`.
3. Add `GetProductByIdQueryHandler`.
4. Add `GET /products/{id}` to the MediatR `ProductsController`.
5. Return 404 when the handler returns null.

Teaching focus:

- MediatR does not decide HTTP status codes.
- The handler returns application data/result.
- The controller still maps handler outcomes to HTTP responses.

## Step 13 Status

Step 13 has been completed.

Changed:

- Added `Features/Products/Queries/GetProductById/GetProductByIdQuery.cs`.
- Added `Features/Products/Queries/GetProductById/ProductDetailsDto.cs`.
- Added `Features/Products/Queries/GetProductById/GetProductByIdQueryHandler.cs`.
- Added `GET /products/{id}` to the MediatR `ProductsController`.
- Updated `CqrsLearning.MediatR.Api.http` with a get-by-id request.

Important teaching notes:

- `GetProductByIdQuery` implements `IRequest<ProductDetailsDto?>`.
- The nullable response matters because "not found" is a normal query result.
- The handler returns `null` when the product does not exist or is inactive.
- MediatR does not know or care that `null` should become HTTP 404.
- The controller still maps application results to HTTP:

```csharp
if (product is null)
{
    return NotFound();
}
```

- MediatR dispatches requests to handlers, but controllers still own HTTP behavior.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Next proposed step:

Step 14: Add MediatR `POST /products`.

Planned actions:

1. Add `CreateProductCommand : IRequest<CreateProductResult>`.
2. Add `CreateProductResponse`.
3. Add `CreateProductResult`.
4. Add `CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResult>`.
5. Add `POST /products` to the MediatR controller.
6. Use `_mediator.Send(command, cancellationToken)`.
7. Return validation problem details for invalid input.
8. Return `CreatedAtAction(...)` for success.

Teaching focus:

- Commands also become MediatR requests.
- The controller sends commands and queries the same way.
- The command handler still owns validation and persistence.
- MediatR does not remove the need for result mapping.

## Step 14 Status

Step 14 has been completed.

Changed:

- Added `Features/Products/Commands/CreateProduct/CreateProductCommand.cs`.
- Added `Features/Products/Commands/CreateProduct/CreateProductResponse.cs`.
- Added `Features/Products/Commands/CreateProduct/CreateProductResult.cs`.
- Added `Features/Products/Commands/CreateProduct/CreateProductCommandHandler.cs`.
- Added `POST /products` to the MediatR `ProductsController`.
- Updated `CqrsLearning.MediatR.Api.http` with a create-product request.

Important teaching notes:

- `CreateProductCommand` implements `IRequest<CreateProductResult>`.
- This means the command itself tells MediatR what response/result type to expect.
- The handler implements:

```csharp
IRequestHandler<CreateProductCommand, CreateProductResult>
```

- The controller sends the command through MediatR:

```csharp
var result = await _mediator.Send(command, cancellationToken);
```

- The handler still owns validation and persistence.
- The controller still maps the result to HTTP:
  - validation failure -> 400
  - success -> 201 Created
- MediatR does not replace validation, EF Core, DTOs, result objects, or HTTP response decisions.
- MediatR only dispatches the command to the correct handler through DI.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Next proposed step:

Step 15: Add MediatR `PUT /products/{id}/price`.

Planned actions:

1. Add `UpdateProductPriceCommand : IRequest<UpdateProductPriceResult>`.
2. Add `UpdateProductPriceRequest`.
3. Add `UpdateProductPriceResult`.
4. Add `UpdateProductPriceCommandHandler`.
5. Add `PUT /products/{id}/price` to the MediatR controller.
6. Return 404 for missing product.
7. Return 400 for invalid price.
8. Return 204 for success.

Teaching focus:

- Commands with route data often need a separate request body type.
- The controller combines route data and body data into one command.
- MediatR still dispatches only one request object to one handler.

## Step 15 Status

Step 15 has been completed.

Changed:

- Added `Features/Products/Commands/UpdateProductPrice/UpdateProductPriceCommand.cs`.
- Added `Features/Products/Commands/UpdateProductPrice/UpdateProductPriceRequest.cs`.
- Added `Features/Products/Commands/UpdateProductPrice/UpdateProductPriceResult.cs`.
- Added `Features/Products/Commands/UpdateProductPrice/UpdateProductPriceCommandHandler.cs`.
- Added `PUT /products/{id}/price` to the MediatR `ProductsController`.
- Updated `CqrsLearning.MediatR.Api.http` with an update-price request.

Important teaching notes:

- The route contains the product id.
- The request body contains the new price.
- The controller combines both into one command:

```csharp
new UpdateProductPriceCommand(id, request.Price)
```

- The command implements `IRequest<UpdateProductPriceResult>`.
- The handler implements `IRequestHandler<UpdateProductPriceCommand, UpdateProductPriceResult>`.
- MediatR still dispatches one request object to one handler.
- The controller still maps the result to HTTP:
  - missing product -> 404
  - validation failure -> 400
  - success -> 204

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Next proposed step:

Step 16: Add MediatR `DELETE /products/{id}` with `DeactivateProductCommand`.

Planned actions:

1. Add `DeactivateProductCommand : IRequest<DeactivateProductResult>`.
2. Add `DeactivateProductResult`.
3. Add `DeactivateProductCommandHandler`.
4. Add `DELETE /products/{id}` to the MediatR controller.
5. Return 404 for missing/inactive product.
6. Return 204 for successful deactivation.

Teaching focus:

- The external HTTP verb can be `DELETE`, while the internal command can use business language.
- MediatR does not change the domain decision to soft delete/deactivate.

## Step 16 Status

Step 16 has been completed.

Changed:

- Added `Features/Products/Commands/DeactivateProduct/DeactivateProductCommand.cs`.
- Added `Features/Products/Commands/DeactivateProduct/DeactivateProductResult.cs`.
- Added `Features/Products/Commands/DeactivateProduct/DeactivateProductCommandHandler.cs`.
- Added `DELETE /products/{id}` to the MediatR `ProductsController`.
- Updated `CqrsLearning.MediatR.Api.http` with a delete/deactivate request.

Important teaching notes:

- The public API uses the familiar HTTP verb `DELETE`.
- The internal command uses business language: `DeactivateProductCommand`.
- The command implements `IRequest<DeactivateProductResult>`.
- The handler implements `IRequestHandler<DeactivateProductCommand, DeactivateProductResult>`.
- The handler only deactivates active products.
- Missing or already inactive products return a not-found result.
- The controller maps:
  - not found -> 404
  - success -> 204
- MediatR does not change the business decision to soft delete.
- MediatR also does not decide HTTP semantics; the controller still does.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.

Next proposed step:

Step 17: Add endpoint integration tests for `CqrsLearning.MediatR.Api`.

Planned actions:

1. Add `CqrsLearning.MediatR.Api.Tests`.
2. Add xUnit and `Microsoft.AspNetCore.Mvc.Testing`.
3. Use SQLite in-memory for isolated tests.
4. Test create, list, get-by-id, update price, deactivate, and validation failure.
5. Compare the test code with the direct API tests.

Teaching focus:

- The endpoint behavior should be the same whether the implementation uses direct handlers or MediatR.
- MediatR changes internal dispatch, not the external API contract.

## Step 17 Status

Step 17 has been completed.

Created:

- `CqrsLearning.MediatR.Api.Tests`

Changed:

- Added the MediatR API test project to `CqrsLearning.sln`.
- Added a project reference from `CqrsLearning.MediatR.Api.Tests` to `CqrsLearning.MediatR.Api`.
- Added `Microsoft.AspNetCore.Mvc.Testing`.
- Added `ProductsApiFactory`.
- Added `ProductsEndpointsTests`.

Test coverage added:

- `POST /products` creates a product and returns 201.
- `GET /products/{id}` returns the created product.
- `GET /products` includes the active product.
- `PUT /products/{id}/price` updates the product price and returns 204.
- `DELETE /products/{id}` deactivates the product and returns 204.
- `GET /products/{id}` returns 404 after deactivation.
- `GET /products` excludes the deactivated product.
- Invalid create input returns 400.

Important teaching notes:

- The MediatR endpoint tests intentionally mirror the direct CQRS endpoint tests.
- This proves the public API behavior is the same.
- The internal dispatch differs:

```text
Direct CQRS:
Controller -> Handler

MediatR CQRS:
Controller -> IMediator -> Handler
```

- Endpoint tests care about API behavior, not internal dispatch style.
- The test host replaces the file-based SQLite database with SQLite in-memory.
- The tests still exercise ASP.NET Core routing, model binding, controllers, MediatR dispatch, handlers, EF Core, and migrations.

Verification:

- `dotnet test CqrsLearning.sln --no-restore` succeeds when run with `DOTNET_ROLL_FORWARD=LatestMajor`.
- Result:
  - Direct API tests: 2 passed, 0 failed.
  - MediatR API tests: 2 passed, 0 failed.
  - Total: 4 passed, 0 failed.

Next proposed step:

Step 18: Compare direct CQRS and MediatR CQRS side by side.

Planned discussion:

1. Compare controller dependencies.
2. Compare request/handler definitions.
3. Compare DI registration.
4. Compare test behavior.
5. Explain what MediatR improves.
6. Explain what MediatR costs.
7. Decide whether to add validation pipeline behavior next.

Recommended next coding step after comparison:

- Add a MediatR pipeline behavior for validation/logging to show where MediatR becomes more useful than direct handler calls.

## Step 18 Status

Step 18 has been completed.

Changed:

- Added `CqrsLearning.MediatR.Api/Application/Behaviors/RequestLoggingBehavior.cs`.
- Registered the behavior in `CqrsLearning.MediatR.Api/Program.cs` with `configuration.AddOpenBehavior(typeof(RequestLoggingBehavior<,>));`.
- Updated `README.md` with a MediatR pipeline behavior section.

Important teaching notes:

- This is the first MediatR pipeline behavior.
- The request flow is now:

```text
Controller -> IMediator -> RequestLoggingBehavior -> Handler
```

- The behavior runs around every MediatR command/query.
- The behavior logs the request type before the handler runs.
- The behavior logs elapsed time after the handler completes.
- This does not change API behavior; it adds cross-cutting behavior around handlers.
- This is one of MediatR's real advantages over direct handler injection.

Verification:

- `dotnet build CqrsLearning.sln` succeeds.
- `dotnet test CqrsLearning.sln --no-restore` succeeds with `DOTNET_ROLL_FORWARD=LatestMajor`.
- Result:
  - Direct API tests: 2 passed, 0 failed.
  - MediatR API tests: 2 passed, 0 failed.
  - Total: 4 passed, 0 failed.

Next proposed step:

Step 19: Add validation pipeline behavior.

Planned actions:

1. Introduce a small validation abstraction for MediatR requests.
2. Move command validation out of individual handlers where appropriate.
3. Add a MediatR validation behavior that runs before handlers.
4. Keep controller HTTP mapping explicit.

Teaching focus:

- Pipeline behaviors can centralize cross-cutting concerns.
- Validation can run before the handler executes.
- This helps handlers focus on business behavior and persistence.
