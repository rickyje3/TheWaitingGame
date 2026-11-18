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
    [field: SerializeField] public bool HasValue { get; private set; }

    [field: SerializeField] public TItemType Value { get; private set; }

    private Optional(bool hasValue, TItemType value)
    {
        HasValue = hasValue;
        Value = value;
    }

    /// <summary>
    /// Unwraps the optional value, returning true if the value is present and assigning it to the out parameter.
    /// If the value is not present, returns false and assigns the default value of TItemType to the out parameter.
    /// This method provides a safe way to access the value of the optional without throwing an exception if the value is not present.
    /// </summary>
    /// <param name="value">The optional value, if present.</param>
    /// <returns>True if the value is present, false otherwise.</returns>
    public bool Unwrap(out TItemType value)
    {
        value = Value;
        return HasValue;
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


    /// <summary>
    /// A match function that returns the value if present,
    /// or invokes the 'none' function to provide a default value if not present.
    /// </summary>
    /// <param name="none"></param>
    /// <returns></returns>
    public TItemType MatchValueOrNone(Func<TItemType> none) => Match(some: value => value, none: none);

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