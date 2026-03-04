namespace SwiftOrder.Worker;

public sealed class WorkerRabbitOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string Exchange { get; set; } = "swiftorder.exchange";
    public string Queue { get; set; } = "swiftorder.order.submitted";
}