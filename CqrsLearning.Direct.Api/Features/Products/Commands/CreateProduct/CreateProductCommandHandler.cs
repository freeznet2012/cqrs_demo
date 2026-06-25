using CqrsLearning.Direct.Api.Application.Abstractions;
using CqrsLearning.Direct.Api.Data;
using CqrsLearning.Direct.Api.Domain;

namespace CqrsLearning.Direct.Api.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    private readonly AppDbContext _dbContext;

    public CreateProductCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateProductResult> Handle(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var errors = Validate(command);

        if (errors.Count > 0)
        {
            return CreateProductResult.ValidationFailure(errors);
        }

        var product = new Product(command.Name.Trim(), command.Price);

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreateProductResult.Success(new CreateProductResponse(product.Id));
    }

    private static Dictionary<string, string[]> Validate(CreateProductCommand command)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            errors[nameof(command.Name)] = ["Product name is required."];
        }
        else if (command.Name.Length > 150)
        {
            errors[nameof(command.Name)] = ["Product name cannot exceed 150 characters."];
        }

        if (command.Price < 0)
        {
            errors[nameof(command.Price)] = ["Product price cannot be negative."];
        }

        return errors;
    }
}
