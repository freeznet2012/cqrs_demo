using CqrsLearning.MediatR.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsLearning.MediatR.Api.Features.Products.Commands.DeactivateProduct;

public sealed class DeactivateProductCommandHandler
    : IRequestHandler<DeactivateProductCommand, DeactivateProductResult>
{
    private readonly AppDbContext _dbContext;

    public DeactivateProductCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeactivateProductResult> Handle(
        DeactivateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .Where(product => product.Id == request.ProductId && product.IsActive)
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
