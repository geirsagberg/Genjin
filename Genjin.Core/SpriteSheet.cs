using System.Drawing;
using Veldrid;
using Rectangle = System.Drawing.Rectangle;

namespace Genjin.Example;

public record SpriteSheet {
    public SpriteSheet(Texture texture, int columns, int rows) {
        if (texture.Width % columns > 0) {
            throw new ArgumentException("Columns must be divisor of texture width");
        }

        if (texture.Height % rows > 0) {
            throw new ArgumentException("Rows must be divisor of texture height");
        }

        Texture = texture;
        Columns = columns;
        Rows = rows;
    }

    public Texture Texture { get; init; }
    public int Columns { get; init; }
    public int Rows { get; init; }

    public int SpriteWidth => (int) (Texture.Width / Columns);
    public int SpriteHeight => (int) (Texture.Height / Rows);

    public Size SpriteSize => new(SpriteWidth, SpriteHeight);

    public Rectangle GetSpriteRectangle(int column, int row) =>
        new(SpriteWidth * column, SpriteHeight * row, SpriteWidth, SpriteHeight);
}
