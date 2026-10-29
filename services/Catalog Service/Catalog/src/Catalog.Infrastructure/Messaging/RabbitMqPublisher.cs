using RabbitMQ.Client;

namespace Catalog.Infrastructure.Messaging;

public sealed class RabbitMqPublisher
{
    private readonly RabbitMqConnection _connection;

    public RabbitMqPublisher(RabbitMqConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync(
    string routingKey,
    byte[] body,
    CancellationToken cancellationToken = default)
    {
        var connection = await _connection.GetConnectionAsync(
            cancellationToken);

        var options = new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true);

        await using var channel = await connection.CreateChannelAsync(
            options,
            cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: "catalog.events",
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
    queue: "inventory.product-activated",
    durable: true,
    exclusive: false,
    autoDelete: false,
    cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: "inventory.product-activated",
            exchange: "catalog.events",
            routingKey: "catalog.product.activated.v1",
            cancellationToken: cancellationToken);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: "catalog.events",
            routingKey: routingKey,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }
}
