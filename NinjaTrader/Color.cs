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

using System;
using System.Globalization;

namespace cAlgoNt8Wrapper
{
    //     Represents an ARGB (alpha, red, green, blue) color.
    public sealed class Color : IEquatable<Color>
    {
        private enum SystemColor
        {
            Transparent,
            AliceBlue,
            AntiqueWhite,
            Aqua,
            Aquamarine,
            Azure,
            Beige,
            Bisque,
            Black,
            BlanchedAlmond,
            Blue,
            BlueViolet,
            Brown,
            BurlyWood,
            CadetBlue,
            Chartreuse,
            Chocolate,
            Coral,
            CornflowerBlue,
            Cornsilk,
            Crimson,
            Cyan,
            DarkBlue,
            DarkCyan,
            DarkGoldenrod,
            DarkGray,
            DarkGreen,
            DarkKhaki,
            DarkMagenta,
            DarkOliveGreen,
            DarkOrange,
            DarkOrchid,
            DarkRed,
            DarkSalmon,
            DarkSeaGreen,
            DarkSlateBlue,
            DarkSlateGray,
            DarkTurquoise,
            DarkViolet,
            DeepPink,
            DeepSkyBlue,
            DimGray,
            DodgerBlue,
            Firebrick,
            FloralWhite,
            ForestGreen,
            Fuchsia,
            Gainsboro,
            GhostWhite,
            Gold,
            Goldenrod,
            Gray,
            Green,
            GreenYellow,
            Honeydew,
            HotPink,
            IndianRed,
            Indigo,
            Ivory,
            Khaki,
            Lavender,
            LavenderBlush,
            LawnGreen,
            LemonChiffon,
            LightBlue,
            LightCoral,
            LightCyan,
            LightGoldenrodYellow,
            LightGray,
            LightGreen,
            LightPink,
            LightSalmon,
            LightSeaGreen,
            LightSkyBlue,
            LightSlateGray,
            LightSteelBlue,
            LightYellow,
            Lime,
            LimeGreen,
            Linen,
            Magenta,
            Maroon,
            MediumAquamarine,
            MediumBlue,
            MediumOrchid,
            MediumPurple,
            MediumSeaGreen,
            MediumSlateBlue,
            MediumSpringGreen,
            MediumTurquoise,
            MediumVioletRed,
            MidnightBlue,
            MintCream,
            MistyRose,
            Moccasin,
            NavajoWhite,
            Navy,
            OldLace,
            Olive,
            OliveDrab,
            Orange,
            OrangeRed,
            Orchid,
            PaleGoldenrod,
            PaleGreen,
            PaleTurquoise,
            PaleVioletRed,
            PapayaWhip,
            PeachPuff,
            Peru,
            Pink,
            Plum,
            PowderBlue,
            Purple,
            Red,
            RosyBrown,
            RoyalBlue,
            SaddleBrown,
            Salmon,
            SandyBrown,
            SeaGreen,
            SeaShell,
            Sienna,
            Silver,
            SkyBlue,
            SlateBlue,
            SlateGray,
            Snow,
            SpringGreen,
            SteelBlue,
            Tan,
            Teal,
            Thistle,
            Tomato,
            Turquoise,
            Violet,
            Wheat,
            White,
            WhiteSmoke,
            Yellow,
            YellowGreen
        }

        private const int ArgbAShift = 24;

        private const int ArgbRShift = 16;

        private const int ArgbGShift = 8;

        private const int ArgbBShift = 0;

        private const char HexFormatPrefix = '#';

        private readonly uint _value;

        private static readonly Color[] _systemColorMap;

        //
        // Summary:
        //     Represents empty color.
        public static readonly Color Empty;

        //
        // Summary:
        //     Gets the alpha component value of the color.
        public byte A => (byte)(_value >> 24);

        //
        // Summary:
        //     Gets the red component value of the color.
        public byte R => (byte)(_value >> 16);

