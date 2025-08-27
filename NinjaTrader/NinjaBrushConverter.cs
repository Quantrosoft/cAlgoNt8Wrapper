/* MIT License
Copyright (c) 2025 Quantrosoft Pty. Ltd.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/

using System.Windows.Media;

public static class NinjaBrushConverter
{
    //public static Brush FromCtraderColor(Color color)
    //{
    //    // Check if the color matches one of the known system ARGB values
    //    switch (color.ToArgb())
    //    {
    //        case 0x00000000:
    //        return Brushes.Transparent;
    //        case 0xFFF0F8FF:
    //        return Brushes.AliceBlue;
    //        case 0xFFFAEBD7:
    //        return Brushes.AntiqueWhite;
    //        //case 0xFF00FFFF:
    //        //return Brushes.Aqua;
    //        case 0xFF7FFFD4:
    //        return Brushes.Aquamarine;
    //        case 0xFFF0FFFF:
    //        return Brushes.Azure;
    //        case 0xFFF5F5DC:
    //        return Brushes.Beige;
    //        case 0xFFFFE4C4:
    //        return Brushes.Bisque;
    //        case 0xFF000000:
    //        return Brushes.Black;
    //        case 0xFFFFEBCD:
    //        return Brushes.BlanchedAlmond;
    //        case 0xFF0000FF:
    //        return Brushes.Blue;
    //        case 0xFF8A2BE2:
    //        return Brushes.BlueViolet;
    //        case 0xFFA52A2A:
    //        return Brushes.Brown;
    //        case 0xFFDEB887:
    //        return Brushes.BurlyWood;
    //        case 0xFF5F9EA0:
    //        return Brushes.CadetBlue;
    //        case 0xFF7FFF00:
    //        return Brushes.Chartreuse;
    //        case 0xFFD2691E:
    //        return Brushes.Chocolate;
    //        case 0xFFFF7F50:
    //        return Brushes.Coral;
    //        case 0xFF6495ED:
    //        return Brushes.CornflowerBlue;
    //        case 0xFFFFF8DC:
    //        return Brushes.Cornsilk;
    //        case 0xFFDC143C:
    //        return Brushes.Crimson;
    //        case 0xFF00FFFF:
    //        return Brushes.Cyan;
    //        case 0xFF00008B:
    //        return Brushes.DarkBlue;
    //        case 0xFF008B8B:
    //        return Brushes.DarkCyan;
    //        case 0xFFB8860B:
    //        return Brushes.DarkGoldenrod;
    //        case 0xFFA9A9A9:
    //        return Brushes.DarkGray;
    //        case 0xFF006400:
    //        return Brushes.DarkGreen;
    //        case 0xFFBDB76B:
    //        return Brushes.DarkKhaki;
    //        case 0xFF8B008B:
    //        return Brushes.DarkMagenta;
    //        case 0xFF556B2F:
    //        return Brushes.DarkOliveGreen;
    //        case 0xFFFF8C00:
    //        return Brushes.DarkOrange;
    //        case 0xFF9932CC:
    //        return Brushes.DarkOrchid;
    //        case 0xFF8B0000:
    //        return Brushes.DarkRed;
    //        case 0xFFE9967A:
    //        return Brushes.DarkSalmon;
    //        case 0xFF8FBC8F:
    //        return Brushes.DarkSeaGreen;
    //        case 0xFF483D8B:
    //        return Brushes.DarkSlateBlue;
    //        case 0xFF2F4F4F:
    //        return Brushes.DarkSlateGray;
    //        case 0xFF00CED1:
    //        return Brushes.DarkTurquoise;
    //        case 0xFF9400D3:
    //        return Brushes.DarkViolet;
    //        case 0xFFFF1493:
    //        return Brushes.DeepPink;
    //        case 0xFF00BFFF:
    //        return Brushes.DeepSkyBlue;
    //        case 0xFF696969:
    //        return Brushes.DimGray;
    //        case 0xFF1E90FF:
    //        return Brushes.DodgerBlue;
    //        case 0xFFB22222:
    //        return Brushes.Firebrick;
    //        case 0xFFFFFAF0:
    //        return Brushes.FloralWhite;
    //        case 0xFF228B22:
    //        return Brushes.ForestGreen;
    //        //case 0xFFFF00FF:
    //        //return Brushes.Fuchsia;
    //        case 0xFFDCDCDC:
    //        return Brushes.Gainsboro;
    //        case 0xFFF8F8FF:
    //        return Brushes.GhostWhite;
    //        case 0xFFFFD700:
    //        return Brushes.Gold;
    //        case 0xFFDAA520:
    //        return Brushes.Goldenrod;
    //        case 0xFF808080:
    //        return Brushes.Gray;
    //        case 0xFF008000:
    //        return Brushes.Green;
    //        case 0xFFADFF2F:
    //        return Brushes.GreenYellow;
    //        case 0xFFF0FFF0:
    //        return Brushes.Honeydew;
    //        case 0xFFFF69B4:
    //        return Brushes.HotPink;
    //        case 0xFFCD5C5C:
    //        return Brushes.IndianRed;
    //        case 0xFF4B0082:
    //        return Brushes.Indigo;
    //        case 0xFFFFFFF0:
    //        return Brushes.Ivory;
    //        case 0xFFF0E68C:
    //        return Brushes.Khaki;
    //        case 0xFFE6E6FA:
    //        return Brushes.Lavender;
    //        case 0xFFFFF0F5:
    //        return Brushes.LavenderBlush;
    //        case 0xFF7CFC00:
    //        return Brushes.LawnGreen;
    //        case 0xFFFFFACD:
    //        return Brushes.LemonChiffon;
    //        case 0xFFADD8E6:
    //        return Brushes.LightBlue;
    //        case 0xFFF08080:
    //        return Brushes.LightCoral;
    //        case 0xFFE0FFFF:
    //        return Brushes.LightCyan;
    //        case 0xFFFAFAD2:
    //        return Brushes.LightGoldenrodYellow;
    //        case 0xFFD3D3D3:
    //        return Brushes.LightGray;
    //        case 0xFF90EE90:
    //        return Brushes.LightGreen;
    //        case 0xFFFFB6C1:
    //        return Brushes.LightPink;
    //        case 0xFFFFA07A:
    //        return Brushes.LightSalmon;
    //        case 0xFF20B2AA:
    //        return Brushes.LightSeaGreen;
    //        case 0xFF87CEFA:
    //        return Brushes.LightSkyBlue;
    //        case 0xFF778899:
    //        return Brushes.LightSlateGray;
    //        case 0xFFB0C4DE:
    //        return Brushes.LightSteelBlue;
    //        case 0xFFFFFFE0:
    //        return Brushes.LightYellow;
    //        case 0xFF00FF00:
    //        return Brushes.Lime;
    //        case 0xFF32CD32:
    //        return Brushes.LimeGreen;
    //        case 0xFFFAF0E6:
    //        return Brushes.Linen;
    //        case 0xFFFF00FF:
    //        return Brushes.Magenta;
    //        case 0xFF800000:
    //        return Brushes.Maroon;
    //        case 0xFF66CDAA:
    //        return Brushes.MediumAquamarine;
    //        case 0xFF0000CD:
    //        return Brushes.MediumBlue;
    //        case 0xFFBA55D3:
    //        return Brushes.MediumOrchid;
    //        case 0xFF9370DB:
    //        return Brushes.MediumPurple;
    //        case 0xFF3CB371:
    //        return Brushes.MediumSeaGreen;
    //        case 0xFF7B68EE:
    //        return Brushes.MediumSlateBlue;
    //        case 0xFF00FA9A:
    //        return Brushes.MediumSpringGreen;
    //        case 0xFF48D1CC:
    //        return Brushes.MediumTurquoise;
    //        case 0xFFC71585:
    //        return Brushes.MediumVioletRed;
    //        case 0xFF191970:
    //        return Brushes.MidnightBlue;
    //        case 0xFFF5FFFA:
    //        return Brushes.MintCream;
    //        case 0xFFFFE4E1:
    //        return Brushes.MistyRose;
    //        case 0xFFFFE4B5:
    //        return Brushes.Moccasin;
    //        case 0xFFFFDEAD:
    //        return Brushes.NavajoWhite;
    //        case 0xFF000080:
    //        return Brushes.Navy;
    //        case 0xFFFDF5E6:
    //        return Brushes.OldLace;
    //        case 0xFF808000:
    //        return Brushes.Olive;
    //        case 0xFF6B8E23:
    //        return Brushes.OliveDrab;
    //        case 0xFFFFA500:
    //        return Brushes.Orange;
    //        case 0xFFFF4500:
    //        return Brushes.OrangeRed;
    //        case 0xFFDA70D6:
    //        return Brushes.Orchid;
    //        case 0xFFEEE8AA:
    //        return Brushes.PaleGoldenrod;
    //        case 0xFF98FB98:
    //        return Brushes.PaleGreen;
    //        case 0xFFAFEEEE:
    //        return Brushes.PaleTurquoise;
    //        case 0xFFDB7093:
    //        return Brushes.PaleVioletRed;
    //        case 0xFFFFEFD5:
    //        return Brushes.PapayaWhip;
    //        case 0xFFFFDAB9:
    //        return Brushes.PeachPuff;
    //        case 0xFFCD853F:
    //        return Brushes.Peru;
    //        case 0xFFFFC0CB:
    //        return Brushes.Pink;
    //        case 0xFFDDA0DD:
    //        return Brushes.Plum;
    //        case 0xFFB0E0E6:
    //        return Brushes.PowderBlue;
    //        case 0xFF800080:
    //        return Brushes.Purple;
    //        case 0xFFFF0000:
    //        return Brushes.Red;
    //        case 0xFFBC8F8F:
    //        return Brushes.RosyBrown;
    //        case 0xFF4169E1:
    //        return Brushes.RoyalBlue;
    //        case 0xFF8B4513:
    //        return Brushes.SaddleBrown;
    //        case 0xFFFA8072:
    //        return Brushes.Salmon;
    //        case 0xFFF4A460:
    //        return Brushes.SandyBrown;
    //        case 0xFF2E8B57:
    //        return Brushes.SeaGreen;
    //        case 0xFFFFF5EE:
    //        return Brushes.SeaShell;
    //        case 0xFFA0522D:
    //        return Brushes.Sienna;
    //        case 0xFFC0C0C0:
    //        return Brushes.Silver;
    //        case 0xFF87CEEB:
    //        return Brushes.SkyBlue;
    //        case 0xFF6A5ACD:
    //        return Brushes.SlateBlue;
    //        case 0xFF708090:
    //        return Brushes.SlateGray;
    //        case 0xFFFFFAFA:
    //        return Brushes.Snow;
    //        case 0xFF00FF7F:
    //        return Brushes.SpringGreen;
    //        case 0xFF4682B4:
    //        return Brushes.SteelBlue;
    //        case 0xFFD2B48C:
    //        return Brushes.Tan;
    //        case 0xFF008080:
    //        return Brushes.Teal;
    //        case 0xFFD8BFD8:
    //        return Brushes.Thistle;
    //        case 0xFFFF6347:
    //        return Brushes.Tomato;
    //        case 0xFF40E0D0:
    //        return Brushes.Turquoise;
    //        case 0xFFEE82EE:
    //        return Brushes.Violet;
    //        case 0xFFF5DEB3:
    //        return Brushes.Wheat;
    //        case 0xFFFFFFFF:
    //        return Brushes.White;
    //        case 0xFFF5F5F5:
    //        return Brushes.WhiteSmoke;
    //        case 0xFFFFFF00:
    //        return Brushes.Yellow;
    //        case 0xFF9ACD32:
    //        return Brushes.YellowGreen;
    //        default:
    //        // Fallback for custom or unknown ARGB colors
    //        return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
    //    }
    //}

    public static SolidColorBrush FromCtraderColor(NinjaTrader.NinjaScript.Strategies.Color ctraderColor)
    {
        return new SolidColorBrush(
            System.Windows.Media.Color.FromArgb(
                ctraderColor.A,
                ctraderColor.R,
                ctraderColor.G,
                ctraderColor.B
            ));
    }

    public static NinjaTrader.NinjaScript.Strategies.Color ToCtraderColor(Brush ninjaBrush)
    {
        if (ninjaBrush is SolidColorBrush solidColorBrush)
        {
            var mediaColor = solidColorBrush.Color;
            return NinjaTrader.NinjaScript.Strategies.Color.FromArgb(
                mediaColor.A,
                mediaColor.R,
                mediaColor.G,
                mediaColor.B
            );
        }

        // Fallback: return opaque black if it's not a SolidColorBrush
        return NinjaTrader.NinjaScript.Strategies.Color.Black;
    }
}

