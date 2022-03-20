using System.Drawing;

namespace Genjin.Core;

public static class ColorExtensions {
    public static Color WithAlpha(this Color color, int alpha) => Color.FromArgb(alpha, color);
    public static Color WithAlpha(this Color color, float alpha) => Color.FromArgb((int)(alpha * 256 % 256), color);
}
