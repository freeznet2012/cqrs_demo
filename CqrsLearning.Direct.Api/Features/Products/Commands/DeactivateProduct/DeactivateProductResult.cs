namespace CqrsLearning.Direct.Api.Features.Products.Commands.DeactivateProduct;

public sealed record DeactivateProductResult(bool IsSuccess, bool NotFound)
{
    public static DeactivateProductResult Success()
    {
        return new DeactivateProductResult(true, false);
    }

    public static DeactivateProductResult ProductNotFound()
    {
        return new DeactivateProductResult(false, true);
    }
}
