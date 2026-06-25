using CqrsLearning.Direct.Api.Application.Abstractions;
using CqrsLearning.Direct.Api.Data;

namespace CqrsLearning.Direct.Api.Features.Products.Commands.UpdateProductPrice;

public sealed class UpdateProductPriceCommandHandler
    : ICommandHandler<UpdateProductPriceCommand, UpdateProductPriceResult>
{
    private readonly AppDbContext _dbContext;

    public UpdateProductPriceCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UpdateProductPriceResult> Handle(
        UpdateProductPriceCommand command,
        CancellationToken cancellationToken)
    {
        var errors = Validate(command);

        if (errors.Count > 0)
        {
            return UpdateProductPriceResult.ValidationFailure(errors);
        }

        var product = await _dbContext.Products.FindAsync(
            [command.ProductId],
            cancellationToken);

        if (product is null)
        {
            return UpdateProductPriceResult.ProductNotFound();
        }

        product.UpdatePrice(command.Price);

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
