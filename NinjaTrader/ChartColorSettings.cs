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

// cTrader API Emulator
// Type: cAlgo.ChartColorSettings
namespace NinjaTrader.NinjaScript.Strategies
{
    /// <summary>Represents the charts NinjaTrader.API.Color Settings.</summary>
    /// <remarks>
    /// Use the NinjaTrader.API.Color classes to set the chart NinjaTrader.API.Color Settings.
    /// </remarks>
    public class ChartColorSettings
    {
        /// <summary>Gets or sets the color of the chart background.</summary>
        public Color BackgroundColor { get; set; }

        /// <summary>Gets or sets the color of the chart foreground.</summary>
        public Color ForegroundColor { get; set; }

        /// <summary>Gets or sets the color of the grid lines.</summary>
        public Color GridLinesColor { get; set; }

        /// <summary>Gets or sets the color of the period separator.</summary>
        public Color PeriodSeparatorColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the bull candle or bar outline.
        /// </summary>
        public Color BullOutlineColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the bear candle or bar outline.
        /// </summary>
        public Color BearOutlineColor { get; set; }

        /// <summary>Gets or sets the color of the bull candle fill.</summary>
        public Color BullFillColor { get; set; }

        /// <summary>Gets or sets the color of the bear candle fill.</summary>
        public Color BearFillColor { get; set; }

        /// <summary>Gets or sets the color of the tick volume.</summary>
        public Color TickVolumeColor { get; set; }

        /// <summary>Gets or sets the color of the winning deal.</summary>
        public Color WinningDealColor { get; set; }

        /// <summary>Gets or sets the color of the losing deal.</summary>
        public Color LosingDealColor { get; set; }

        /// <summary>Gets or sets the color of the ask price line.</summary>
        public Color AskPriceLineColor { get; set; }

        /// <summary>Gets or sets the color of the bid price line.</summary>
        public Color BidPriceLineColor { get; set; }

        /// <summary>Gets or sets the color of Buy positions and orders.</summary>
        public Color BuyColor { get; set; }

        /// <summary>
        /// Gets or sets the color of Sell order positions and orders.
        /// </summary>
        public Color SellColor { get; set; }
    }
}
