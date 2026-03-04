using SwiftOrder.Domain.Exceptions;

namespace SwiftOrder.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid OrderId { get; private set; }   // EF relationship
    public Guid ProductId { get; private set; }

    public string ProductNameSnapshot { get; private set; } = string.Empty;

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public decimal LineTotal { get; private set; }

    // For EF Core
    private OrderItem() { }

    public OrderItem(Guid orderId, Guid productId, string productNameSnapshot, decimal unitPrice, int quantity)
    {
        if (orderId == Guid.Empty)
            throw new DomainException("OrderId is required.");
        if (productId == Guid.Empty)
            throw new DomainException("ProductId is required.");

        OrderId = orderId;
        ProductId = productId;

        if (string.IsNullOrWhiteSpace(productNameSnapshot))
            throw new DomainException("Product name snapshot is required.");

        ProductNameSnapshot = productNameSnapshot.Trim();

        if (unitPrice <= 0)
            throw new DomainException("UnitPrice must be greater than zero.");

        UnitPrice = unitPrice;

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Quantity = quantity;

        LineTotal = UnitPrice * Quantity;
    }

    public void UpdateQuantity(int quantity)
    {
        SetQuantity(quantity);
        RecalculateLineTotal();
    }

    private void SetSnapshotName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name snapshot is required.");

        ProductNameSnapshot = name.Trim();
    }

    private void SetUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
            throw new DomainException("UnitPrice must be greater than zero.");

        UnitPrice = unitPrice;
    }

    private void SetQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Quantity = quantity;
    }

    private void RecalculateLineTotal()
    {
        LineTotal = UnitPrice * Quantity;
    }
}