// MIT-licensed (https://github.com/craftworkgames/MonoGame.Extended)
namespace Genjin.Core.Primitives; 

public struct Thickness : IEquatable<Thickness>
{
    public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

    public Thickness(int all)
        : this(all, all, all, all)
    {
    }

    public Thickness(int leftRight, int topBottom)
        : this(leftRight, topBottom, leftRight, topBottom)
    {
    }

    public Thickness(int left, int top, int right, int bottom)
        : this()
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public int Left { get; set; }
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
    public int Width => Left + Right;
    public int Height => Top + Bottom;
    public Size2 Size => new(Width, Height);

    public static implicit operator Thickness(int value)
    {
        return new Thickness(value);
    }

    public bool Equals(Thickness other)
    {
        return Left == other.Left && Right == other.Right && Top == other.Top && Bottom == other.Bottom;
    }

    public static Thickness FromValues(int[] values) {
        return values.Length switch {
            1 => new Thickness(values[0]),
            2 => new Thickness(values[0], values[1]),
            4 => new Thickness(values[0], values[1], values[2], values[3]),
            _ => throw new FormatException("Invalid thickness")
        };
    }

    public static Thickness Parse(string value)
    {
        var values = value
            .Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();

        return FromValues(values);
    }

    public override string ToString()
    {
        if (Left == Right && Top == Bottom)
            return Left == Top ? $"{Left}" : $"{Left} {Top}";

        return $"{Left}, {Right}, {Top}, {Bottom}";
    }

    public override bool Equals(object? obj) {
        return obj is Thickness thickness && Equals(thickness);
    }

    public static bool operator ==(Thickness left, Thickness right) {
        return left.Equals(right);
    }

    public static bool operator !=(Thickness left, Thickness right) {
        return !(left == right);
    }
}
