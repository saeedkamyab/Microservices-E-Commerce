using Catalog.Domain.Enums;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Entities;

public sealed class CategoryAttributeDefinition
{
    public Guid Id { get; private set; }
    public Name Name { get; private set; } = null!;
    public AttributeType Type {  get; private set; }
    public bool IsRequired { get; private set; }

    private readonly List<AttributeOption> _options = new();
    public IReadOnlyCollection<AttributeOption> Options =>
  _options.AsReadOnly();

    private CategoryAttributeDefinition()
    {
        //Ef Core
    }

    private CategoryAttributeDefinition(Guid id, Name name,
       AttributeType type,bool isRequired)
    {
        Id = id;
        Name = name;
        Type = type;
        IsRequired = isRequired;
    }

    public static CategoryAttributeDefinition Create(Name name,
      AttributeType type,
      bool isRequired)
    {
        ArgumentNullException.ThrowIfNull(name);

        return new CategoryAttributeDefinition(
           Guid.NewGuid(),
           name,
           type,
           isRequired);
    }
    public void Rename(Name newName)
    {
        ArgumentNullException.ThrowIfNull(newName);

        if (Name == newName)
            return;

        Name = newName;
    }

    public void ChangeRequired(bool isRequired)
    {
        if (IsRequired == isRequired)
            return;

        IsRequired = isRequired;
    }
    public static CategoryAttributeDefinition Rehydrate(
    Guid id,
    Name name,
    AttributeType type,
    bool isRequired)
    {
        return new CategoryAttributeDefinition(
            id,
            name,
            type,
            isRequired);
    }
    public void AddOption(AttributeOption option)
    {
        ArgumentNullException.ThrowIfNull(option);

        if (Type != AttributeType.Option)
            throw new InvalidOperationException("Options can only be added to Option attributes.");

        if (_options.Contains(option))
            return;

        _options.Add(option);

    }
    public void RemoveOption(AttributeOption option)
    {
        ArgumentNullException.ThrowIfNull(option);

        if (Type != AttributeType.Option)
            throw new InvalidOperationException(
                "Options can only be removed from Option attributes.");

        _options.Remove(option);
    }

    public void ValidateValue(ProductSpecificationValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.Type != Type)
            throw new InvalidOperationException(
                $"Value type must be {Type}.");

        if (Type == AttributeType.Option)
        {
            var option = (AttributeOption)value.Value;

            if (!_options.Contains(option))
                throw new InvalidOperationException(
                    "The selected option is not valid for this attribute.");
        }
    }
}
