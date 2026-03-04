using SwiftOrder.Domain.Enums;
using SwiftOrder.Domain.Exceptions;

namespace SwiftOrder.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string? OrderNumber { get; private set; }

    public string CustomerName { get; private set; } = string.Empty;

    public OrderStatus Status { get; private set; } = OrderStatus.Draft;

    public decimal TotalAmount { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? SubmittedAt { get; private set; }

    public DateTime? ConfirmedAt { get; private set; }

    public List<OrderItem> Items { get; private set; } = new();

    // For EF Core
    private Order() { }

    public Order(string customerName)
    {
        SetCustomerName(customerName);
    }

    public void SetOrderNumber(string orderNumber)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new DomainException("OrderNumber is required.");

        if (!string.IsNullOrWhiteSpace(OrderNumber))
            throw new DomainException("OrderNumber is already set and cannot be changed.");

        OrderNumber = orderNumber.Trim();
    }

    public void AddItem(Guid productId, string productNameSnapshot, decimal unitPrice, int quantity)
    {
        EnsureCanEdit();

        if (productId == Guid.Empty)
            throw new DomainException("ProductId is required.");

        if (string.IsNullOrWhiteSpace(productNameSnapshot))
            throw new DomainException("Product name snapshot is required.");

        if (unitPrice <= 0)
            throw new DomainException("UnitPrice must be greater than zero.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        // If the same product is added again, increase quantity instead of creating a duplicate line.
        var existing = Items.FirstOrDefault(i => i.ProductId == productId && i.UnitPrice == unitPrice);
        if (existing is not null)
        {
            existing.UpdateQuantity(existing.Quantity + quantity);
        }
        else
        {
            var newItem = new OrderItem(Id, productId, productNameSnapshot.Trim(), unitPrice, quantity);
            Items.Add(newItem);
        }

        RecalculateTotal();
    }

    public void Submit()
    {
        EnsureCanEdit();

        if (Items.Count == 0)
            throw new DomainException("Cannot submit an order without items.");

        Status = OrderStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
    }

    public void MarkProcessing()
    {
        if (Status != OrderStatus.Submitted)
            throw new DomainException("Only submitted orders can be processed.");

        Status = OrderStatus.Processing;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Processing)
            throw new DomainException("Only processing orders can be confirmed.");

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Confirmed || Status == OrderStatus.Processing)
            throw new DomainException("Processing/confirmed orders cannot be cancelled.");

        Status = OrderStatus.Cancelled;
    }

    private void SetCustomerName(string customerName)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("Customer name is required.");

        CustomerName = customerName.Trim();
    }

    private void EnsureCanEdit()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException("This order can no longer be modified.");
    }

    private void RecalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.LineTotal);
    }
}