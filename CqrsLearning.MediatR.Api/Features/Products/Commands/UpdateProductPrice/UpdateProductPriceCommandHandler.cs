using CqrsLearning.MediatR.Api.Data;
using MediatR;

namespace CqrsLearning.MediatR.Api.Features.Products.Commands.UpdateProductPrice;

public sealed class UpdateProductPriceCommandHandler
    : IRequestHandler<UpdateProductPriceCommand, UpdateProductPriceResult>
{
    private readonly AppDbContext _dbContext;

    public UpdateProductPriceCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UpdateProductPriceResult> Handle(
        UpdateProductPriceCommand request,
        CancellationToken cancellationToken)
    {
        var errors = Validate(request);

        if (errors.Count > 0)
        {
            return UpdateProductPriceResult.ValidationFailure(errors);
        }

        var product = await _dbContext.Products.FindAsync(
            [request.ProductId],
            cancellationToken);

        if (product is null)
        {
            return UpdateProductPriceResult.ProductNotFound();
        }

        product.UpdatePrice(request.Price);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return UpdateProductPriceResult.Success();
    }

    private static Dictionary<string, string[]> Validate(UpdateProductPriceCommand command)
    {
        var errors = new Dictionary<string, string[]>();

        if (command.Price < 0)
        {
            errors[nameof(command.Price)] = ["Product price cannot be negative."];
        }

        return errors;
    }
}
