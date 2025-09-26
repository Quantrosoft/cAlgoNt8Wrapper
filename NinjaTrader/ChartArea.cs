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

using cAlgoNt8Wrapper;
using NinjaTrader.FIX;
using NinjaTrader.Gui;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.Gui.NinjaScript.Wizard;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.NinjaScript.Strategies;
using System;

/*  Knowledge base 
    The chart is always drawn based on BarsArray[0] — that's the primary data series.
 */

namespace cAlgoNt8Wrapper
{
    //     The Chart Area Interface.
    public class ChartArea
    {
        protected Strategy mRobot;
        protected StrategyRenderBase mStrategy;
        protected NinjaTrader.Gui.Tools.SimpleFont mTextFont;

        //
        // Summary:
        //     Checks whether the instance is still on the chart area.
        public bool IsAlive { get; }

        //
        // Summary:
        //     Gets the width of the chart area.
        public double Width { get; }

        //
        // Summary:
        //     Gets the hight of the chart area.
        public double Height { get; }

        //
        // Summary:
        //     Gets the lowest visible Y-axis value.
        public double BottomY { get; }

        //
        // Summary:
        //     Gets the highest visible Y-axis value.
        public double TopY { get; }
#if false
        //
        // Summary:
        //     Gets the chart objects collection.
        IReadonlyList<ChartObject> Objects { get; }

        //
        // Summary:
        //     Gets the list of currently selected objects if any.
        IReadonlyList<ChartObject> SelectedObjects { get; }

        //
        // Summary:
        //     Occurs when the cursor hover over the chart area.
        event Action<ChartMouseEventArgs> MouseEnter;

        //
        // Summary:
        //     Occurs when the cursor leaves the chart area
        event Action<ChartMouseEventArgs> MouseLeave;

        //
        // Summary:
        //     Occurs when the cursor moves over the chart area.
        event Action<ChartMouseEventArgs> MouseMove;

        //
        // Summary:
        //     Occurs when the left mouse button is pressed down.
        event Action<ChartMouseEventArgs> MouseDown;

        //
        // Summary:
        //     Occurs when the left mouse button is released.
        event Action<ChartMouseEventArgs> MouseUp;

        //
        // Summary:
        //     Occurs when the mouse wheel button is rotated.
        event Action<ChartMouseWheelEventArgs> MouseWheel;

        //
        // Summary:
        //     Occurs when MouseDown event is happening on a chart area and a mouse is captured
        //     for dragging.
        event Action<ChartDragEventArgs> DragStart;

        //
        // Summary:
        //     Occurs when mouse button is released while dragging a chart area or a chart area
        //     loses mouse capture.
        event Action<ChartDragEventArgs> DragEnd;

        //
        // Summary:
        //     Occurs when dragging a chart area.
        event Action<ChartDragEventArgs> Drag;

        //
        // Summary:
        //     Occurs when the chart area size has changed.
        event Action<ChartSizeEventArgs> SizeChanged;

        //
        // Summary:
        //     Occurs when the X-axis position value or the Y-axis position value changes while
        //     scrolling.
        event Action<ChartScrollEventArgs> ScrollChanged;

        //
        // Summary:
        //     Occurs when one or several chart objects are added to the chart area.
        event Action<ChartObjectsAddedEventArgs> ObjectsAdded;

        //
        // Summary:
        //     Occurs when one or several chart objects are updated.
        event Action<ChartObjectsUpdatedEventArgs> ObjectsUpdated;

        //
        // Summary:
        //     Occurs when one or several chart object are removed from the chart area.
        event Action<ChartObjectsRemovedEventArgs> ObjectsRemoved;

        //
        // Summary:
        //     Occurs when chart objects are selected or deselected.
        event Action<ChartObjectsSelectionChangedEventArgs> ObjectsSelectionChanged;

        //
        // Summary:
        //     Occurs when the cursor hovers over or leaves the object.
        event Action<ChartObjectHoverChangedEventArgs> ObjectHoverChanged;

        //
        // Summary:
        //     Occurs when one or several chart objects move started.
        event Action<ChartObjectsMoveStartedEventArgs> ObjectsMoveStarted;

        //
        // Summary:
        //     Occurs when one or several chart objects are moving.
        event Action<ChartObjectsMovingEventArgs> ObjectsMoving;

        //
        // Summary:
        //     Occurs when one or several chart objects are moved.
        event Action<ChartObjectsMoveEndedEventArgs> ObjectsMoveEnded;

