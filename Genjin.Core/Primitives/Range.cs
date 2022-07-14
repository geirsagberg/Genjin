namespace Genjin.Core.Primitives; 

/// <summary>
///     Represents a closed interval defined by a minimum and a maximum value of a give type.
/// </summary>
public readonly struct Range<T> : IEquatable<Range<T>> where T : IComparable<T>
{
    public Range(T min, T max)
    {
        if (min.CompareTo(max) > 0 || max.CompareTo(min) < 0)
            throw new ArgumentException("Min has to be smaller than or equal to max.");

        Min = min;
        Max = max;
    }

    public Range(T value)
        : this(value, value)
    {
    }

    /// <summary>
    ///     Gets the minium value of the <see cref="Range{T}" />.
    /// </summary>
    public T Min { get; }

    /// <summary>
    ///     Gets the maximum value of the <see cref="Range{T}" />.
    /// </summary>
    public T Max { get; }


    /// <summary>
    ///     Returns wheter or not this <see cref="Range{T}" /> is degenerate.
    ///     (Min and Max are the same)
    /// </summary>
    public bool IsDegenerate => Min.CompareTo(Max) == 0;

    /// <summary>
    ///     Returns wheter or not this <see cref="Range{T}" /> is proper.
    ///     (Min and Max are not the same)
    /// </summary>
    public bool IsProper => !IsDegenerate;

    public static implicit operator Range<T>(T value) => new(value, value);

    public override string ToString() => $"Range<{typeof(T).Name}> [{Min} {Max}]";

    /// <summary>
    ///     Returns wheter or not the value falls in this <see cref="Range{T}" />.
    /// </summary>
    public bool IsInBetween(T value, bool minValueExclusive = false, bool maxValueExclusive = false)
    {
        if (minValueExclusive)
        {
            if (value.CompareTo(Min) <= 0)
                return false;
        }

        if (value.CompareTo(Min) < 0)
            return false;

        if (maxValueExclusive)
        {
            if (value.CompareTo(Max) >= 0)
                return false;
        }

        return value.CompareTo(Max) <= 0;
    }

    public bool Equals(Range<T> other) => EqualityComparer<T>.Default.Equals(Min, other.Min) && EqualityComparer<T>.Default.Equals(Max, other.Max);

    public override bool Equals(object? obj) => obj is Range<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Min, Max);

    public static bool operator ==(Range<T> left, Range<T> right) {
        return left.Equals(right);
    }

    public static bool operator !=(Range<T> left, Range<T> right) {
        return !(left == right);
    }
}
