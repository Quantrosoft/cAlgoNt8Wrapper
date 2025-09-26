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

namespace cAlgoNt8Wrapper
{
    //     Represents the Trend Line chart object. A straight line that can be drawn from
    //     point 1 to the point 2 in any direction to mark the trends on the chart.
    public class ChartTrendLine : ChartObject
    {
        //
        // Summary:
        //     Gets or sets the value 1 on the Time line.
        public DateTime Time1 { get; set; }

        //
        // Summary:
        //     Gets or sets the value 2 on the Time line.
        public DateTime Time2 { get; set; }

        //
        // Summary:
        //     Gets or sets the value 1 on the Y-axis.
        public double Y1 { get; set; }

        //
        // Summary:
        //     Gets or sets the value 2 on the Y-axis.
        public double Y2 { get; set; }

        //
        // Summary:
        //     Gets or sets the color of the Trend Line.
        public Color Color { get; set; }

        //
        // Summary:
        //     Gets or sets the thickness of the Trend Line.
        public int Thickness { get; set; }

        //
        // Summary:
        //     Gets or sets the Trend Line style.
        public LineStyle LineStyle { get; set; }

        //
        // Summary:
        //     Defines the trend line angle.
        public bool ShowAngle { get; set; }

        //
        // Summary:
        //     Defines if the Trend Line extends to infinity.
        bool ExtendToInfinity { get; set; }

        //
        // Summary:
        //     Calculates Y-axis value corresponding the specified bar index.
        //
        // Parameters:
        //   barIndex:
        //     Index of the bar.
        //
        // Returns:
        //     System.Double.
        //double CalculateY(int barIndex);

        //
        // Summary:
        //     Calculates Y-axis value corresponding the specified time value.
        //
        // Parameters:
        //   time:
        //     The time.
        //
        // Returns:
        //     System.Double.
        //double CalculateY(DateTime time);
    }
}
