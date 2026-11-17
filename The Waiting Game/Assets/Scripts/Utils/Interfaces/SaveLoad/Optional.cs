using System;
using UnityEngine;

/// <summary>
/// A struct that represents an optional value.
/// This can be used to represent a value that may or may not be present, similar to the concept of nullable types in C#.
/// This is especially useful for structs or primitive types that cannot be null,
/// allowing for a more expressive way to handle optional values.
/// </summary>
/// <typeparam name="TItemType">The type of the value.</typeparam>
[Serializable]
public struct Optional<TItemType>
{
    [field: SerializeField]
    public bool HasValue { get; private set; }

    [field: SerializeField]
    public TItemType Value { get; private set; }

    private Optional(bool hasValue, TItemType value)
    {
        HasValue = hasValue;
        Value = value;
    }

    /// <summary>
    /// Matches the optional value to either a 'some' case or a 'none' case.
    /// If the optional has a value, the 'some' function is invoked with the value.
    /// If the optional does not have a value, the 'none' function is invoked to provide a default value.
    /// </summary>
    /// <param name="some">The function to invoke if the optional has a value.</param>
    /// <param name="none">The function to invoke if the optional does not have a value.</param>
    /// <typeparam name="TValueType">The type of the value returned by the matching functions.</typeparam>
    /// <returns>The result of the matching function.</returns>
    public TValueType Match<TValueType>(Func<TItemType, TValueType> some, Func<TValueType> none)
    {
        // If there is a value, invoke the 'some' function with the value and return the result.
        if (HasValue)
            return some(Value);

        // If there is no value, invoke the 'none' function to get a default value.
        return none();
    }

    public static Optional<TItemType> Some(TItemType value) => new(true, value);

    public static Optional<TItemType> None => new(false, default);

    /// <summary>
    /// Implicitly converts a value of type TItemType to an Optional.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator Optional<TItemType>(TItemType value) => Some(value);
}

public static class Optional
{
    public static Optional<TItemType> Some<TItemType>(TItemType value) => Optional<TItemType>.Some(value);
}