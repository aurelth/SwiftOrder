using FluentAssertions;
using SwiftOrder.Domain.Entities;
using SwiftOrder.Domain.Enums;
using SwiftOrder.Domain.Exceptions;

namespace SwiftOrder.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void Submit_should_fail_when_order_has_no_items()
    {
        // Arrange
        var order = new Order("John Doe");

        // Act
        var act = () => order.Submit();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*without items*");
    }

    [Fact]
    public void AddItem_should_recalculate_total_amount()
    {
        // Arrange
        var order = new Order("John Doe");
        var productId = Guid.NewGuid();

        // Act
        order.AddItem(productId, "Product A", 10.00m, 2); // 20.00
        order.AddItem(productId, "Product A", 5.00m, 1);  // +5.00

        // Assert
        order.TotalAmount.Should().Be(25.00m);
        order.Items.Should().HaveCount(2);
    }

    [Fact]
    public void Submit_should_change_status_and_set_submitted_at()
    {
        // Arrange
        var order = new Order("John Doe");
        order.AddItem(Guid.NewGuid(), "Product A", 10.00m, 1);

        // Act
        order.Submit();

        // Assert
        order.Status.Should().Be(OrderStatus.Submitted);
        order.SubmittedAt.Should().NotBeNull();
    }
}