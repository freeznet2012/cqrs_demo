using CqrsLearning.Direct.Api.Application.Abstractions;
using CqrsLearning.Direct.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace CqrsLearning.Direct.Api.Features.Products.Queries.GetProducts;

public sealed class GetProductsQueryHandler
    : IQueryHandler<GetProductsQuery, IReadOnlyList<ProductListItemDto>>
{
    private readonly AppDbContext _dbContext;

    public GetProductsQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductListItemDto>> Handle(
        GetProductsQuery query,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .OrderBy(product => product.Name)
            .Select(product => new ProductListItemDto(
                product.Id,
                product.Name,
                product.Price,
                product.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
