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

using NinjaTrader.NinjaScript.Strategies;
using RobotLib;
using System;

namespace NinjaTrader.NinjaScript.Strategies
{
    //     Represents the Chart Interface.
    public class Chart : ChartArea
    {
        public Chart(Strategy robot)
        {
            mRobot = robot;
            mStrategy = robot;
            mTextFont = new NinjaTrader.Gui.Tools.SimpleFont("Arial", 14);
            ColorSettings = new ChartColorSettings()
            {
                ForegroundColor = Color.White
            };
        }

        //
        // Summary:
        //     Gets the unique Id of Chart.
        Guid Id { get; }
#if false
        //
        // Summary:
        //     Gets the read only list of the indicator areas.
        IReadonlyList<IndicatorArea> IndicatorAreas { get; }

        //
        // Summary:
        //     Gets the chart display settings.
        ChartDisplaySettings DisplaySettings { get; }
#endif
        //
        // Summary:
        //     Gets the chart color settings.
        public ChartColorSettings ColorSettings { get; internal set; }

        //
        // Summary:
        //     Gets or sets the type of the chart - Bar, Candlesticks, Line or Dots chart.
        //ChartType ChartType { get; set; }

        //
        // Summary:
        //     Gets or sets the zoom percent values. Valid values are from 5 to 500 with a step
        //     of 5, as can be seen on UI in the charts Zoom control.
        public int ZoomLevel { get; set; }

        //
        // Summary:
        //     Gets the index of the first visible bar on the chart.
        public int FirstVisibleBarIndex { get; }

        //
        // Summary:
        //     Gets the index of the last visible bar on the chart.
        public int LastVisibleBarIndex { get; }

        //
        // Summary:
        //     Gets the maximum number of the visible bars on the chart.
        public int MaxVisibleBars { get; }

        //
        // Summary:
        //     Gets the total number of the bars on the chart.
        public int BarsTotal { get; }

        //
        // Summary:
        //     Gets the chart Bar objects.
        public NtQcBars Bars => (NtQcBars)mRobot.AbstractRobot.QcBars;

        //
        // Summary:
        //     Gets the time frame of the chart from 1 minute to 1 month.
        public TimeFrame TimeFrame
            => AbstractRobot.Secs2Tf(mRobot.AbstractRobot.QcBars.TimeFrameSeconds, out _);

        //
        // Summary:
        //     Gets the chart symbol.
        public Symbol Symbol => mRobot.Symbol;

        //
        // Summary:
        //     Gets the symbol name.
        public string SymbolName => mRobot.Symbol.Name;

        //
        // Summary:
        //     Gets or sets the value indicating whether the scrolling is enabled or disabled
        //     for the chart. If disabled, then the chart is not affected by scrolling, dragging,
        //     scaling, or pressing any keyboard keys, but is still affected by resizing, zooming,
        //     and API calls for changing X or Y-axis positions on the chart.
        public bool IsScrollingEnabled { get; set; }

        //
        // Summary:
        //     True if Chart is active otherwise False
        public bool IsActive { get; }

        //
        // Summary:
        //     True if Chart is visible otherwise False.
        public bool IsVisible { get; }

        //[Obsolete("Use ZoomLevel instead")]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //int Zoom { get; set; }

        //[Obsolete("Use NtQcBars instead")]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //MarketSeries MarketSeries { get; }
#if false
        //
        // Summary:
        //     Occurs when chart is activated
        event Action<ChartActivationChangedEventArgs> Activated;

        //
        // Summary:
        //     Occurs when chart is deactivated
        event Action<ChartActivationChangedEventArgs> Deactivated;

        //
        // Summary:
        //     Occurs when one or several charts display settings change.
        event Action<ChartDisplaySettingsEventArgs> DisplaySettingsChanged;

        //
        // Summary:
        //     Occurs when the chart color settings change.
        event Action<ChartColorEventArgs> ColorsChanged;

        //
        // Summary:
        //     Occurs when the chart type changes.
        event Action<ChartTypeEventArgs> ChartTypeChanged;

        //
        // Summary:
        //     Occurs when the chart zoom options change.
        event Action<ChartZoomEventArgs> ZoomChanged;

        //
        // Summary:
        //     Occurs when the indicator area is added.
        event Action<IndicatorAreaAddedEventArgs> IndicatorAreaAdded;

        //
        // Summary:
        //     Occurs when the indicator area is removed.
        event Action<IndicatorAreaRemovedEventArgs> IndicatorAreaRemoved;

        //
        // Summary:
        //     Occurs when a keyboard key pressed while the mouse cursor is over the chart
        event Action<ChartKeyboardEventArgs> KeyDown;

        //
        // Summary:
        //     Occurs when chart visibility changes.
        event Action<ChartVisibilityChangedEventArgs> VisibilityChanged;

        //
        // Summary:
        //     Scrolls the chart by the X-axis for the specified number of bars.
        //
        // Parameters:
        //   bars:
        //     The number of bars
        void ScrollXBy(int bars);

