namespace Catalog.Domain.ValueObjects;

public sealed record Name
{
    public string Value { get; }

    private Name(string value)
    {
        Value = value;
    }

    public static Name Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Name cannot be empty !", nameof(value));

        value = value.Trim();

        if (value.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters", nameof(value));

        return new Name(value);
    }

    public override string ToString() => Value;

}
