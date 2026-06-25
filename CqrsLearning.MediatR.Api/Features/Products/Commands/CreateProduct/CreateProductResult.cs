namespace CqrsLearning.MediatR.Api.Features.Products.Commands.CreateProduct;

public sealed record CreateProductResult(
    bool IsSuccess,
    CreateProductResponse? Response,
    IDictionary<string, string[]> Errors)
{
    public static CreateProductResult Success(CreateProductResponse response)
    {
        return new CreateProductResult(true, response, new Dictionary<string, string[]>());
    }

    public static CreateProductResult ValidationFailure(IDictionary<string, string[]> errors)
    {
        return new CreateProductResult(false, null, errors);
    }
}
