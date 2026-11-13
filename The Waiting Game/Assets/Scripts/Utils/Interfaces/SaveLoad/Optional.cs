/// <summary>
/// A struct that represents an optional value.
/// This can be used to represent a value that may or may not be present, similar to the concept of nullable types in C#.
/// This is especially useful for structs or primitive types that cannot be null,
/// allowing for a more expressive way to handle optional values.
/// </summary>
/// <typeparam name="TItemType"></typeparam>
public struct Optional<TItemType>
{
    public bool HasValue { get; private set; }
    public TItemType Value { get; private set; }

    private Optional(bool hasValue, TItemType value)
    {
        HasValue = hasValue;
        Value = value;
    }

    public static Optional<TItemType> Some(TItemType value) => new(true, value);

    public static Optional<TItemType> None => new(false, default);
}