        //
        // Summary:
        //     Gets the green component value of the color.
        public byte G => (byte)(_value >> 8);

        //
        // Summary:
        //     Gets the blue component value of the color.
        public byte B => (byte)_value;

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #00FFFFFF.
        public static Color Transparent { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF0F8FF.
        public static Color AliceBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFAEBD7.
        public static Color AntiqueWhite { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00FFFF.
        public static Color Aqua { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF7FFFD4.
        public static Color Aquamarine { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF0FFFF.
        public static Color Azure { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF5F5DC.
        public static Color Beige { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFE4C4.
        public static Color Bisque { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF000000.
        public static Color Black { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFEBCD.
        public static Color BlanchedAlmond { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF0000FF.
        public static Color Blue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF8A2BE2.
        public static Color BlueViolet { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFA52A2A.
        public static Color Brown { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDEB887.
        public static Color BurlyWood { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF5F9EA0.
        public static Color CadetBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF7FFF00.
        public static Color Chartreuse { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFD2691E.
        public static Color Chocolate { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF7F50.
        public static Color Coral { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF6495ED.
        public static Color CornflowerBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFF8DC.
        public static Color Cornsilk { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDC143C.
        public static Color Crimson { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00FFFF.
        public static Color Cyan { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00008B.
        public static Color DarkBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF008B8B.
        public static Color DarkCyan { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFB8860B.
        public static Color DarkGoldenrod { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFA9A9A9.
        public static Color DarkGray { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF006400.
        public static Color DarkGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFBDB76B.
        public static Color DarkKhaki { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF8B008B.
        public static Color DarkMagenta { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF556B2F.
        public static Color DarkOliveGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF8C00.
        public static Color DarkOrange { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF9932CC.
        public static Color DarkOrchid { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF8B0000.
        public static Color DarkRed { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFE9967A.
        public static Color DarkSalmon { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF8FBC8F.
        public static Color DarkSeaGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF483D8B.
        public static Color DarkSlateBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF2F4F4F.
        public static Color DarkSlateGray { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00CED1.
        public static Color DarkTurquoise { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF9400D3.
        public static Color DarkViolet { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF1493.
        public static Color DeepPink { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00BFFF.
        public static Color DeepSkyBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF696969.
        public static Color DimGray { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF1E90FF.
        public static Color DodgerBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFB22222.
        public static Color Firebrick { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFAF0.
        public static Color FloralWhite { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF228B22.
        public static Color ForestGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF00FF.
        public static Color Fuchsia { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDCDCDC.
        public static Color Gainsboro { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF8F8FF.
        public static Color GhostWhite { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFD700.
        public static Color Gold { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDAA520.
        public static Color Goldenrod { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF808080.
        public static Color Gray { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF008000.
        public static Color Green { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFADFF2F.
        public static Color GreenYellow { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF0FFF0.
        public static Color Honeydew { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF69B4.
        public static Color HotPink { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFCD5C5C.
        public static Color IndianRed { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF4B0082.
        public static Color Indigo { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFFF0.
        public static Color Ivory { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF0E68C.
        public static Color Khaki { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFE6E6FA.
        public static Color Lavender { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFF0F5.
        public static Color LavenderBlush { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF7CFC00.
        public static Color LawnGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFACD.
        public static Color LemonChiffon { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFADD8E6.
        public static Color LightBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF08080.
        public static Color LightCoral { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFE0FFFF.
        public static Color LightCyan { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFAFAD2.
        public static Color LightGoldenrodYellow { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFD3D3D3.
        public static Color LightGray { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF90EE90.
        public static Color LightGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFB6C1.
        public static Color LightPink { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFA07A.
        public static Color LightSalmon { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF20B2AA.
        public static Color LightSeaGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF87CEFA.
        public static Color LightSkyBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF778899.
        public static Color LightSlateGray { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFB0C4DE.
        public static Color LightSteelBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFFE0.
        public static Color LightYellow { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00FF00.
        public static Color Lime { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF32CD32.
        public static Color LimeGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFAF0E6.
        public static Color Linen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF00FF.
        public static Color Magenta { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF800000.
        public static Color Maroon { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF66CDAA.
        public static Color MediumAquamarine { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF0000CD.
        public static Color MediumBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFBA55D3.
        public static Color MediumOrchid { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF9370DB.
        public static Color MediumPurple { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF3CB371.
        public static Color MediumSeaGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF7B68EE.
        public static Color MediumSlateBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00FA9A.
        public static Color MediumSpringGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF48D1CC.
        public static Color MediumTurquoise { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFC71585
        public static Color MediumVioletRed { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF191970.
        public static Color MidnightBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF5FFFA.
        public static Color MintCream { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFE4E1.
        public static Color MistyRose { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFE4B5.
        public static Color Moccasin { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFDEAD.
        public static Color NavajoWhite { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF000080.
        public static Color Navy { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFDF5E6.
        public static Color OldLace { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF808000.
        public static Color Olive { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF6B8E23.
        public static Color OliveDrab { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFA500.
        public static Color Orange { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF4500.
        public static Color OrangeRed { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDA70D6.
        public static Color Orchid { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFEEE8AA.
        public static Color PaleGoldenrod { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF98FB98.
        public static Color PaleGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFAFEEEE.
        public static Color PaleTurquoise { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDB7093.
        public static Color PaleVioletRed { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFEFD5.
        public static Color PapayaWhip { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFDAB9.
        public static Color PeachPuff { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFCD853F.
        public static Color Peru { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFC0CB.
        public static Color Pink { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFDDA0DD.
        public static Color Plum { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFB0E0E6.
        public static Color PowderBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF800080.
        public static Color Purple { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF0000.
        public static Color Red { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFBC8F8F.
        public static Color RosyBrown { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF4169E1.
        public static Color RoyalBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF8B4513.
        public static Color SaddleBrown { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFA8072.
        public static Color Salmon { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF4A460.
        public static Color SandyBrown { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF2E8B57.
        public static Color SeaGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFF5EE.
        public static Color SeaShell { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFA0522D.
        public static Color Sienna { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFC0C0C0.
        public static Color Silver { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF87CEEB.
        public static Color SkyBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF6A5ACD.
        public static Color SlateBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF708090.
        public static Color SlateGray { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFAFA.
        public static Color Snow { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF00FF7F.
        public static Color SpringGreen { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF4682B4.
        public static Color SteelBlue { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFD2B48C.
        public static Color Tan { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF008080.
        public static Color Teal { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFD8BFD8.
        public static Color Thistle { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFF6347.
        public static Color Tomato { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF40E0D0.
        public static Color Turquoise { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFEE82EE.
        public static Color Violet { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF5DEB3.
        public static Color Wheat { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFFFF.
        public static Color White { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFF5F5F5.
        public static Color WhiteSmoke { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FFFFFF00.
        public static Color Yellow { get; }

        //
        // Summary:
        //     Gets a system-defined color that has an ARGB value of #FF9ACD32.
        public static Color YellowGreen { get; }

        private Color(uint value)
        {
            _value = value;
        }

        //
        // Summary:
        //     Get the 32-bit ARGB color value.
        public uint ToArgb()
        {
            return _value;
        }

        //
        // Summary:
        //     Get the hex string representation of the color.
        //
        // Returns:
        //     The hex string representation of the color.
        public string ToHexString()
        {
            return $"{35}{_value:X8}";
        }

        //
        // Summary:
        //     Returns a System.String that represents this instance.
        //
        // Returns:
        //     A System.String that represents this instance.
        public override string ToString()
        {
            return $"Color [A={A}, R={R}, G={G}, B={B}]";
        }

        //
        // Summary:
        //     Defines whether the specified object is equal to this instance.
        //
        // Parameters:
        //   other:
        //     The object to compare with the current object.
        //
        // Returns:
        //     true if the specified System.Object is equal to this instance; otherwise, false.
        public bool Equals(Color other)
        {
            if (other != null)
            {
                return _value == other._value;
            }

            return false;
        }

        //
        // Summary:
        //     Defines whether the specified object is equal to this instance.
        //
        // Parameters:
        //   obj:
        //     The object to compare with the current object.
        //
        // Returns:
        //     true if the specified System.Object is equal to this instance; otherwise, false.
        public override bool Equals(object obj)
        {
            if (obj is Color other)
            {
                return Equals(other);
            }

            return false;
        }

        //
        // Summary:
        //     Returns the hash code for this instance.
        public override int GetHashCode()
        {
            return _value.GetHashCode() ^ 0x11;
        }

        //
        // Summary:
        //     Creates a color from alpha, red, green and blue components.
        //
        // Parameters:
        //   alpha:
        //     Alpha value from 0 to 255
        //
        //   red:
        //     Red value from 0 to 255
        //
        //   green:
        //     Green value from 0 to 255
        //
        //   blue:
        //     Blue value from 0 to 255
        //
        // Returns:
        //     The Color for specified parameters.
        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            return new Color(MakeArgb(ToByte(alpha), ToByte(red), ToByte(green), ToByte(blue)));
        }

        //
        // Summary:
        //     Creates a color from existing color, but with new specified alpha value.
        //
        // Parameters:
        //   alpha:
        //     New alpha value from 0 to 255
        //
        //   baseColor:
        //     Base color from which red, green and blue values will be copied to a new color
        //
        //
        // Returns:
        //     The Color for specified parameters.
        public static Color FromArgb(int alpha, Color baseColor)
        {
            return new Color(MakeArgb(ToByte(alpha), baseColor.R, baseColor.G, baseColor.B));
        }

        //
        // Summary:
        //     Creates a color from a 32-bit ARGB value.
        //
        // Parameters:
        //   argb:
        //     Color ARGB 32-bit integer value
        //
        // Returns:
        //     The Color for specified parameter.
        public static Color FromArgb(int argb)
        {
            return new Color((uint)argb);
        }

        //
        // Summary:
        //     Creates a color from red, green and blue values. The alpha value is implicitly
        //     255 (fully opaque).
        //
        // Parameters:
        //   red:
        //     Red value from 0 to 255
        //
        //   green:
        //     Green value from 0 to 255
        //
        //   blue:
        //     Blue value from 0 to 255
        //
        // Returns:
        //     The Color for specified parameters.
        public static Color FromArgb(int red, int green, int blue)
        {
            return FromArgb(255, red, green, blue);
        }

        //
        // Summary:
        //     Attempts to convert a hex string to a Color.
        //
        // Parameters:
        //   hex:
        //     Hex string to convert to a Color
        //
        // Returns:
        //     A Color that represents the converted hex string.
        public static Color FromHex(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return Empty;
            }

            hex = hex.TrimStart('#');
            if (uint.TryParse(hex, NumberStyles.HexNumber, null, out var result))
            {
                if (hex.Length == 6)
                {
                    return FromArgb((byte)(result >> 16), (byte)(result >> 8), (byte)result);
                }

                if (hex.Length == 8)
                {
                    return FromArgb((byte)(result >> 24), (byte)(result >> 16), (byte)(result >> 8), (byte)result);
                }
            }

            return Empty;
        }

        //
        // Summary:
        //     Creates a color from the specified name of a predefined color.
        //
        // Parameters:
        //   name:
        //     Name of predefined color
        //
        // Returns:
        //     A Color for specified name or color with value #00000000 if the name was not
        //     found.
        public static Color FromName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Empty;
            }

            if (Enum.TryParse<SystemColor>(name, ignoreCase: true, out var result) && Enum.IsDefined(typeof(SystemColor), result))
            {
                return _systemColorMap[(int)result];
            }

            return Empty;
        }

        private static byte ToByte(int value)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value > 255)
            {
                return byte.MaxValue;
            }

            return checked((byte)value);
        }

        private static uint MakeArgb(byte a, byte r, byte g, byte b)
        {
            return (uint)(b | (g << 8) | (r << 16) | (a << 24));
        }

        public static bool operator ==(Color left, Color right)
        {
            if ((object)left == null && (object)right == null)
            {
                return true;
            }

            if ((object)left == null || (object)right == null)
            {
                return false;
            }

            return left._value == right._value;
        }

        public static bool operator !=(Color left, Color right)
        {
            return !(left == right);
        }

        public static implicit operator Color(int argb)
        {
            return FromArgb(argb);
        }

        public static implicit operator Color(string value)
        {
            Color color = FromName(value);
            if (color == Empty)
            {
                color = FromHex(value);
            }

            return color;
        }

        static Color()
        {
            _systemColorMap = new Color[141];
            Empty = new Color(0u);
            _systemColorMap[0] = (Transparent = new Color(16777215u));
            _systemColorMap[1] = (AliceBlue = new Color(4293982463u));
            _systemColorMap[2] = (AntiqueWhite = new Color(4294634455u));
            _systemColorMap[3] = (Aqua = new Color(4278255615u));
            _systemColorMap[4] = (Aquamarine = new Color(4286578644u));
            _systemColorMap[5] = (Azure = new Color(4293984255u));
            _systemColorMap[6] = (Beige = new Color(4294309340u));
            _systemColorMap[7] = (Bisque = new Color(4294960324u));
            _systemColorMap[8] = (Black = new Color(4278190080u));
            _systemColorMap[9] = (BlanchedAlmond = new Color(4294962125u));
            _systemColorMap[10] = (Blue = new Color(4278190335u));
            _systemColorMap[11] = (BlueViolet = new Color(4287245282u));
            _systemColorMap[12] = (Brown = new Color(4289014314u));
            _systemColorMap[13] = (BurlyWood = new Color(4292786311u));
            _systemColorMap[14] = (CadetBlue = new Color(4284456608u));
            _systemColorMap[15] = (Chartreuse = new Color(4286578432u));
            _systemColorMap[16] = (Chocolate = new Color(4291979550u));
            _systemColorMap[17] = (Coral = new Color(4294934352u));
            _systemColorMap[18] = (CornflowerBlue = new Color(4284782061u));
            _systemColorMap[19] = (Cornsilk = new Color(4294965468u));
            _systemColorMap[20] = (Crimson = new Color(4292613180u));
            _systemColorMap[21] = (Cyan = new Color(4278255615u));
            _systemColorMap[22] = (DarkBlue = new Color(4278190219u));
            _systemColorMap[23] = (DarkCyan = new Color(4278225803u));
            _systemColorMap[24] = (DarkGoldenrod = new Color(4290283019u));
            _systemColorMap[25] = (DarkGray = new Color(4289309097u));
            _systemColorMap[26] = (DarkGreen = new Color(4278215680u));
            _systemColorMap[27] = (DarkKhaki = new Color(4290623339u));
            _systemColorMap[28] = (DarkMagenta = new Color(4287299723u));
            _systemColorMap[29] = (DarkOliveGreen = new Color(4283788079u));
            _systemColorMap[30] = (DarkOrange = new Color(4294937600u));
            _systemColorMap[31] = (DarkOrchid = new Color(4288230092u));
            _systemColorMap[32] = (DarkRed = new Color(4287299584u));
            _systemColorMap[33] = (DarkSalmon = new Color(4293498490u));
            _systemColorMap[34] = (DarkSeaGreen = new Color(4287609999u));
            _systemColorMap[35] = (DarkSlateBlue = new Color(4282924427u));
            _systemColorMap[36] = (DarkSlateGray = new Color(4281290575u));
            _systemColorMap[37] = (DarkTurquoise = new Color(4278243025u));
            _systemColorMap[38] = (DarkViolet = new Color(4287889619u));
            _systemColorMap[39] = (DeepPink = new Color(4294907027u));
            _systemColorMap[40] = (DeepSkyBlue = new Color(4278239231u));
            _systemColorMap[41] = (DimGray = new Color(4285098345u));
            _systemColorMap[42] = (DodgerBlue = new Color(4280193279u));
            _systemColorMap[43] = (Firebrick = new Color(4289864226u));
            _systemColorMap[44] = (FloralWhite = new Color(4294966000u));
            _systemColorMap[45] = (ForestGreen = new Color(4280453922u));
            _systemColorMap[46] = (Fuchsia = new Color(4294902015u));
            _systemColorMap[47] = (Gainsboro = new Color(4292664540u));
            _systemColorMap[48] = (GhostWhite = new Color(4294506751u));
            _systemColorMap[49] = (Gold = new Color(4294956800u));
            _systemColorMap[50] = (Goldenrod = new Color(4292519200u));
            _systemColorMap[51] = (Gray = new Color(4286611584u));
            _systemColorMap[52] = (Green = new Color(4278222848u));
            _systemColorMap[53] = (GreenYellow = new Color(4289593135u));
            _systemColorMap[54] = (Honeydew = new Color(4293984240u));
            _systemColorMap[55] = (HotPink = new Color(4294928820u));
            _systemColorMap[56] = (IndianRed = new Color(4291648604u));
            _systemColorMap[57] = (Indigo = new Color(4283105410u));
            _systemColorMap[58] = (Ivory = new Color(4294967280u));
            _systemColorMap[59] = (Khaki = new Color(4293977740u));
            _systemColorMap[60] = (Lavender = new Color(4293322490u));
            _systemColorMap[61] = (LavenderBlush = new Color(4294963445u));
            _systemColorMap[62] = (LawnGreen = new Color(4286381056u));
            _systemColorMap[63] = (LemonChiffon = new Color(4294965965u));
            _systemColorMap[64] = (LightBlue = new Color(4289583334u));
            _systemColorMap[65] = (LightCoral = new Color(4293951616u));
            _systemColorMap[66] = (LightCyan = new Color(4292935679u));
            _systemColorMap[67] = (LightGoldenrodYellow = new Color(4294638290u));
            _systemColorMap[68] = (LightGray = new Color(4292072403u));
            _systemColorMap[69] = (LightGreen = new Color(4287688336u));
            _systemColorMap[70] = (LightPink = new Color(4294948545u));
            _systemColorMap[71] = (LightSalmon = new Color(4294942842u));
            _systemColorMap[72] = (LightSeaGreen = new Color(4280332970u));
            _systemColorMap[73] = (LightSkyBlue = new Color(4287090426u));
            _systemColorMap[74] = (LightSlateGray = new Color(4286023833u));
            _systemColorMap[75] = (LightSteelBlue = new Color(4289774814u));
            _systemColorMap[76] = (LightYellow = new Color(4294967264u));
            _systemColorMap[77] = (Lime = new Color(4278255360u));
            _systemColorMap[78] = (LimeGreen = new Color(4281519410u));
            _systemColorMap[79] = (Linen = new Color(4294635750u));
            _systemColorMap[80] = (Magenta = new Color(4294902015u));
            _systemColorMap[81] = (Maroon = new Color(4286578688u));
            _systemColorMap[82] = (MediumAquamarine = new Color(4284927402u));
            _systemColorMap[83] = (MediumBlue = new Color(4278190285u));
            _systemColorMap[84] = (MediumOrchid = new Color(4290401747u));
            _systemColorMap[85] = (MediumPurple = new Color(4287852763u));
            _systemColorMap[86] = (MediumSeaGreen = new Color(4282168177u));
            _systemColorMap[87] = (MediumSlateBlue = new Color(4286277870u));
            _systemColorMap[88] = (MediumSpringGreen = new Color(4278254234u));
            _systemColorMap[89] = (MediumTurquoise = new Color(4282962380u));
            _systemColorMap[90] = (MediumVioletRed = new Color(4291237253u));
            _systemColorMap[91] = (MidnightBlue = new Color(4279834992u));
            _systemColorMap[92] = (MintCream = new Color(4294311930u));
            _systemColorMap[93] = (MistyRose = new Color(4294960353u));
            _systemColorMap[94] = (Moccasin = new Color(4294960309u));
            _systemColorMap[95] = (NavajoWhite = new Color(4294958765u));
            _systemColorMap[96] = (Navy = new Color(4278190208u));
            _systemColorMap[97] = (OldLace = new Color(4294833638u));
            _systemColorMap[98] = (Olive = new Color(4286611456u));
            _systemColorMap[99] = (OliveDrab = new Color(4285238819u));
            _systemColorMap[100] = (Orange = new Color(4294944000u));
            _systemColorMap[101] = (OrangeRed = new Color(4294919424u));
            _systemColorMap[102] = (Orchid = new Color(4292505814u));
            _systemColorMap[103] = (PaleGoldenrod = new Color(4293847210u));
            _systemColorMap[104] = (PaleGreen = new Color(4288215960u));
            _systemColorMap[105] = (PaleTurquoise = new Color(4289720046u));
            _systemColorMap[106] = (PaleVioletRed = new Color(4292571283u));
            _systemColorMap[107] = (PapayaWhip = new Color(4294963157u));
            _systemColorMap[108] = (PeachPuff = new Color(4294957753u));
            _systemColorMap[109] = (Peru = new Color(4291659071u));
            _systemColorMap[110] = (Pink = new Color(4294951115u));
            _systemColorMap[111] = (Plum = new Color(4292714717u));
            _systemColorMap[112] = (PowderBlue = new Color(4289781990u));
            _systemColorMap[113] = (Purple = new Color(4286578816u));
            _systemColorMap[114] = (Red = new Color(4294901760u));
            _systemColorMap[115] = (RosyBrown = new Color(4290547599u));
            _systemColorMap[116] = (RoyalBlue = new Color(4282477025u));
            _systemColorMap[117] = (SaddleBrown = new Color(4287317267u));
            _systemColorMap[118] = (Salmon = new Color(4294606962u));
            _systemColorMap[119] = (SandyBrown = new Color(4294222944u));
            _systemColorMap[120] = (SeaGreen = new Color(4281240407u));
            _systemColorMap[121] = (SeaShell = new Color(4294964718u));
            _systemColorMap[122] = (Sienna = new Color(4288696877u));
            _systemColorMap[123] = (Silver = new Color(4290822336u));
            _systemColorMap[124] = (SkyBlue = new Color(4287090411u));
            _systemColorMap[125] = (SlateBlue = new Color(4285160141u));
            _systemColorMap[126] = (SlateGray = new Color(4285563024u));
            _systemColorMap[127] = (Snow = new Color(4294966010u));
            _systemColorMap[128] = (SpringGreen = new Color(4278255487u));
            _systemColorMap[129] = (SteelBlue = new Color(4282811060u));
            _systemColorMap[130] = (Tan = new Color(4291998860u));
            _systemColorMap[131] = (Teal = new Color(4278222976u));
            _systemColorMap[132] = (Thistle = new Color(4292394968u));
            _systemColorMap[133] = (Tomato = new Color(4294927175u));
            _systemColorMap[134] = (Turquoise = new Color(4282441936u));
            _systemColorMap[135] = (Violet = new Color(4293821166u));
            _systemColorMap[136] = (Wheat = new Color(4294303411u));
            _systemColorMap[137] = (White = new Color(uint.MaxValue));
            _systemColorMap[138] = (WhiteSmoke = new Color(4294309365u));
            _systemColorMap[139] = (Yellow = new Color(4294967040u));
            _systemColorMap[140] = (YellowGreen = new Color(4288335154u));
        }
    }
}
