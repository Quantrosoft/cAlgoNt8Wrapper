//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

namespace cAlgo.API
{
    //     Represents the chart static text.
    public class ChartStaticText : ChartObject
    {
        //
        // Summary:
        //     Gets or sets the chart static text color.
        public Color Color { get; set; }

        //
        // Summary:
        //     Gets or sets the chart static text content.
        public string Text { get; set; }

        //
        // Summary:
        //     Gets or sets the chart static text vertical alignment.
        public VerticalAlignment VerticalAlignment { get; set; }

        //
        // Summary:
        //     Gets or sets the chart static text horizontal alignment.
        public HorizontalAlignment HorizontalAlignment { get; set; }
    }
}
