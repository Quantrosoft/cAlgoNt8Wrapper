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

namespace NinjaTrader.NinjaScript.Strategies
{
    //     Represents the Text chart object. Allows place the text anywhere on the chart,
    //     bound to the chart.
    public class ChartText : ChartObject
    {
        //
        // Summary:
        //     Gets or sets the Time line value.
        public DateTime Time { get; set; }

        //
        // Summary:
        //     Gets or sets the Y-axis value.
        public double Y { get; set; }

        //
        // Summary:
        //     Gets or sets the text color.
        public Color Color { get; set; }

        //
        // Summary:
        //     Gets or sets the text content.
        public string Text { get; set; }

        //
        // Summary:
        //     Gets or sets the font size of text
        public double FontSize { get; set; }

        //
        // Summary:
        //     Gets or sets if the text is Bold or not
        public bool IsBold { get; set; }

        //
        // Summary:
        //     Gets or sets if the text is Italic or not
        public bool IsItalic { get; set; }

        //
        // Summary:
        //     Gets or sets if the text is Underlined or not
        public bool IsUnderlined { get; set; }

        //
        // Summary:
        //     Gets or sets the vertical alignment of the text regarding the anchor point.
        public VerticalAlignment VerticalAlignment { get; set; }

        //
        // Summary:
        //     Gets or sets the horizontal alignment of the text regarding the anchor point.
        public HorizontalAlignment HorizontalAlignment { get; set; }

        //
        // Summary:
        //     Gets or sets the font style of the text.
        public string FontFamily { get; set; }
    }
}
