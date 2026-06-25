namespace CqrsLearning.MediatR.Api.Domain;

public sealed class Product
{
    private Product()
    {
    }

    public Product(string name, decimal price)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public void UpdatePrice(decimal price)
    {
        Price = price;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
