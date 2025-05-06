//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using NinjaTrader.Gui.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAlgo.API
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
