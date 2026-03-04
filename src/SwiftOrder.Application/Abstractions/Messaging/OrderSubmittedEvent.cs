namespace SwiftOrder.Application.Abstractions.Messaging;

public sealed record OrderSubmittedEvent(
    Guid OrderId,
    string OrderNumber,
    DateTime SubmittedAtUtc
);