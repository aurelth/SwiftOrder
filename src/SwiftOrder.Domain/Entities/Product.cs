using SwiftOrder.Domain.Exceptions;

namespace SwiftOrder.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public string Sku { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int StockQuantity { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // For EF Core
    private Product() { }

    public Product(string name, string sku, decimal price, int stockQuantity)
    {
        SetName(name);
        SetSku(sku);
        SetPrice(price);
        SetStockQuantity(stockQuantity);
    }

    public void Update(string name, string sku, decimal price, int stockQuantity, bool isActive)
    {
        SetName(name);
        SetSku(sku);
        SetPrice(price);
        SetStockQuantity(stockQuantity);

        IsActive = isActive;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required.");

        Name = name.Trim();
    }

    private void SetSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            throw new DomainException("Product SKU is required.");

        Sku = sku.Trim().ToUpperInvariant();
    }

    private void SetPrice(decimal price)
    {
        if (price <= 0)
            throw new DomainException("Product price must be greater than zero.");

        Price = price;
    }

    private void SetStockQuantity(int stockQuantity)
    {
        if (stockQuantity < 0)
            throw new DomainException("Stock quantity cannot be negative.");

        StockQuantity = stockQuantity;
    }

    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Increase quantity must be greater than zero.");

        StockQuantity += quantity;
    }

    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Decrease quantity must be greater than zero.");

        if (StockQuantity - quantity < 0)
            throw new DomainException("Insufficient stock.");

        StockQuantity -= quantity;
    }
}