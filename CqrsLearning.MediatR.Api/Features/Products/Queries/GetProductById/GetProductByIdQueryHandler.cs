using CqrsLearning.MediatR.Api.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CqrsLearning.MediatR.Api.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, ProductDetailsDto?>
{
    private readonly AppDbContext _dbContext;

    public GetProductByIdQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductDetailsDto?> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product => product.Id == request.Id && product.IsActive)
            .Select(product => new ProductDetailsDto(
                product.Id,
                product.Name,
                product.Price,
                product.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
