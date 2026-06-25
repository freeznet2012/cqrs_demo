using System.Net;
using System.Net.Http.Json;

namespace CqrsLearning.MediatR.Api.Tests;

public sealed class ProductsEndpointsTests : IClassFixture<ProductsApiFactory>
{
    private readonly HttpClient _client;

    public ProductsEndpointsTests(ProductsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ProductEndpoints_CreateReadUpdateAndDeactivateProduct()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/products",
            new
            {
                Name = "Mechanical Keyboard",
                Price = 129.99m
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createResponse.Headers.Location);

        var createdProduct = await createResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        Assert.NotNull(createdProduct);

        var details = await _client.GetFromJsonAsync<ProductDetailsResponse>(
            $"/products/{createdProduct.Id}");

        Assert.NotNull(details);
        Assert.Equal(createdProduct.Id, details.Id);
        Assert.Equal("Mechanical Keyboard", details.Name);
        Assert.Equal(129.99m, details.Price);

        var products = await _client.GetFromJsonAsync<List<ProductListItemResponse>>("/products");

        Assert.NotNull(products);
        Assert.Contains(products, product => product.Id == createdProduct.Id);

        var updateResponse = await _client.PutAsJsonAsync(
            $"/products/{createdProduct.Id}/price",
            new { Price = 149.99m });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var updatedDetails = await _client.GetFromJsonAsync<ProductDetailsResponse>(
            $"/products/{createdProduct.Id}");

        Assert.NotNull(updatedDetails);
        Assert.Equal(149.99m, updatedDetails.Price);

        var deactivateResponse = await _client.DeleteAsync($"/products/{createdProduct.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deactivateResponse.StatusCode);

        var getDeactivatedResponse = await _client.GetAsync($"/products/{createdProduct.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getDeactivatedResponse.StatusCode);

        var productsAfterDeactivation = await _client.GetFromJsonAsync<List<ProductListItemResponse>>(
            "/products");

        Assert.NotNull(productsAfterDeactivation);
        Assert.DoesNotContain(productsAfterDeactivation, product => product.Id == createdProduct.Id);
    }

    [Fact]
    public async Task CreateProduct_ReturnsBadRequest_WhenInputIsInvalid()
    {
        var response = await _client.PostAsJsonAsync(
            "/products",
            new
            {
                Name = "",
                Price = -1m
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private sealed record CreateProductResponse(Guid Id);

    private sealed record ProductDetailsResponse(
        Guid Id,
        string Name,
        decimal Price,
        DateTime CreatedAtUtc);

    private sealed record ProductListItemResponse(
        Guid Id,
        string Name,
        decimal Price,
        DateTime CreatedAtUtc);
}
