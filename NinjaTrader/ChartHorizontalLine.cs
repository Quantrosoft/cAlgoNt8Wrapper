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

namespace cAlgoNt8Wrapper
{
    //     Represents the Horizontal Line chart object. Used to mark a certain value on
    //     the Y-axis throughout the whole chart.
    public class ChartHorizontalLine : ChartObject
    {
        //
        // Summary:
        //     Gets or sets the Y-axis value of the line location.
        public double Y { get; set; }

        //
        // Summary:
        //     Gets or sets the line thickness.
        public int Thickness { get; set; }

        //
        // Summary:
        //     Gets or sets the line color.
        public Color Color { get; set; }

        //
        // Summary:
        //     Gets or sets the line style.
        public LineStyle LineStyle { get; set; }
    }
}