        [Obsolete("Use ObjectsAdded instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        event Action<ChartObjectAddedEventArgs> ObjectAdded;

        [Obsolete("Use ObjectsUpdated instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        event Action<ChartObjectUpdatedEventArgs> ObjectUpdated;

        [Obsolete("Use ObjectsRemoved instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        event Action<ChartObjectRemovedEventArgs> ObjectRemoved;

        [Obsolete("Use ObjectsSelectionChanged instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        event Action<ChartObjectSelectionChangedEventArgs> ObjectSelectionChanged;

        //
        // Summary:
        //     Adds a chart control from the Control Base to the main chart area or to the indicator
        //     area.
        //
        // Parameters:
        //   control:
        //     The control that will be added
        void AddControl(ControlBase control);

        //
        // Summary:
        //     Remove a chart control from the chart area.
        //
        // Parameters:
        //   control:
        //     An existing control that will be removed
        void RemoveControl(ControlBase control);

        //
        // Summary:
        //     Adds a chart control to chart / indicator area on absolute bar index and price
        //     (x,y) coordinates.
        //
        // Parameters:
        //   control:
        //     The control that will be added
        //
        //   barIndex:
        //     Bar Index
        //
        //   y:
        //     Price
        void AddControl(ControlBase control, int barIndex, double y);

        //
        // Summary:
        //     Adds a chart control to chart / indicator area on absolute time and price (x,y)
        //     coordinates.
        //
        // Parameters:
        //   control:
        //     The control that will be added
        //
        //   time:
        //     Time
        //
        //   y:
        //     Price
        void AddControl(ControlBase control, DateTime time, double y);

        //
        // Summary:
        //     Adds a chart control to chart / indicator area on absolute time (x) coordinate.
        //
        //
        // Parameters:
        //   control:
        //     The control that will be added
        //
        //   time:
        //     Time
        void AddControl(ControlBase control, DateTime time);

        //
        // Summary:
        //     Adds a chart control to chart / indicator area on absolute bar index (x) coordinate.
        //
        //
        // Parameters:
        //   control:
        //     The control that will be added
        //
        //   barIndex:
        //     Bar Index
        void AddControl(ControlBase control, int barIndex);

        //
        // Summary:
        //     Adds a chart control to chart / indicator area on absolute price (y) coordinate.
        //
        //
        // Parameters:
        //   control:
        //     The control that will be added
        //
        //   y:
        //     Price
        void AddControl(ControlBase control, double y);

        //
        // Summary:
        //     Moves a chart control to chart / indicator area on absolute bar index and price
        //     (x,y) coordinates.
        //
        // Parameters:
        //   control:
        //     The control that will be added
        //
        //   barIndex:
        //     Bar Index
        //
        //   y:
        //     Price
        void MoveControl(ControlBase control, int barIndex, double y);

        //
        // Summary:
        //     Moves a chart control to chart / indicator area on absolute time and price (x,y)
        //     coordinates.
        //
        // Parameters:
        //   control:
        //     The control that will be added
        //
        //   time:
        //     Time
        //
        //   y:
        //     Price
        void MoveControl(ControlBase control, DateTime time, double y);

        //
        // Summary:
        //     Moves a chart control to chart / indicator area on absolute time (x) coordinate.
        //
        //
        // Parameters:
        //   control:
        //     The control that will be added
        //
        //   time:
        //     Time
        void MoveControl(ControlBase control, DateTime time);

        //
        // Summary:
        //     Moves a chart control to chart / indicator area on absolute bar index (x) coordinate.
        //
        //
        // Parameters:
        //   control:
        //     The control that will be added
        //
        //   barIndex:
        //     Bar Index
        void MoveControl(ControlBase control, int barIndex);

        //
        // Summary:
        //     Moves a chart control to chart / indicator area on absolute price (y) coordinate.
        //
        //
        // Parameters:
        //   control:
        //     The control that will be added
        //
        //   y:
        //     Price
        void MoveControl(ControlBase control, double y);

        //
        // Summary:
        //     Sets the Y-axis lowest and highest values range. Allows scrolling the chart by
        //     the Y-axis. If only one of the values is set, then the chart will be expanded
        //     regarding the lowest or highest value respectively.
        //
        // Parameters:
        //   bottomY:
        //     The lowest visible Y-axis value.
        //
        //   topY:
        //     The highest visible Y-axis value.
        void SetYRange(double bottomY, double topY);
#endif
        //
        // Summary:
        //     Finds all the chart objects of the specified type.
        //
        // Type parameters:
        //   T:
        //
        // Returns:
        //     T[].
        public T[] FindAllObjects<T>() where T : ChartObject
        {
            return null;
        }

        //
        // Summary:
        //     Finds all the chart objects of the specified type.
        //
        // Parameters:
        //   objectType:
        //     Type of the object.
        //
        // Returns:
        //     ChartObject[].
        public ChartObject[] FindAllObjects(ChartObjectType objectType)
        {
            return null;
        }
#if false
        //
        // Summary:
        //     Finds the chart object of the specified name.
        //
        // Parameters:
        //   objectName:
        //     The name of the object.
        //
        // Returns:
        //     ChartObject.
        ChartObject FindObject(string objectName);
#endif
        //
        // Summary:
        //     Removes the chart object of the specified name.
        //
        // Parameters:
        //   objectName:
        //     The name of the object.
        public void RemoveObject(string objectName)
        {
        }

        //
        // Summary:
        //     Removes all interactive and non-interactive objects available for the cBot or
        //     Indicator.
        public void RemoveAllObjects()
        {
        }

        //
        // Summary:
        //     Draws a horizontal line.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once on a chart area.
        //     If duplicated, the chart object will be replaced with a new one of the same name.
        //
        //
        //   y:
        //     The Y-axis value of the line location.
        //
        //   color:
        //     The line color.
        //
        // Returns:
        //     ChartHorizontalLine.
        public ChartHorizontalLine DrawHorizontalLine(string name, double y, Color color)
        {
            return null;
        }

        //
        // Summary:
        //     Draws a horizontal line.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   y:
        //     The Y-axis value of the line location.
        //
        //   color:
        //     The line color.
        //
        //   thickness:
        //     The line thickness.
        //
        // Returns:
        //     ChartHorizontalLine.
        public ChartHorizontalLine DrawHorizontalLine(string name, double y, Color color, int thickness)
        {
            return null;
        }

        //
        // Summary:
        //     Draws a horizontal line.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   y:
        //     The Y-axis value of the line location.
        //
        //   color:
        //     The line color.
        //
        //   thickness:
        //     The line thickness.
        //
        //   lineStyle:
        //     The line style.
        //
        // Returns:
        //     ChartHorizontalLine.
        public ChartHorizontalLine DrawHorizontalLine(string name,
            double y,
            Color color,
            int thickness,
            LineStyle lineStyle)
        {
            return null;
        }
#if false
        //
        // Summary:
        //     Draws a vertical line.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time:
        //     The time value on the X-axis.
        //
        //   color:
        //     The line color.
        //
        // Returns:
        //     ChartVerticalLine.
        ChartVerticalLine DrawVerticalLine(string name, DateTime time, Color color);

        //
        // Summary:
        //     Draws a vertical line.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time:
        //     The time value of the line location on the X-axis.
        //
        //   color:
        //     The line color.
        //
        //   thickness:
        //     The line thickness.
        //
        // Returns:
        //     ChartVerticalLine.
        ChartVerticalLine DrawVerticalLine(string name, DateTime time, Color color, int thickness);

        //
        // Summary:
        //     Draws a vertical line.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time:
        //     The time value of the line location on the X-axis.
        //
        //   color:
        //     The line color.
        //
        //   thickness:
        //     The line thickness.
        //
        //   lineStyle:
        //     The line style.
        //
        // Returns:
        //     ChartVerticalLine.
        ChartVerticalLine DrawVerticalLine(string name, DateTime time, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a vertical line.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex:
        //     The bar index of the line location on the X-axis.
        //
        //   color:
        //     The line color.
        //
        // Returns:
        //     ChartVerticalLine.
        ChartVerticalLine DrawVerticalLine(string name, int barIndex, Color color);

        //
        // Summary:
        //     Draws a vertical line.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex:
        //     The bar index of the line location on the X-axis.
        //
        //   color:
        //     The line color.
        //
        //   thickness:
        //     The line thickness.
        //
        // Returns:
        //     ChartVerticalLine.
        ChartVerticalLine DrawVerticalLine(string name, int barIndex, Color color, int thickness);

        //
        // Summary:
        //     Draws a vertical line.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex:
        //     The bar index of the line location on the X-axis.
        //
        //   color:
        //     The line color.
        //
        //   thickness:
        //     The line thickness.
        //
        //   lineStyle:
        //     The line style.
        ChartVerticalLine DrawVerticalLine(string name, int barIndex, Color color, int thickness, LineStyle lineStyle);
#endif
        //
        // Summary:
        //     Draws a trend line.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the trend line start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the trend line start location.
        //
        //   time2:
        //     The time value of the trend line end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the trend line end location.
        //
        //   color:
        //     The color of the trend line.
        //
        //   thickness:
        //     The thickness of the trend line.
        //
        //   lineStyle:
        //     The trend line style.
        public ChartTrendLine DrawTrendLine(
            string name,
            DateTime time1,
            double y1,
            DateTime time2,
            double y2,
            Color color,
            int thickness = 1,
            LineStyle lineStyle = LineStyle.Solid)
        {
#if !GITHUB
            Draw.Line(mRobot, // owner
                name,   // tag
                true,   // isAutoScale
                time1 + TimeSpan.FromSeconds(mRobot.AbstractRobot.QcBars.TimeFrameSeconds),  // startTime
                y1,     // startY
                time2 + TimeSpan.FromSeconds(mRobot.AbstractRobot.QcBars.TimeFrameSeconds),  // endTime
                y2,     // endY
                NinjaBrushConverter.FromCtraderColor(color),    // brush
                MapToDashStyleHelper(lineStyle),    // dashStyle
                thickness);   // width
#endif
            return new ChartTrendLine
            {
                Time1 = time1,
                Y1 = y1,
                Time2 = time2,
                Y2 = y2,
                Color = color,
                Thickness = thickness,
                LineStyle = lineStyle
            };
        }


        //
        // Summary:
        //     Draws a trend line.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the trend line start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the trend line start location.
        //
        //   barIndex2:
        //     The bar index of the trend line end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the trend line end location.
        //
        //   color:
        //     The color of the trend line.
        //
        //   thickness:
        //     The thickness of the trend line.
        //
        //   lineStyle:
        //     The trend line style.
        public ChartTrendLine DrawTrendLine(
            string name,
            int barIndex1,
            double y1,
            int barIndex2,
            double y2,
            Color color,
            int thickness = 1,
            LineStyle lineStyle = LineStyle.Solid)
        {
            return null;
        }

#if false
        //
        // Summary:
        //     Draws an equidistant channel.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the equidistant channel start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the equidistant channel start location.
        //
        //   time2:
        //     The time value of the equidistant channel end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the equidistant channel end location.
        //
        //   channelHeight:
        //     The equidistant channel hight in pips.
        //
        //   color:
        //     The color of the equidistant channel lines.
        ChartEquidistantChannel DrawEquidistantChannel(string name, DateTime time1, double y1, DateTime time2, double y2, double channelHeight, Color color);

        //
        // Summary:
        //     Draws an equidistant channel.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the equidistant channel start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the equidistant channel start location.
        //
        //   time2:
        //     The time value of the equidistant channel end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the equidistant channel end location.
        //
        //   channelHeight:
        //     The equidistant channel hight in pips.
        //
        //   color:
        //     The color of the equidistant channel lines.
        //
        //   thickness:
        //     The thickness of the equidistant channel lines.
        ChartEquidistantChannel DrawEquidistantChannel(string name, DateTime time1, double y1, DateTime time2, double y2, double channelHeight, Color color, int thickness);

        //
        // Summary:
        //     Draws an equidistant channel.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the equidistant channel start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the equidistant channel start location.
        //
        //   time2:
        //     The time value of the equidistant channel end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the equidistant channel end location.
        //
        //   channelHeight:
        //     The equidistant channel hight in pips.
        //
        //   color:
        //     The color of the equidistant channel lines.
        //
        //   thickness:
        //     The thickness of the equidistant channel lines.
        //
        //   lineStyle:
        //     The equidistant channel lines style.
        ChartEquidistantChannel DrawEquidistantChannel(string name, DateTime time1, double y1, DateTime time2, double y2, double channelHeight, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws an equidistant channel.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the equidistant channel start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the equidistant channel start location.
        //
        //   barIndex2:
        //     The bar index of the equidistant channel end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the equidistant channel end location.
        //
        //   channelHeight:
        //     The equidistant channel hight in pips.
        //
        //   color:
        //     The color of the equidistant channel lines.
        ChartEquidistantChannel DrawEquidistantChannel(string name, int barIndex1, double y1, int barIndex2, double y2, double channelHeight, Color color);

        //
        // Summary:
        //     Draws an equidistant channel.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the equidistant channel start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the equidistant channel start location.
        //
        //   barIndex2:
        //     The bar index of the equidistant channel end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the equidistant channel end location.
        //
        //   channelHeight:
        //     The equidistant channel hight in pips.
        //
        //   color:
        //     The color of the equidistant channel lines.
        //
        //   thickness:
        //     The thickness of the equidistant channel lines.
        ChartEquidistantChannel DrawEquidistantChannel(string name, int barIndex1, double y1, int barIndex2, double y2, double channelHeight, Color color, int thickness);

        //
        // Summary:
        //     Draws an equidistant channel.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the equidistant channel start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the equidistant channel start location.
        //
        //   barIndex2:
        //     The bar index of the equidistant channel end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the equidistant channel end location.
        //
        //   channelHeight:
        //     The equidistant channel hight in pips.
        //
        //   color:
        //     The color of the equidistant channel lines.
        //
        //   thickness:
        //     The thickness of the equidistant channel lines.
        //
        //   lineStyle:
        //     The equidistant channel lines style.
        //
        // Returns:
        //     ChartEquidistantChannel.
        ChartEquidistantChannel DrawEquidistantChannel(string name, int barIndex1, double y1, int barIndex2, double y2, double channelHeight, Color color, int thickness, LineStyle lineStyle);
#endif
        //
        // Summary:
        //     Draws the text.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   text:
        //     The text content.
        //
        //   time:
        //     The time value of the text object location on the X-axis.
        //
        //   y:
        //     The Y-axis value of the text object location.
        //
        //   color:
        //     The color of the text.
        //
        // Returns:
        //     ChartText.
        public ChartText DrawText(string name, string text, DateTime time, double y, Color color)
        {
            //public static Text Text(NinjaScriptBase owner, string tag, bool isAutoScale, string text, DateTime time, double y,
            //int yPixelOffset, Brush textBrush, Gui.Tools.SimpleFont font,
            //TextAlignment alignment, Brush outlineBrush, Brush areaBrush,
            //int areaOpacity, DashStyleHelper outlineDashStyle,
            //int outlineWidth, bool isGlobal, string templateName)

            if (null != mStrategy.ChartControl)
            {
                // NinjaTrader uses the close time of a bar to draw it on the chart
                // The chart is always drawn based on BarsArray[0] — that's the primary data series
                Draw.Text(mRobot, name, true, text, time + TimeSpan.FromSeconds(mRobot.AbstractRobot.QcBars.TimeFrameSeconds), y,
                    0, NinjaBrushConverter.FromCtraderColor(color), mTextFont,
                    System.Windows.TextAlignment.Center, System.Windows.Media.Brushes.Transparent, System.Windows.Media.Brushes.Transparent,
                    0, DashStyleHelper.Solid, 1, false, "");    // V0.12 isGlobal must be false to draw only in current chart
            }

            return new ChartText { Text = text, Time = time, Y = y, Color = color };
        }

        //
        // Summary:
        //     Draws the text.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   text:
        //     The text content.
        //
        //   barIndex:
        //     The bar index of the text object location on the X-axis.
        //
        //   y:
        //     The Y-axis value of the text object location.
        //
        //   color:
        //     The color of the text.
        //
        // Returns:
        //     ChartText.
        public ChartText DrawText(string name, string text, int barIndex, double y, Color color)
        {
            if (null != mStrategy.ChartControl)
                Draw.Text(mRobot, name, text, barIndex, y,
                    NinjaBrushConverter.FromCtraderColor(color));

            return new ChartText { Text = text, Time = mStrategy.Time[barIndex], Y = y, Color = color };
        }

        //
        // Summary:
        //     Draws the static text.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   text:
        //     The text content.
        //
        //   verticalAlignment:
        //     The vertical alignment.
        //
        //   horizontalAlignment:
        //     The horizontal alignment.
        //
        //   color:
        //     The color of the text.
        public ChartStaticText DrawStaticText(string name,
            string text,
            VerticalAlignment verticalAlignment,
            HorizontalAlignment horizontalAlignment,
            Color color)
        {
            if (null != mStrategy.ChartControl)
            {
                Draw.TextFixed(mStrategy,
                        name,
                        text,
                        ToTextPosition(verticalAlignment, horizontalAlignment),
                        NinjaBrushConverter.FromCtraderColor(color),
                        mTextFont,
                        System.Windows.Media.Brushes.Transparent,
                        System.Windows.Media.Brushes.Transparent,
                        0
                    );
            }
            return new ChartStaticText { Text = text, Color = color };
        }

        //
        // Summary:
        //     Draws an icon.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   iconType:
        //     The type of the icon.
        //
        //   time:
        //     The time value of the icon location on the X-axis.
        //
        //   y:
        //     The Y-axis value of the icon location.
        //
        //   color:
        //     The color of the icon.
        //
        // Returns:
        //     ChartIcon.
        public ChartIcon DrawIcon(string name, ChartIconType iconType, DateTime time, double y, Color color)
        {
            DrawText(name, MapToUnicode(iconType), time, y, color);
            return new ChartIcon
            {
                IconType = iconType,
                Time = time,
                Y = y,
                Color = color
            };
        }

        //
        // Summary:
        //     Draws an icon.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   iconType:
        //     Type of the icon.
        //
        //   barIndex:
        //     The bar index of the icon location on the X-axis.
        //
        //   y:
        //     The Y-axis value of the icon location.
        //
        //   color:
        //     The color of the icon.
        //
        // Returns:
        //     ChartIcon.
        public ChartIcon DrawIcon(string name, ChartIconType iconType, int barIndex, double y, Color color)
        {
            return null;
        }
#if false
        //
        // Summary:
        //     Draws a Fibonacci retracement.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci retracement start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci retracement start point location.
        //
        //   time2:
        //     The time value of the Fibonacci retracement end point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci retracement end point location.
        //
        //   color:
        //     The Fibonacci retracement lines color.
        //
        // Returns:
        //     ChartFibonacciRetracement.
        ChartFibonacciRetracement DrawFibonacciRetracement(string name, DateTime time1, double y1, DateTime time2, double y2, Color color);

        //
        // Summary:
        //     Draws a Fibonacci retracement.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci retracement start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci retracement start point location.
        //
        //   time2:
        //     The time value of the Fibonacci retracement end point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci retracement end point location.
        //
        //   color:
        //     The Fibonacci retracement lines color.
        //
        //   thickness:
        //     The Fibonacci retracement lines thickness.
        //
        // Returns:
        //     ChartFibonacciRetracement.
        ChartFibonacciRetracement DrawFibonacciRetracement(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws a Fibonacci retracement.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci retracement start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci retracement start point location.
        //
        //   time2:
        //     The time value of the Fibonacci retracement end point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci retracement end point location.
        //
        //   color:
        //     The Fibonacci retracement lines color.
        //
        //   thickness:
        //     The Fibonacci retracement lines thickness.
        //
        //   lineStyle:
        //     The Fibonacci retracement lines style.
        //
        // Returns:
        //     ChartFibonacciRetracement.
        ChartFibonacciRetracement DrawFibonacciRetracement(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a Fibonacci retracement.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci retracement start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci retracement start point location.
        //
        //   barIndex2:
        //     The bar index of the Fibonacci retracement end point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci retracement end point location.
        //
        //   color:
        //     The Fibonacci retracement lines color.
        //
        // Returns:
        //     ChartFibonacciRetracement.
        ChartFibonacciRetracement DrawFibonacciRetracement(string name, int barIndex1, double y1, int barIndex2, double y2, Color color);

        //
        // Summary:
        //     Draws a Fibonacci retracement.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci retracement start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci retracement start point location.
        //
        //   barIndex2:
        //     The bar index of the Fibonacci retracement end point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci retracement end point location.
        //
        //   color:
        //     The Fibonacci retracement lines color.
        //
        //   thickness:
        //     The Fibonacci retracement lines thickness.
        //
        // Returns:
        //     ChartFibonacciRetracement.
        ChartFibonacciRetracement DrawFibonacciRetracement(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws a Fibonacci retracement.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci retracement start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci retracement start point location.
        //
        //   barIndex2:
        //     The bar index of the Fibonacci retracement end point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci retracement end point location.
        //
        //   color:
        //     The Fibonacci retracement lines color.
        //
        //   thickness:
        //     The Fibonacci retracement lines thickness.
        //
        //   lineStyle:
        //     The Fibonacci retracement lines style.
        //
        // Returns:
        //     ChartFibonacciRetracement.
        ChartFibonacciRetracement DrawFibonacciRetracement(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a Fibonacci expansion.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci expansion start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci expansion start point location.
        //
        //   time2:
        //     The time value of the Fibonacci expansion central point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci expansion central point location.
        //
        //   time3:
        //     The time value of the Fibonacci expansion end point location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Fibonacci expansion end point location.
        //
        //   color:
        //     The Fibonacci expansion lines color.
        //
        // Returns:
        //     ChartFibonacciExpansion.
        ChartFibonacciExpansion DrawFibonacciExpansion(string name, DateTime time1, double y1, DateTime time2, double y2, DateTime time3, double y3, Color color);

        //
        // Summary:
        //     Draws a Fibonacci expansion.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci expansion start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci expansion start point location.
        //
        //   time2:
        //     The time value of the Fibonacci expansion central point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci expansion central point location.
        //
        //   time3:
        //     The time value of the Fibonacci expansion end point location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Fibonacci expansion end point location.
        //
        //   color:
        //     The Fibonacci expansion lines color.
        //
        //   thickness:
        //     The Fibonacci expansion lines thickness.
        //
        // Returns:
        //     ChartFibonacciExpansion.
        ChartFibonacciExpansion DrawFibonacciExpansion(string name, DateTime time1, double y1, DateTime time2, double y2, DateTime time3, double y3, Color color, int thickness);

        //
        // Summary:
        //     Draws a Fibonacci expansion.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci expansion start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci expansion start point location.
        //
        //   time2:
        //     The time value of the Fibonacci expansion central point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci expansion central point location.
        //
        //   time3:
        //     The time value of the Fibonacci expansion end point location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Fibonacci expansion end point location.
        //
        //   color:
        //     The Fibonacci expansion lines color.
        //
        //   thickness:
        //     The Fibonacci expansion lines thickness.
        //
        //   lineStyle:
        //     The Fibonacci expansion lines style.
        //
        // Returns:
        //     ChartFibonacciExpansion.
        ChartFibonacciExpansion DrawFibonacciExpansion(string name, DateTime time1, double y1, DateTime time2, double y2, DateTime time3, double y3, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a Fibonacci expansion.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci expansion start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci expansion start point location.
        //
        //   barIndex2:
        //     The bar index of the Fibonacci expansion central point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci expansion central point location.
        //
        //   barIndex3:
        //     The bar index of the Fibonacci expansion end point location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Fibonacci expansion end point location.
        //
        //   color:
        //     The Fibonacci expansion lines color.
        //
        // Returns:
        //     ChartFibonacciExpansion.
        ChartFibonacciExpansion DrawFibonacciExpansion(string name, int barIndex1, double y1, int barIndex2, double y2, int barIndex3, double y3, Color color);

        //
        // Summary:
        //     Draws a Fibonacci expansion.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci expansion start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci expansion start point location.
        //
        //   barIndex2:
        //     The bar index of the Fibonacci expansion central point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci expansion central point location.
        //
        //   barIndex3:
        //     The bar index of the Fibonacci expansion end point location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Fibonacci expansion end point location.
        //
        //   color:
        //     The Fibonacci expansion lines color.
        //
        //   thickness:
        //     The Fibonacci expansion lines thickness.
        //
        // Returns:
        //     ChartFibonacciExpansion.
        ChartFibonacciExpansion DrawFibonacciExpansion(string name, int barIndex1, double y1, int barIndex2, double y2, int barIndex3, double y3, Color color, int thickness);

        //
        // Summary:
        //     Draws a Fibonacci expansion.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci expansion start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci expansion start point location.
        //
        //   barIndex2:
        //     The bar index of the Fibonacci expansion central point location on the X-axis.
        //
        //
        //   y2:
        //     The Y-axis value of the Fibonacci expansion central point location.
        //
        //   barIndex3:
        //     The bar index of the Fibonacci expansion end point location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Fibonacci expansion end point location.
        //
        //   color:
        //     The Fibonacci expansion lines color.
        //
        //   thickness:
        //     The Fibonacci expansion lines thickness.
        //
        //   lineStyle:
        //     The Fibonacci expansion lines style.
        //
        // Returns:
        //     ChartFibonacciExpansion.
        ChartFibonacciExpansion DrawFibonacciExpansion(string name, int barIndex1, double y1, int barIndex2, double y2, int barIndex3, double y3, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a Fibonacci fan.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci fan start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Fibonacci fan start point location.
        //
        //   time2:
        //     The time value of the Fibonacci fan end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci fan end point location.
        //
        //   color:
        //     The Fibonacci fan lines color.
        //
        // Returns:
        //     ChartFibonacciFan.
        ChartFibonacciFan DrawFibonacciFan(string name, DateTime time1, double y1, DateTime time2, double y2, Color color);

        //
        // Summary:
        //     Draws a Fibonacci fan.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci fan start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Fibonacci fan start point location.
        //
        //   time2:
        //     The time value of the Fibonacci fan end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci fan end point location.
        //
        //   color:
        //     The Fibonacci fan lines color.
        //
        //   thickness:
        //     The Fibonacci fan lines thickness.
        //
        // Returns:
        //     ChartFibonacciFan.
        ChartFibonacciFan DrawFibonacciFan(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws a Fibonacci fan.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci fan start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Fibonacci fan start point location.
        //
        //   time2:
        //     The time value of the Fibonacci fan end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci fan end point location.
        //
        //   color:
        //     The Fibonacci fan lines color.
        //
        //   thickness:
        //     The Fibonacci fan lines thickness.
        //
        //   lineStyle:
        //     The Fibonacci fan lines style.
        //
        // Returns:
        //     ChartFibonacciFan.
        ChartFibonacciFan DrawFibonacciFan(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a Fibonacci fan.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci fan start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Fibonacci fan start point location.
        //
        //   barIndex2:
        //     The Y-axis value of the Fibonacci fan end point location.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci fan end point location.
        //
        //   color:
        //     The Fibonacci fan lines color.
        //
        // Returns:
        //     ChartFibonacciFan.
        ChartFibonacciFan DrawFibonacciFan(string name, int barIndex1, double y1, int barIndex2, double y2, Color color);

        //
        // Summary:
        //     Draws a Fibonacci fan.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci fan start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Fibonacci fan start point location.
        //
        //   barIndex2:
        //     The Y-axis value of the Fibonacci fan end point location.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci fan end point location.
        //
        //   color:
        //     The Fibonacci fan lines color.
        //
        //   thickness:
        //     The Fibonacci fan lines thickness.
        //
        // Returns:
        //     ChartFibonacciFan.
        ChartFibonacciFan DrawFibonacciFan(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws a Fibonacci fan.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci fan start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Fibonacci fan start point location.
        //
        //   barIndex2:
        //     The Y-axis value of the Fibonacci fan end point location.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci fan end point location.
        //
        //   color:
        //     The Fibonacci fan lines color.
        //
        //   thickness:
        //     The Fibonacci fan lines thickness.
        //
        //   lineStyle:
        //     The Fibonacci fan lines style.
        //
        // Returns:
        //     ChartFibonacciFan.
        ChartFibonacciFan DrawFibonacciFan(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a Fibonacci timezones.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci timezones start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci timezones start point location.
        //
        //   time2:
        //     The time value of the Fibonacci timezones end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci timezones end point location.
        //
        //   color:
        //     The Fibonacci timezones lines color.
        //
        // Returns:
        //     ChartFibonacciTimezones.
        ChartFibonacciTimezones DrawFibonacciTimezones(string name, DateTime time1, double y1, DateTime time2, double y2, Color color);

        //
        // Summary:
        //     Draws a Fibonacci timezones.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci timezones start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci timezones start point location.
        //
        //   time2:
        //     The time value of the Fibonacci timezones end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci timezones end point location.
        //
        //   color:
        //     The Fibonacci timezones lines color.
        //
        //   thickness:
        //     The Fibonacci timezones lines thickness.
        //
        // Returns:
        //     ChartFibonacciTimezones.
        ChartFibonacciTimezones DrawFibonacciTimezones(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws a Fibonacci timezones.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Fibonacci timezones start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci timezones start point location.
        //
        //   time2:
        //     The time value of the Fibonacci timezones end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci timezones end point location.
        //
        //   color:
        //     The Fibonacci timezones lines color.
        //
        //   thickness:
        //     The Fibonacci timezones lines thickness.
        //
        //   lineStyle:
        //     The Fibonacci timezones lines style.
        //
        // Returns:
        //     ChartFibonacciTimezones.
        ChartFibonacciTimezones DrawFibonacciTimezones(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a Fibonacci timezones.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci timezones start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci timezones start point location.
        //
        //   barIndex2:
        //     The Y-axis value of the Fibonacci timezones end point location.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci timezones end point location.
        //
        //   color:
        //     The Fibonacci timezones lines color.
        //
        // Returns:
        //     ChartFibonacciTimezones.
        ChartFibonacciTimezones DrawFibonacciTimezones(string name, int barIndex1, double y1, int barIndex2, double y2, Color color);

        //
        // Summary:
        //     Draws a Fibonacci timezones.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci timezones start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci timezones start point location.
        //
        //   barIndex2:
        //     The Y-axis value of the Fibonacci timezones end point location.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci timezones end point location.
        //
        //   color:
        //     The Fibonacci timezones lines color.
        //
        //   thickness:
        //     The Fibonacci timezones lines thickness.
        //
        // Returns:
        //     ChartFibonacciTimezones.
        ChartFibonacciTimezones DrawFibonacciTimezones(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws a Fibonacci timezones.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Fibonacci timezones start point location on the X-axis.
        //
        //
        //   y1:
        //     The Y-axis value of the Fibonacci timezones start point location.
        //
        //   barIndex2:
        //     The Y-axis value of the Fibonacci timezones end point location.
        //
        //   y2:
        //     The Y-axis value of the Fibonacci timezones end point location.
        //
        //   color:
        //     The Fibonacci timezones lines color.
        //
        //   thickness:
        //     The Fibonacci timezones lines thickness.
        //
        //   lineStyle:
        //     The Fibonacci timezones lines style.
        //
        // Returns:
        //     ChartFibonacciTimezones.
        ChartFibonacciTimezones DrawFibonacciTimezones(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws an Andrew's pitchfork.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Andrew's pitchfork point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Andrew's pitchfork point 1 location.
        //
        //   time2:
        //     The time value of the Andrew's pitchfork point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Andrew's pitchfork point 2 location.
        //
        //   time3:
        //     The time value of the Andrew's pitchfork point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Andrew's pitchfork point 3 location.
        //
        //   color:
        //     The Andrew's pitchfork lines color.
        //
        // Returns:
        //     ChartAndrewsPitchfork.
        ChartAndrewsPitchfork DrawAndrewsPitchfork(string name, DateTime time1, double y1, DateTime time2, double y2, DateTime time3, double y3, Color color);

        //
        // Summary:
        //     Draws an Andrew's pitchfork.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Andrew's pitchfork point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Andrew's pitchfork point 1 location.
        //
        //   time2:
        //     The time value of the Andrew's pitchfork point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Andrew's pitchfork point 2 location.
        //
        //   time3:
        //     The time value of the Andrew's pitchfork point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Andrew's pitchfork point 3 location.
        //
        //   color:
        //     The Andrew's pitchfork lines color.
        //
        //   thickness:
        //     The Andrew's pitchfork lines thickness.
        //
        // Returns:
        //     ChartAndrewsPitchfork.
        ChartAndrewsPitchfork DrawAndrewsPitchfork(string name, DateTime time1, double y1, DateTime time2, double y2, DateTime time3, double y3, Color color, int thickness);

        //
        // Summary:
        //     Draws an Andrew's pitchfork.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the Andrew's pitchfork point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Andrew's pitchfork point 1 location.
        //
        //   time2:
        //     The time value of the Andrew's pitchfork point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Andrew's pitchfork point 2 location.
        //
        //   time3:
        //     The time value of the Andrew's pitchfork point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Andrew's pitchfork point 3 location.
        //
        //   color:
        //     The Andrew's pitchfork lines color.
        //
        //   thickness:
        //     The Andrew's pitchfork lines thickness.
        //
        //   lineStyle:
        //     The Andrew's pitchfork lines style.
        //
        // Returns:
        //     ChartAndrewsPitchfork.
        ChartAndrewsPitchfork DrawAndrewsPitchfork(string name, DateTime time1, double y1, DateTime time2, double y2, DateTime time3, double y3, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws an Andrew's pitchfork.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Andrew's pitchfork point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Andrew's pitchfork point 1 location.
        //
        //   barIndex2:
        //     The bar index of the Andrew's pitchfork point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Andrew's pitchfork point 2 location.
        //
        //   barIndex3:
        //     The bar index of the Andrew's pitchfork point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Andrew's pitchfork point 3 location.
        //
        //   color:
        //     The Andrew's pitchfork lines color.
        //
        // Returns:
        //     ChartAndrewsPitchfork.
        ChartAndrewsPitchfork DrawAndrewsPitchfork(string name, int barIndex1, double y1, int barIndex2, double y2, int barIndex3, double y3, Color color);

        //
        // Summary:
        //     Draws an Andrews pitchfork.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Andrew's pitchfork point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Andrew's pitchfork point 1 location.
        //
        //   barIndex2:
        //     The bar index of the Andrew's pitchfork point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Andrew's pitchfork point 2 location.
        //
        //   barIndex3:
        //     The bar index of the Andrew's pitchfork point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Andrew's pitchfork point 3 location.
        //
        //   color:
        //     The Andrew's pitchfork lines color.
        //
        //   thickness:
        //     The Andrew's pitchfork lines thickness.
        //
        // Returns:
        //     ChartAndrewsPitchfork.
        ChartAndrewsPitchfork DrawAndrewsPitchfork(string name, int barIndex1, double y1, int barIndex2, double y2, int barIndex3, double y3, Color color, int thickness);

        //
        // Summary:
        //     Draws an Andrew's pitchfork.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the Andrew's pitchfork point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the Andrew's pitchfork point 1 location.
        //
        //   barIndex2:
        //     The bar index of the Andrew's pitchfork point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the Andrew's pitchfork point 2 location.
        //
        //   barIndex3:
        //     The bar index of the Andrew's pitchfork point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the Andrew's pitchfork point 3 location.
        //
        //   color:
        //     The Andrew's pitchfork lines color.
        //
        //   thickness:
        //     The Andrew's pitchfork lines thickness.
        //
        //   lineStyle:
        //     The Andrew's pitchfork lines style.
        //
        // Returns:
        //     ChartAndrewsPitchfork.
        ChartAndrewsPitchfork DrawAndrewsPitchfork(string name, int barIndex1, double y1, int barIndex2, double y2, int barIndex3, double y3, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a rectangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the rectangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the rectangle point 1 location.
        //
        //   time2:
        //     The time value of the rectangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the rectangle point 2 location.
        //
        //   color:
        //     The rectangle lines color.
        //
        // Returns:
        //     ChartRectangle.
        ChartRectangle DrawRectangle(string name, DateTime time1, double y1, DateTime time2, double y2, Color color);

        //
        // Summary:
        //     Draws a rectangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the rectangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the rectangle point 1 location.
        //
        //   time2:
        //     The time value of the rectangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the rectangle point 2 location.
        //
        //   color:
        //     The rectangle lines color.
        //
        //   thickness:
        //     The rectangle lines thickness.
        //
        // Returns:
        //     ChartRectangle.
        ChartRectangle DrawRectangle(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws a rectangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the rectangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the rectangle point 1 location.
        //
        //   time2:
        //     The time value of the rectangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the rectangle point 2 location.
        //
        //   color:
        //     The rectangle lines color.
        //
        //   thickness:
        //     The rectangle lines thickness.
        //
        //   lineStyle:
        //     The rectangle lines style.
        //
        // Returns:
        //     ChartRectangle.
        ChartRectangle DrawRectangle(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a rectangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the rectangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the rectangle point 1 location.
        //
        //   barIndex2:
        //     The bar index of the rectangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the rectangle point 2 location.
        //
        //   color:
        //     The rectangle lines color.
        //
        // Returns:
        //     ChartRectangle.
        ChartRectangle DrawRectangle(string name, int barIndex1, double y1, int barIndex2, double y2, Color color);

        //
        // Summary:
        //     Draws a rectangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the rectangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the rectangle point 1 location.
        //
        //   barIndex2:
        //     The bar index of the rectangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the rectangle point 2 location.
        //
        //   color:
        //     The rectangle lines color.
        //
        //   thickness:
        //     The rectangle lines thickness.
        //
        // Returns:
        //     ChartRectangle.
        ChartRectangle DrawRectangle(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws a rectangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the rectangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the rectangle point 1 location.
        //
        //   barIndex2:
        //     The bar index of the rectangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the rectangle point 2 location.
        //
        //   color:
        //     The rectangle lines color.
        //
        //   thickness:
        //     The rectangle lines thickness.
        //
        //   lineStyle:
        //     The rectangle lines style.
        //
        // Returns:
        //     ChartRectangle.
        ChartRectangle DrawRectangle(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws an ellipse.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the ellipse point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the ellipse point 1 location.
        //
        //   time2:
        //     The time value of the ellipse point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the ellipse point 2 location.
        //
        //   color:
        //     The ellipse line color.
        //
        // Returns:
        //     ChartEllipse.
        ChartEllipse DrawEllipse(string name, DateTime time1, double y1, DateTime time2, double y2, Color color);

        //
        // Summary:
        //     Draws an ellipse.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the ellipse point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the ellipse point 1 location.
        //
        //   time2:
        //     The time value of the ellipse point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the ellipse point 2 location.
        //
        //   color:
        //     The ellipse line color.
        //
        //   thickness:
        //     The ellipse line thickness.
        //
        // Returns:
        //     ChartEllipse.
        ChartEllipse DrawEllipse(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws an ellipse.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the ellipse point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the ellipse point 1 location.
        //
        //   time2:
        //     The time value of the ellipse point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the ellipse point 2 location.
        //
        //   color:
        //     The ellipse line color.
        //
        //   thickness:
        //     The ellipse line thickness.
        //
        //   lineStyle:
        //     The ellipse line style.
        //
        // Returns:
        //     ChartEllipse.
        ChartEllipse DrawEllipse(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws an ellipse.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the ellipse point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the ellipse point 1 location.
        //
        //   barIndex2:
        //     The bar index of the ellipse point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the ellipse point 2 location.
        //
        //   color:
        //     The ellipse line color.
        //
        // Returns:
        //     ChartEllipse.
        ChartEllipse DrawEllipse(string name, int barIndex1, double y1, int barIndex2, double y2, Color color);

        //
        // Summary:
        //     Draws an ellipse.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the ellipse point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the ellipse point 1 location.
        //
        //   barIndex2:
        //     The bar index of the ellipse point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the ellipse point 2 location.
        //
        //   color:
        //     The ellipse line color.
        //
        //   thickness:
        //     The ellipse line thickness.
        //
        // Returns:
        //     ChartEllipse.
        ChartEllipse DrawEllipse(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws an ellipse.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the ellipse point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the ellipse point 1 location.
        //
        //   barIndex2:
        //     The bar index of the ellipse point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the ellipse point 2 location.
        //
        //   color:
        //     The ellipse line color.
        //
        //   thickness:
        //     The ellipse line thickness.
        //
        //   lineStyle:
        //     The ellipse line style.
        //
        // Returns:
        //     ChartEllipse.
        ChartEllipse DrawEllipse(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a triangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the triangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the triangle point 1 location.
        //
        //   time2:
        //     The time value of the triangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the triangle point 2 location.
        //
        //   time3:
        //     The time value of the triangle point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the triangle point 3 location.
        //
        //   color:
        //     The triangle line color.
        //
        // Returns:
        //     ChartTriangle.
        ChartTriangle DrawTriangle(string name, DateTime time1, double y1, DateTime time2, double y2, DateTime time3, double y3, Color color);

        //
        // Summary:
        //     Draws a triangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the triangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the triangle point 1 location.
        //
        //   time2:
        //     The time value of the triangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the triangle point 2 location.
        //
        //   time3:
        //     The time value of the triangle point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the triangle point 3 location.
        //
        //   color:
        //     The triangle line color.
        //
        //   thickness:
        //     The triangle line thickness.
        //
        // Returns:
        //     ChartTriangle.
        ChartTriangle DrawTriangle(string name, DateTime time1, double y1, DateTime time2, double y2, DateTime time3, double y3, Color color, int thickness);

        //
        // Summary:
        //     Draws a triangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the triangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the triangle point 1 location.
        //
        //   time2:
        //     The time value of the triangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the triangle point 2 location.
        //
        //   time3:
        //     The time value of the triangle point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the triangle point 3 location.
        //
        //   color:
        //     The triangle line color.
        //
        //   thickness:
        //     The triangle line thickness.
        //
        //   lineStyle:
        //     The triangle line style.
        //
        // Returns:
        //     ChartTriangle.
        ChartTriangle DrawTriangle(string name, DateTime time1, double y1, DateTime time2, double y2, DateTime time3, double y3, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws a triangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the triangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the triangle point 1 location.
        //
        //   barIndex2:
        //     The bar index of the triangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the triangle point 2 location.
        //
        //   barIndex3:
        //     The bar index of the triangle point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the triangle point 3 location.
        //
        //   color:
        //     The triangle line color.
        //
        // Returns:
        //     ChartTriangle.
        ChartTriangle DrawTriangle(string name, int barIndex1, double y1, int barIndex2, double y2, int barIndex3, double y3, Color color);

        //
        // Summary:
        //     Draws a triangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the triangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the triangle point 1 location.
        //
        //   barIndex2:
        //     The bar index of the triangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the triangle point 2 location.
        //
        //   barIndex3:
        //     The bar index of the triangle point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the triangle point 3 location.
        //
        //   color:
        //     The triangle line color.
        //
        //   thickness:
        //     The triangle line thickness.
        //
        // Returns:
        //     ChartTriangle.
        ChartTriangle DrawTriangle(string name, int barIndex1, double y1, int barIndex2, double y2, int barIndex3, double y3, Color color, int thickness);

        //
        // Summary:
        //     Draws a triangle.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the triangle point 1 location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the triangle point 1 location.
        //
        //   barIndex2:
        //     The bar index of the triangle point 2 location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the triangle point 2 location.
        //
        //   barIndex3:
        //     The bar index of the triangle point 3 location on the X-axis.
        //
        //   y3:
        //     The Y-axis value of the triangle point 3 location.
        //
        //   color:
        //     The triangle line color.
        //
        //   thickness:
        //     The triangle line thickness.
        //
        //   lineStyle:
        //     The triangle line style.
        //
        // Returns:
        //     ChartTriangle.
        ChartTriangle DrawTriangle(string name, int barIndex1, double y1, int barIndex2, double y2, int barIndex3, double y3, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws an arrow.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the trend line start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the trend line start location.
        //
        //   time2:
        //     The time value of the trend line end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the trend line end location.
        //
        //   color:
        //     The color of the trend line.
        ChartArrow DrawArrow(string name, DateTime time1, double y1, DateTime time2, double y2, Color color);

        //
        // Summary:
        //     Draws an arrow.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the trend line start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the trend line start location.
        //
        //   time2:
        //     The time value of the trend line end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the trend line end location.
        //
        //   color:
        //     The color of the trend line.
        //
        //   thickness:
        //     The thickness of the trend line.
        ChartArrow DrawArrow(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws an arrow.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   time1:
        //     The time value of the trend line start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the trend line start location.
        //
        //   time2:
        //     The time value of the trend line end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the trend line end location.
        //
        //   color:
        //     The color of the trend line.
        //
        //   thickness:
        //     The thickness of the trend line.
        //
        //   lineStyle:
        //     The trend line style.
        ChartArrow DrawArrow(string name, DateTime time1, double y1, DateTime time2, double y2, Color color, int thickness, LineStyle lineStyle);

        //
        // Summary:
        //     Draws an arrow.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the trend line start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the trend line start location.
        //
        //   barIndex2:
        //     The bar index of the trend line end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the trend line end location.
        //
        //   color:
        //     The color of the trend line.
        ChartArrow DrawArrow(string name, int barIndex1, double y1, int barIndex2, double y2, Color color);

        //
        // Summary:
        //     Draws an arrow.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the trend line start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the trend line start location.
        //
        //   barIndex2:
        //     The bar index of the trend line end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the trend line end location.
        //
        //   color:
        //     The color of the trend line.
        //
        //   thickness:
        //     The thickness of the trend line.
        ChartArrow DrawArrow(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness);

        //
        // Summary:
        //     Draws an arrow.
        //
        // Parameters:
        //   name:
        //     The chart object name - a unique name that can be only used once for a chart
        //     area. If duplicated, the chart object will be replaced with a new one of the
        //     same name.
        //
        //   barIndex1:
        //     The bar index of the trend line start point location on the X-axis.
        //
        //   y1:
        //     The Y-axis value of the trend line start location.
        //
        //   barIndex2:
        //     The bar index of the trend line end point location on the X-axis.
        //
        //   y2:
        //     The Y-axis value of the trend line end location.
        //
        //   color:
        //     The color of the trend line.
        //
        //   thickness:
        //     The thickness of the trend line.
        //
        //   lineStyle:
        //     The trend line style.
        ChartArrow DrawArrow(string name, int barIndex1, double y1, int barIndex2, double y2, Color color, int thickness, LineStyle lineStyle);
#endif
        private NinjaTrader.NinjaScript.DrawingTools.TextPosition ToTextPosition(VerticalAlignment vAlign, HorizontalAlignment hAlign)
        {
            switch (vAlign)
            {
                case VerticalAlignment.Top:
                    switch (hAlign)
                    {
                        case HorizontalAlignment.Left:
                            return NinjaTrader.NinjaScript.DrawingTools.TextPosition.TopLeft;
                        case HorizontalAlignment.Right:
                            return NinjaTrader.NinjaScript.DrawingTools.TextPosition.TopRight;
                        default:
                            return NinjaTrader.NinjaScript.DrawingTools.TextPosition.Center;
                    }

                case VerticalAlignment.Bottom:
                    switch (hAlign)
                    {
                        case HorizontalAlignment.Left:
                            return NinjaTrader.NinjaScript.DrawingTools.TextPosition.BottomLeft;
                        case HorizontalAlignment.Right:
                            return NinjaTrader.NinjaScript.DrawingTools.TextPosition.BottomRight;
                        default:
                            return NinjaTrader.NinjaScript.DrawingTools.TextPosition.Center;
                    }

                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                default:
                    return NinjaTrader.NinjaScript.DrawingTools.TextPosition.Center;
            }
        }

        private string MapToUnicode(ChartIconType iconType)
        {
            return iconType switch
            {
                ChartIconType.UpArrow => "↑",       // U+2191
                ChartIconType.DownArrow => "↓",     // U+2193
                ChartIconType.Circle => "●",        // U+25CF
                ChartIconType.Square => "■",        // U+25A0
                ChartIconType.Diamond => "◆",       // U+25C6
                ChartIconType.Star => "★",         // U+2605
                ChartIconType.UpTriangle => "▲",    // U+25B2
                ChartIconType.DownTriangle => "▼",  // U+25BC
                _ => "?"                            // fallback symbol
            };
        }

        public static DashStyleHelper MapToDashStyleHelper(LineStyle style)
        {
            return style switch
            {
                LineStyle.Solid => DashStyleHelper.Solid,
                LineStyle.Dots => DashStyleHelper.Dot,
                LineStyle.DotsRare => DashStyleHelper.Dot,         // No spacing control in DashStyleHelper
                LineStyle.DotsVeryRare => DashStyleHelper.Dot,     // Map to closest approximation
                LineStyle.LinesDots => DashStyleHelper.DashDot,
                LineStyle.Lines => DashStyleHelper.Dash,
                _ => DashStyleHelper.Solid
            };
        }
    }
}
