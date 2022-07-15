using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Genjin.Core.Primitives;
// Real-Time Collision Detection, Christer Ericson, 2005. Chapter 4.2; Bounding Volumes - Axis-aligned Bounding Boxes (AABBs). pg 77 
// MIT-licensed (https://github.com/craftworkgames/MonoGame.Extended)

/// <summary>
///     An axis-aligned, four sided, two dimensional box defined by a top-left position (<see cref="X" /> and
///     <see cref="Y" />) and a size (<see cref="Width" /> and <see cref="Height" />).
/// </summary>
/// <remarks>
///     <para>
///         An <see cref="Box" /> is categorized by having its faces oriented in such a way that its
///         face normals are at all times parallel with the axes of the given coordinate system.
///     </para>
///     <para>
///         The bounding <see cref="Box" /> of a rotated <see cref="Box" /> will be equivalent or larger
///         in size than the original depending on the angle of rotation.
///     </para>
/// </remarks>
[DebuggerDisplay("{DebugDisplayString,nq}")]
public record struct Box(float X, float Y, float Width, float Height) {
    /// <summary>
    ///     The <see cref="Box" /> with <see cref="X" />, <see cref="Y" />, <see cref="Width" /> and
    ///     <see cref="Height" /> all set to <code>0.0f</code>.
    /// </summary>
    public static readonly Box Empty = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="Box" /> structure from the specified top-left
    ///     <see cref="Vector2" /> and the extents <see cref="Primitives.Size" />.
    /// </summary>
    /// <param name="position">The top-left point.</param>
    /// <param name="size">The extents.</param>
    public Box(Vector2 position, Size size) : this(position.X, position.Y, size.Width, size.Height) {
        X = position.X;
        Y = position.Y;
        Width = size.Width;
        Height = size.Height;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Box FromCenter(Vector2 center, Size size) {
        return new Box(center.X - size.Width / 2, center.Y - size.Height / 2, size.Width, size.Height);
    }

    /// <summary>
    ///     Gets the x-coordinate of the left edge of this <see cref="Box" />.
    /// </summary>
    public float Left => X;

    /// <summary>
    ///     Gets the x-coordinate of the right edge of this <see cref="Box" />.
    /// </summary>
    public float Right => X + Width;

    /// <summary>
    ///     Gets the y-coordinate of the top edge of this <see cref="Box" />.
    /// </summary>
    public float Top => Y;

    /// <summary>
    ///     Gets the y-coordinate of the bottom edge of this <see cref="Box" />.
    /// </summary>
    public float Bottom => Y + Height;

    /// <summary>
    ///     Gets a value indicating whether this <see cref="Box" /> has a <see cref="X" />, <see cref="Y" />,
    ///     <see cref="Width" />,
    ///     <see cref="Height" /> all equal to <code>0.0f</code>.
    /// </summary>
    /// <value>
    ///     <c>true</c> if this instance is empty; otherwise, <c>false</c>.
    /// </value>
    public bool IsEmpty => Width.Equals(0) && Height.Equals(0) && X.Equals(0) && Y.Equals(0);

    /// <summary>
    ///     Gets the <see cref="Vector2" /> representing the the top-left of this <see cref="Box" />.
    /// </summary>
    public Vector2 Position {
        get { return new Vector2(X, Y); }
        set {
            X = value.X;
            Y = value.Y;
        }
    }

    /// <summary>
    ///     Gets the <see cref="Primitives.Size" /> representing the extents of this <see cref="Box" />.
    /// </summary>
    public Size Size {
        get { return new Size(Width, Height); }
        set {
            Width = value.Width;
            Height = value.Height;
        }
    }

    /// <summary>
    ///     Gets the <see cref="Vector2" /> representing the center of this <see cref="Box" />.
    /// </summary>
    public Vector2 Center => new(X + (Width * 0.5f), Y + (Height * 0.5f));

    /// <summary>
    ///     Gets the <see cref="Vector2" /> representing the top-left of this <see cref="Box" />.
    /// </summary>
    public Vector2 TopLeft => new(X, Y);

    /// <summary>
    ///     Gets the <see cref="Vector2" /> representing the top-right of this <see cref="Box" />.
    /// </summary>
    public Vector2 TopRight => new(X + Width, Y);

    /// <summary>
    ///     Gets the <see cref="Vector2" /> representing the bottom-left of this <see cref="Box" />.
    /// </summary>
    public Vector2 BottomLeft => new(X, Y + Height);

    /// <summary>
    ///     Gets the <see cref="Vector2" /> representing the bottom-right of this <see cref="Box" />.
    /// </summary>
    public Vector2 BottomRight => new(X + Width, Y + Height);

    internal string DebugDisplayString => $"{X}  {Y}  {Width}  {Height}";

    public static Box Transform(in Box rectangle, in Matrix3x2 transformMatrix) {
        var center = rectangle.Center;
        var halfExtents = (Vector2) rectangle.Size * 0.5f;

        PrimitivesHelper.TransformRectangle(ref center, ref halfExtents, transformMatrix);

        return new Box(center.X - halfExtents.X, center.Y - halfExtents.Y, halfExtents.X * 2, halfExtents.Y * 2);
    }

    /// <summary>
    ///     Computes the <see cref="Box" /> that contains the two specified
    ///     <see cref="Box" /> structures.
    /// </summary>
    public static Box Union(in Box first, in Box second) {
        return new Box(
            Math.Min(first.X, second.X),
            Math.Min(first.Y, second.Y),
            Math.Max(first.Right, second.Right) - Math.Min(first.Left, second.Left),
            Math.Max(first.Bottom, second.Bottom) - Math.Min(first.Top, second.Top)
        );
    }
    
    public Box Union(in Box second) => Union(this, second);

    /// <summary>
    ///     Computes the <see cref="Box" /> that is in common between the two specified <see cref="Box" /> structures.
    /// </summary>
    public static Box Intersection(in Box first,
        in Box second) {
        var firstMinimum = first.TopLeft;
        var firstMaximum = first.BottomRight;
        var secondMinimum = second.TopLeft;
        var secondMaximum = second.BottomRight;

        var minimum = Vector2.Max(firstMinimum, secondMinimum);
        var maximum = Vector2.Min(firstMaximum, secondMaximum);

        if (maximum.X < minimum.X || maximum.Y < minimum.Y)
            return new Box();
        else
            return new Box(minimum.X, minimum.Y, maximum.X - minimum.X, maximum.Y - minimum.Y);
    }
    
    public Box Intersection(in Box second) => Intersection(this, second);

    /// <summary>
    ///     Determines whether the two specified <see cref="Box" /> structures intersect.
    /// </summary>
    /// <param name="first">The first rectangle.</param>
    /// <param name="second">The second rectangle.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="first" /> intersects with the <see cref="second" />; otherwise, <c>false</c>.
    /// </returns>
    public static bool Intersects(in Box first, in Box second) {
        return first.X < second.X + second.Width && first.X + first.Width > second.X &&
            first.Y < second.Y + second.Height && first.Y + first.Height > second.Y;
    }

    /// <summary>
    ///     Determines whether the specified <see cref="Box" /> intersects with this
    ///     <see cref="Box" />.
    /// </summary>
    /// <param name="rectangle">The bounding rectangle.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="rectangle" /> intersects with this
    ///     <see cref="Box" />; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool Intersects(Box rectangle) {
        return Intersects(this, rectangle);
    }

