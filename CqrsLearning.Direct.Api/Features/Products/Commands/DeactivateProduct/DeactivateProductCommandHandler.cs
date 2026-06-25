using CqrsLearning.Direct.Api.Application.Abstractions;
using CqrsLearning.Direct.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CqrsLearning.Direct.Api.Features.Products.Commands.DeactivateProduct;

public sealed class DeactivateProductCommandHandler
    : ICommandHandler<DeactivateProductCommand, DeactivateProductResult>
{
    private readonly AppDbContext _dbContext;

    public DeactivateProductCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeactivateProductResult> Handle(
        DeactivateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .Where(product => product.Id == command.ProductId && product.IsActive)
            .SingleOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return DeactivateProductResult.ProductNotFound();
        }

        product.Deactivate();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return DeactivateProductResult.Success();
    }
}
