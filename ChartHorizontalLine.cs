//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using cAlgo.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAlgo.API
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
