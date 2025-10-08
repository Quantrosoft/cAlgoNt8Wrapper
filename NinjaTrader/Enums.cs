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
    public enum ProtectionType
    {
        //
        // Summary:
        //     No protection.
        None,
        //
        // Summary:
        //     Relative or distance based protection type.
        Relative,
        //
        // Summary:
        //     Absolute or price based protection type.
        Absolute
    }

    public enum TradeType
    {
        Buy,
        Sell,
        Debug
    }

    public enum RunningMode
    {
        //
        // Summary:
        //     The cBot is running in real time.
        RealTime,
        //
        // Summary:
        //     The cBot is running in the silent backtesting mode.
        SilentBacktesting,
        //
        // Summary:
        //     The cBot is running in the visual backtesting mode.
        VisualBacktesting,
        //
        // Summary:
        //     The cBot is running in the optimization mode.
        Optimization
    }

    //     An enumeration of different stroke styles used to render lines.
    public enum LineStyle
    {
        //
        // Summary:
        //     A solid line: -----
        Solid,
        //
        // Summary:
        //     A dotted line: .....
        Dots,
        //
        // Summary:
        //     A dotted line, large gap between dots: . . . .
        DotsRare,
        //
        // Summary:
        //     A dotted line, extra large gap between dots: . . . .
        DotsVeryRare,
        //
        // Summary:
        //     A mixed line / dot style is used to render the line: - . - . - .
        LinesDots,
        //
        // Summary:
        //     Lines with gaps are used to render the line: - - - -
        Lines
    }

    //     Describes vertical position related to an anchor point or a parent element.
    public enum VerticalAlignment
    {
        //
        // Summary:
        //     Center vertical alignment.
        Center,
        //
        // Summary:
        //     Top vertical alignment.
        Top,
        //
        // Summary:
        //     Bottom vertical alignment.
        Bottom,
        //
        // Summary:
        //     Stretch vertical alignment.
        Stretch
    }

    //     Describes horizontal position related to an anchor point or a parent element
    public enum HorizontalAlignment
    {
        //
        // Summary:
        //     Center horizontal alignment.
        Center,
        //
        // Summary:
        //     Left horizontal alignment.
        Left,
        //
        // Summary:
        //     Right horizontal alignment.
        Right,
        //
        // Summary:
        //     Stretch horizontal alignment.
        Stretch
    }

    //     Represents the type of the Icon.
    public enum ChartIconType
    {
        //
        // Summary:
        //     The Up Arrow icon.
        UpArrow,
        //
        // Summary:
        //     The Down Arrow icon.
        DownArrow,
        //
        // Summary:
        //     The Circle icon.
        Circle,
        //
        // Summary:
        //     The Square icon.
        Square,
        //
        // Summary:
        //     The Diamond icon.
        Diamond,
        //
        // Summary:
        //     The Star icon.
        Star,
        //
        // Summary:
        //     The Up Triangle icon.
        UpTriangle,
        //
        // Summary:
        //     The Down Triangle icon.
        DownTriangle
    }

    //     The reason for closing the position.
    public enum PositionCloseReason
    {
        //
        // Summary:
        //     Positions was closed by the trader.
        Closed,
        //
        // Summary:
        //     Position was closed by the Stop Loss.
        StopLoss,
        //
        // Summary:
        //     Position was closed by the Take Profit.
        TakeProfit,
        //
        // Summary:
        //     Position was closed because the Stop Out level reached.
        StopOut
    }

    //     The chart object types.
    public enum ChartObjectType
    {
        //
        // Summary:
        //     The horizontal line. The line parallel to the X-axis that can be set on any Y-axis
        //     value.
        HorizontalLine,
        //
        // Summary:
        //     The vertical line. The line parallel to the Y-axis that can be set on any X-axis
        //     value. used to mark certain time event or chart bar on the chart.TBD
        VerticalLine,
        //
        // Summary:
        //     The trend line. The line with the start and end points that can be drawn in any
        //     direction on the chart.
        TrendLine,
        //
        // Summary:
        //     The text that can be placed directly in the chart, bound to X-Y axises.
        Text,
        //
        // Summary:
        //     The static positioned text that can be placed on fixed locations in the chart
        StaticText,
        //
        // Summary:
        //     The icon. The collection of icons that can be placed directly in the chart, bound
        //     to X-Y axises.
        Icon,
        //
        // Summary:
        //     The Fibonacci Retracement that can be placed directly in the chart, bound to
        //     X-Y axises. - a charting technique that uses the Fibonacci ratios to indicate
        //     the areas of support or resistance.
        FibonacciRetracement,
        //
        // Summary:
        //     The Fibonacci Expansion that can be placed directly in the chart, bound to X-Y
        //     axises. - a charting technique used to plot possible levels of support and resistance
        //     by tracking not only the primary trend but also the retracement.
        FibonacciExpansion,
        //
        // Summary:
        //     The Fibonacci Fan that can be placed directly in the chart, bound to X-Y axises.
        //     a charting technique used to estimate support and resistance levels by drawing
        //     the new trend lines based on the Fibonacci Retracement levels.
        FibonacciFan,
        //
        // Summary:
        //     The Andrews Pitchfork that can be placed directly in the chart, bound to X-Y
        //     axises.
        AndrewsPitchfork,
        //
        // Summary:
        //     The rectangle of any preferable size and rotation that can be drawn directly
        //     in the chart, bound to X-Y axises.
        Rectangle,
        //
        // Summary:
        //     The ellipse of any preferable size and rotation that can be drawn directly in
        //     the chart, bound to X-Y axises.
        Ellipse,
        //
        // Summary:
        //     The triangle of any preferable size and rotation that can be drawn directly in
        //     the chart, bound to X-Y axises.
        Triangle,
        //
        // Summary:
        //     The equidistant channel that can be placed directly in the chart, bound to X-Y
        //     axises.
        EquidistantChannel,
        //
        // Summary:
        //     The user drawings in the chart with Pencil
        Drawing,
        //
        // Summary:
        //     The arrow line.
        ArrowLine,
        //
        // Summary:
        //     The Fibonacci Timezones
        FibonacciTimezones,
        //
        // Summary:
        //     The Risk Reward
        RiskReward
    }

    // Represents the type (Limit or Stop) of pending order.
    public enum PendingOrderType
    {
        /// <summary>
        ///  A limit order is an order to buy or sell at a specific price or better.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var order in PendingOrders)
        /// {
        ///     if(order.OrderType == PendingOrderType.Limit)
        ///         Print(order.Id);
        /// }
        /// </code>
        /// </example>
        Limit,
        /// <summary>
        /// A stop order is an order to buy or sell once the price of the symbol reaches a specified price.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var order in PendingOrders)
        /// {
        ///     if(order.OrderType == PendingOrderType.Stop)
        ///         Print(order.Id);
        /// }
        /// </code>
        /// </example>
        Stop,
        /// <summary>
        /// A stop limit order is an order to buy or sell once the price of the symbol reaches specific price.
        /// NinjaOrder has a parameter for maximum distance from that target price, where it can be executed.
        /// </summary>
        /// <example>
        /// <code>
        /// foreach (var order in PendingOrders)
        /// {
        ///     if(order.OrderType == PendingOrderType.StopLimit)
        ///         Print(order.Id);
        /// }
        /// </code>
        /// </example>
        StopLimit,
    }

    //     The trigger side for the Stop Orders.
    public enum StopTriggerMethod
    {
        //
        // Summary:
        //     Trade method uses default trigger behavior for Stop orders. Buy order and Stop
        //     Loss for Sell position will be triggered when Ask >= order price. Sell order
        //     and Stop Loss for Buy position will be triggered when Bid <= order price.
        Trade,
        //
        // Summary:
        //     Opposite method uses opposite price for order triggering. Buy order and Stop
        //     Loss for Sell position will be triggered when Bid >= order price. Sell order
        //     and Stop Loss for Buy position will be triggered when Ask <= order price.
        Opposite,
        //
        // Summary:
        //     Uses default prices for order triggering, but waits for additional confirmation
        //     - two consecutive prices should meet criteria to trigger order. Buy order and
        //     Stop Loss for Sell position will be triggered when two consecutive Ask prices
        //     >= order price. Sell order and Stop Loss for Buy position will be triggered when
        //     two consecutive Bid prices <= order price.
        DoubleTrade,
        //
        // Summary:
        //     Uses opposite prices for order triggering, and waits for additional confirmation
        //     - two consecutive prices should meet criteria to trigger order. Buy order and
        //     Stop Loss for Sell position will be triggered when two consecutive Bid prices
        //     >= order price. Sell order and Stop Loss for Buy position will be triggered when
        //     two consecutive Ask prices <= order price.
        DoubleOpposite
    }

    //     The reason for the order cancellation.
    public enum PendingOrderCancellationReason
    {
        //
        // Summary:
        //     THe order was cancelled by trader.
        Cancelled,
        //
        // Summary:
        //     The order was cancelled due to expiration.
        Expired,
        //
        // Summary:
        //     The order fill was rejected and the order was cancelled.
        Rejected
    }

    //     Enumeration of standard error codes.
    //
    // Remarks:
    //     Error codes are readable descriptions of the responses returned by the server.
    public enum ErrorCode
    {
        //
        // Summary:
        //     A generic technical error with a trade request.
        TechnicalError,
        //
        // Summary:
        //     The volume value is not valid
        BadVolume,
        //
        // Summary:
        //     There are not enough money in the account to trade with.
        NoMoney,
        //
        // Summary:
        //     The market is closed.
        MarketClosed,
        //
        // Summary:
        //     The server is disconnected.
        Disconnected,
        //
        // Summary:
        //     Position does not exist.
        EntityNotFound,
        //
        // Summary:
        //     Operation timed out.
        Timeout,
        //
        // Summary:
        //     Unknown symbol.
        UnknownSymbol,
        //
        // Summary:
        //     The invalid Stop Loss or Take Profit.
        InvalidStopLossTakeProfit,
        //
        // Summary:
        //     The invalid request.
        InvalidRequest,
        //
        // Summary:
        //     Occurs when accessing trading API without trading permission.
        NoTradingPermission
    }

    public enum ProfitCloseModes
    {
        TakeProfit,
        BreakEven,
        TrailingStop,
    }
}