        //
        // Summary:
        //     Scrolls the chart by the X-axis to the bar with the specified index.
        //
        // Parameters:
        //   barIndex:
        //     The index of the bar.
        void ScrollXTo(int barIndex);

        //
        // Summary:
        //     Scrolls the chart by the X-axis to the specified date time.
        //
        // Parameters:
        //   time:
        //     The X-axis date time
        void ScrollXTo(DateTime time);

        //
        // Summary:
        //     Sets the color of the bar at the specified bar index. It will change the bar
        //     fill color and the outline color.
        //
        // Parameters:
        //   barIndex:
        //     The bar index number
        //
        //   color:
        //     The color for the bar
        void SetBarColor(long barIndex, Color color);

        //
        // Summary:
        //     Sets the fill color of the bar at the specified bar index.
        //
        // Parameters:
        //   barIndex:
        //     The bar index number
        //
        //   color:
        //     The color for filling the bar
        void SetBarFillColor(long barIndex, Color color);

        //
        // Summary:
        //     Sets the outline color of the bar at the specified bar index.
        //
        // Parameters:
        //   barIndex:
        //     The bar index number
        //
        //   color:
        //     The color for bar outline
        void SetBarOutlineColor(long barIndex, Color color);

        //
        // Summary:
        //     Sets the color of tick volume line at the specified bar index.
        //
        // Parameters:
        //   barIndex:
        //     The bar index number
        //
        //   color:
        //     The color for bar tick volume
        void SetTickVolumeColor(long barIndex, Color color);

        //
        // Summary:
        //     Resets the color of the bar to the default.
        //
        // Parameters:
        //   barIndex:
        //     The bar index number
        void ResetBarColor(long barIndex);

        //
        // Summary:
        //     Resets the colors of all the bars.
        void ResetBarColors();

        //
        // Summary:
        //     Resets the color of the tick volume line to the default at the specified bar
        //     index.
        //
        // Parameters:
        //   barIndex:
        //     The bar index number
        void ResetTickVolumeColor(long barIndex);

        //
        // Summary:
        //     Resets the color of all the tick volume bars.
        void ResetTickVolumeColors();

        //
        // Summary:
        //     Adds an hotkey to the chart that will call the handler when pressed
        //
        // Parameters:
        //   hotkeyHandler:
        //     The action delegate handler that will be called with a ChartKeyboardEventArgs
        //
        //
        //   key:
        //     The Hotkey main key
        //
        //   modifiers:
        //     The hotkey modifier key, default to None which means no modifer key
        //
        // Returns:
        //     bool (True if Hotkey was added successfully otherwise False)
        bool AddHotkey(Action<ChartKeyboardEventArgs> hotkeyHandler, Key key, ModifierKeys modifiers = ModifierKeys.None);

        //
        // Summary:
        //     Adds an hotkey to the chart that will call the handler when pressed
        //
        // Parameters:
        //   hotkeyHandler:
        //     The action delegate handler that will be called with a ChartKeyboardEventArgs
        //
        //
        //   hotkey:
        //     The keyboard key for hotkey
        //
        // Returns:
        //     bool (True if Hotkey was added successfully otherwise False)
        bool AddHotkey(Action<ChartKeyboardEventArgs> hotkeyHandler, string hotkey);

        //
        // Summary:
        //     Adds an hotkey to the chart that will call the handler when pressed
        //
        // Parameters:
        //   hotkeyHandler:
        //     The action delegate handler that will be called
        //
        //   key:
        //     The Hotkey main key
        //
        //   modifiers:
        //     The hotkey modifier key, default to None which means no modifer key
        //
        // Returns:
        //     bool (True if Hotkey was added successfully otherwise False)
        bool AddHotkey(Action hotkeyHandler, Key key, ModifierKeys modifiers = ModifierKeys.None);

        //
        // Summary:
        //     Adds an hotkey to the chart that will call the handler when pressed
        //
        // Parameters:
        //   hotkeyHandler:
        //     The action delegate handler that will be called
        //
        //   hotkey:
        //     The keyboard key for hotkey
        //
        // Returns:
        //     bool (True if Hotkey was added successfully otherwise False)
        bool AddHotkey(Action hotkeyHandler, string hotkey);

        //
        // Summary:
        //     Changes the time frame on the chart.
        //
        // Parameters:
        //   timeFrame:
        //     The time frame to change the current chart time frame to
        bool TryChangeTimeFrame(TimeFrame timeFrame);

        //
        // Summary:
        //     Changes the time frame and the symbol on the chart.
        //
        // Parameters:
        //   timeFrame:
        //     The time frame to change the current chart time frame to
        //
        //   symbolName:
        //     The symbol name to change the current chart symbol to
        bool TryChangeTimeFrameAndSymbol(TimeFrame timeFrame, string symbolName);

        //
        // Summary:
        //     Takes a chartshot from the chart that indicator / cBot runs on.
        //
        // Returns:
        //     If successful chartshot data inside a byte array in PNG format otherwise null.
        //
        //
        // Remarks:
        //     TakeChartshot only works if chart is visible.
        byte[] TakeChartshot();
#endif
    }
}
