namespace CqrsLearning.Direct.Api.Features.Products.Commands.UpdateProductPrice;

public sealed record UpdateProductPriceResult(
    bool IsSuccess,
    bool NotFound,
    IDictionary<string, string[]> Errors)
{
    public static UpdateProductPriceResult Success()
    {
        return new UpdateProductPriceResult(true, false, new Dictionary<string, string[]>());
    }

    public static UpdateProductPriceResult ProductNotFound()
    {
        return new UpdateProductPriceResult(false, true, new Dictionary<string, string[]>());
    }

    public static UpdateProductPriceResult ValidationFailure(IDictionary<string, string[]> errors)
    {
        return new UpdateProductPriceResult(false, false, errors);
    }
}
