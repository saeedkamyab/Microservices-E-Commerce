namespace Catalog.Domain.ValueObjects;

public sealed record Price
{
    public decimal Amount { get; }

    private Price(decimal amount)
    {
        Amount = amount;
    }

    public static Price Create(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException(
                "Price cannot be negative.",
                nameof(amount));

        return new Price(amount);
    }

}
