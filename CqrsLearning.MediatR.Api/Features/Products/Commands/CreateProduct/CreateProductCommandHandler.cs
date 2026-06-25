using CqrsLearning.MediatR.Api.Data;
using CqrsLearning.MediatR.Api.Domain;
using MediatR;

namespace CqrsLearning.MediatR.Api.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly AppDbContext _dbContext;

    public CreateProductCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateProductResult> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var errors = Validate(request);

        if (errors.Count > 0)
        {
            return CreateProductResult.ValidationFailure(errors);
        }

        var product = new Product(request.Name.Trim(), request.Price);

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