    /// <summary>
    ///     Determines whether the specified <see cref="Box" /> contains the specified
    ///     <see cref="Vector2" />.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <param name="point">The point.</param>
    /// <returns>
    ///     <c>true</c> if the <paramref name="rectangle" /> contains the <paramref name="point" />; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public static bool Contains(in Box rectangle, in Vector2 point) {
        return rectangle.X <= point.X && point.X < rectangle.X + rectangle.Width && rectangle.Y <= point.Y &&
            point.Y < rectangle.Y + rectangle.Height;
    }

    /// <summary>
    ///     Determines whether this <see cref="Box" /> contains the specified
    ///     <see cref="Vector2" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>
    ///     <c>true</c> if the this <see cref="Box" /> contains the <paramref name="point" />; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool Contains(Vector2 point) {
        return Contains(this, point);
    }

    /// <summary>
    ///     Computes the squared distance from this <see cref="Box" /> to a <see cref="Vector2" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The squared distance from this <see cref="Box" /> to the <paramref name="point" />.</returns>
    public float SquaredDistanceTo(in Vector2 point) {
        return PrimitivesHelper.SquaredDistanceToPointFromRectangle(TopLeft, BottomRight, point);
    }

    /// <summary>
    ///     Computes the distance from this <see cref="Box" /> to a <see cref="Vector2" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The distance from this <see cref="Box" /> to the <paramref name="point" />.</returns>
    public float DistanceTo(in Vector2 point) {
        return (float) Math.Sqrt(SquaredDistanceTo(point));
    }

    /// <summary>
    ///     Computes the closest <see cref="Vector2" /> on this <see cref="Box" /> to a specified
    ///     <see cref="Vector2" />.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns>The closest <see cref="Vector2" /> on this <see cref="Box" /> to the <paramref name="point" />.</returns>
    public Vector2 ClosestPointTo(in Vector2 point) {
        return PrimitivesHelper.ClosestPointToPointFromRectangle(TopLeft, BottomRight, point);
    }

    //TODO: Document this.
    public void Inflate(float horizontalAmount, float verticalAmount) {
        X -= horizontalAmount;
        Y -= verticalAmount;
        Width += horizontalAmount * 2;
        Height += verticalAmount * 2;
    }

    //TODO: Document this.
    public void Offset(float offsetX, float offsetY) {
        X += offsetX;
        Y += offsetY;
    }

    //TODO: Document this.
    public void Offset(Vector2 amount) {
        X += amount.X;
        Y += amount.Y;
    }

    /// <summary>
    ///     Performs an implicit conversion from a <see cref="System.Drawing.Rectangle" /> to a <see cref="Box" />.
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>
    ///     The resulting <see cref="Box" />.
    /// </returns>
    public static implicit operator Box(System.Drawing.Rectangle rectangle) {
        return new Box {
            X = rectangle.X,
            Y = rectangle.Y,
            Width = rectangle.Width,
            Height = rectangle.Height
        };
    }

    public static explicit operator System.Drawing.Rectangle(Box rectangle) {
        return new System.Drawing.Rectangle((int) rectangle.X, (int) rectangle.Y, (int) rectangle.Width,
            (int) rectangle.Height);
    }
    public static explicit operator System.Drawing.RectangleF(Box rectangle) {
        return new System.Drawing.RectangleF(rectangle.X, rectangle.Y, rectangle.Width,
            rectangle.Height);
    }

    /// <summary>
    ///     Returns a <see cref="string" /> that represents this <see cref="Box" />.
    /// </summary>
    /// <returns>
    ///     A <see cref="string" /> that represents this <see cref="Box" />.
    /// </returns>
    public override string ToString() {
        return $"{{X: {X}, Y: {Y}, Width: {Width}, Height: {Height}";
    }
}
