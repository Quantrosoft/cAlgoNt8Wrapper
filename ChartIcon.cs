//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

// cTrader API Emulator
// Type: cAlgo.ChartIcon
using System;
using cAlgo;

namespace cAlgo.API
{
    /// <summary>Represents the Icon chart object.</summary>
    public class ChartIcon : ChartObject
    {
        /// <summary>Gets or sets the type of the icon.</summary>
        public ChartIconType IconType { get; set; }

        /// <summary>Gets or sets the Time value of the icon location.</summary>
        public DateTime Time { get; set; }

        /// <summary>Gets or sets the Y-axis value of the icon location.</summary>
        public double Y { get; set; }

        /// <summary>Gets or sets the color of the icon.</summary>
        public Color Color { get; set; }
    }
}
