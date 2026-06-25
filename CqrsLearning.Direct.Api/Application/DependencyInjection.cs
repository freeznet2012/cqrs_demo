using CqrsLearning.Direct.Api.Features.Products.Commands.CreateProduct;
using CqrsLearning.Direct.Api.Features.Products.Commands.DeactivateProduct;
using CqrsLearning.Direct.Api.Features.Products.Commands.UpdateProductPrice;
using CqrsLearning.Direct.Api.Features.Products.Queries.GetProductById;
using CqrsLearning.Direct.Api.Features.Products.Queries.GetProducts;

namespace CqrsLearning.Direct.Api.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateProductCommandHandler>();
        services.AddScoped<DeactivateProductCommandHandler>();
        services.AddScoped<GetProductByIdQueryHandler>();
        services.AddScoped<GetProductsQueryHandler>();
        services.AddScoped<UpdateProductPriceCommandHandler>();

        return services;
    }
}
