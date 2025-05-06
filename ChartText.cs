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
