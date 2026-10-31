using RabbitMQ.Client;

namespace Catalog.Infrastructure.Messaging;

public sealed class RabbitMqPublisher
{
    private readonly RabbitMqConnection _connection;
    private const string ExchangeName = "catalog.events";
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
            exchange: ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
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
