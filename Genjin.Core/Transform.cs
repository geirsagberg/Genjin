﻿using System.Numerics;
using Genjin.Core.Primitives;

namespace Genjin.Core;

/**
 * M11 = x scale, rotation
 * M12 = rotation
 * M21 = rotation
 * M22 = y scale, rotation
 * M31 = x translation
 * M32 = y translation
 * Size = Original size
 */
public record Transform {
    private Transform(Vector2 position, float rotation, Vector2 scale, Vector2 origin, Size2F size) {
        Position = position;
        Rotation = rotation;
        Scale = scale;
        Origin = origin;
        Size = size;
    }

    public Transform(Vector2 position, float rotation, Size2F size) : this(position, rotation, Vector2.One,
        new Vector2(size.Width / 2f, size.Height / 2f),
        size) {
    }

    public Size2F Size { get; set; }

    public Vector2 Origin { get; set; }

    public Vector2 Scale { get; set; }

    public float Rotation { get; set; }

    public Vector2 Position { get; set; }

    public RectangleF RectangleF => new(Position.X, Position.Y, Size.Width, Size.Height);

    public bool Intersects(Transform transform) => RectangleF.Intersects(transform.RectangleF);
}
