using SwiftOrder.Domain.Enums;
using SwiftOrder.Domain.Exceptions;

namespace SwiftOrder.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string OrderNumber { get; private set; } = string.Empty;

    public string CustomerName { get; private set; } = string.Empty;

    public OrderStatus Status { get; private set; } = OrderStatus.Draft;

    public decimal TotalAmount { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public DateTime? SubmittedAt { get; private set; }

    public DateTime? ConfirmedAt { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

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

        OrderNumber = orderNumber.Trim();
    }

    public void AddItem(Guid productId, string productNameSnapshot, decimal unitPrice, int quantity)
    {
        EnsureCanEdit();

        var newItem = new OrderItem(productId, productNameSnapshot, unitPrice, quantity);
        _items.Add(newItem);

        RecalculateTotal();
    }

    public void Submit()
    {
        EnsureCanEdit();

        if (_items.Count == 0)
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
        if (Status != OrderStatus.Processing && Status != OrderStatus.Submitted)
            throw new DomainException("Only submitted/processing orders can be confirmed.");

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Confirmed)
            throw new DomainException("Confirmed orders cannot be cancelled.");

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
        TotalAmount = _items.Sum(i => i.LineTotal);
    }
}