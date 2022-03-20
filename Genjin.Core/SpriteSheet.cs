using System.Drawing;
using Peridot.Veldrid;

namespace Genjin.Example;

public record SpriteSheet {
    public SpriteSheet(TextureWrapper texture, int columns, int rows) {
        if (texture.Size.Width % columns > 0) {
            throw new ArgumentException("Columns must be divisor of texture width");
        }

        if (texture.Size.Height % rows > 0) {
            throw new ArgumentException("Rows must be divisor of texture height");
        }

        Texture = texture;
        Columns = columns;
        Rows = rows;
    }

    public TextureWrapper Texture { get; init; }
    public int Columns { get; init; }
    public int Rows { get; init; }

    public int SpriteWidth => Texture.Size.Width / Columns;
    public int SpriteHeight => Texture.Size.Height / Rows;

    public Size SpriteSize => new(SpriteWidth, SpriteHeight);

    public Rectangle GetSpriteRectangle(int column, int row) =>
        new(SpriteWidth * column, SpriteHeight * row, SpriteWidth, SpriteHeight);
}
