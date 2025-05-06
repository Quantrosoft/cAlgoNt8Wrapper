/* MIT License
Copyright (c) 2035 Quantrosoft Pty. Ltd.

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
using System.ComponentModel;

namespace cAlgo.API
{
    public interface PendingOrder
    {
        /// <summary>SymbolName code of the order</summary>
        /// <example>
        /// <code>
        /// PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,SymbolName.Bid);
        /// Print("SymbolCode = {0}", LastResult.PendingOrder.SymbolCode);
        /// </code>
        /// </example>
        [Obsolete("Use SymbolName instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        string SymbolCode { get; }

        /// <summary>Specifies whether this order is to buy or sell.</summary>
        /// <example>
        /// <code>
        /// PlaceLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice);
        /// Print(LastResult.PendingOrder.TradeType);
        /// </code>
        /// </example>
        TradeType TradeType { get; }

        /// <summary>Volume of this order.</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice);
        /// var order = result.PendingOrder;
        /// Print("The order's volume is: {0}", order.VolumeInUnits);
        ///  </code>
        /// </example>
        double VolumeInUnits { get; }

        /// <summary>Unique order Id.</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice);
        /// var order = result.PendingOrder;
        /// Print("The pending order's ID: {0}", order.Id);
        /// </code>
        /// </example>
        int Id { get; }

        /// <summary>Specifies whether this order is Stop or Limit.</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice);
        /// var order = result.PendingOrder;
        /// Print("Order type = {0}", order.OrderType);
        /// </code>
        /// </example>
        PendingOrderType OrderType { get; }

        /// <summary>The order target price.</summary>
        /// <example>
        /// <code>
        /// var targetPrice = SymbolName.Bid;
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice);
        /// </code>
        /// </example>
        double TargetPrice { get; }

        /// <summary>
        /// The order Expiration time
        /// The Timezone used is set in the Robot attribute
        /// </summary>
        /// <example>
        /// <code>
        /// DateTime expiration = Server.Time.AddMinutes(120);
        /// PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,
        ///     SymbolName.Bid, null, 10, 10, expiration);
        /// </code>
        /// </example>
        DateTime? ExpirationTime { get; }

        /// <summary>The order stop loss in price</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,
        /// SymbolName.Bid, null, 10, 10);
        /// 
        /// var order = result.PendingOrder;
        /// Print("Order SL price = {0}", order.StopLoss);
        /// </code>
        /// </example>
        double? StopLoss { get; }

        /// <summary>The order stop loss in pips</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,
        ///                     SymbolName.Bid, null, 10, 10);
        /// 
        /// var order = result.PendingOrder;
        /// Print("Order SL pips = {0}", order.StopLossPips);
        /// </code>
        /// </example>
        double? StopLossPips { get; }

        /// <summary>The order take profit in price</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,
        /// SymbolName.Bid, null, 10, 10);
        /// 
        /// var order = result.PendingOrder;
        /// Print("Order TP price = {0}", order.TakeProfit);
        /// </code>
        /// </example>
        double? TakeProfit { get; }

        /// <summary>The order take profit in pips</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,
        /// SymbolName.Bid, null, 10, 10);
        /// 
        /// var order = result.PendingOrder;
        /// Print("TP Pips = {0}", order.TakeProfitPips);
        /// </code>
        /// </example>
        double? TakeProfitPips { get; }

        /// <summary>User assigned identifier for the order.</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,
        /// SymbolName.Bid, "myLabel", 10, 10);
        /// 
        /// if(result.IsSuccessful)
        /// {
        ///     var order = result.PendingOrder;
        ///     Print("Label = {0}", order.Label);
        /// }
        /// </code>
        /// </example>
        string Label { get; }

        /// <summary>User assigned Order Comment</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,
        ///                 SymbolName.Bid, null, 10, 10, null, "this is a comment");
        /// var order = result.PendingOrder;
        /// Print("comment = {0}", order.Comment);
        /// </code>
        /// </example>
        string Comment { get; }

        /// <summary>Quantity (lots) of this order</summary>
        double Quantity { get; }

        /// <summary>
        /// When HasTrailingStop set to true,
        /// server updates Stop Loss every time position moves in your favor.
        /// </summary>
        /// <example>
        /// <code>
        /// ExecuteMarketOrder(TradeType.Buy, SymbolName, 10000, "myLabel", 10, 10, 2, "comment", true);
        /// Print("Position was opened, has Trailing Stop = {0}", result.Position.HasTrailingStop);
        /// </code>
        /// </example>
        bool HasTrailingStop { get; }

        /// <summary>Trigger method for position's StopLoss</summary>
        StopTriggerMethod? StopLossTriggerMethod { get; }

        /// <summary>
        /// Determines how pending order will be triggered in case it's a StopOrder
        /// </summary>
        StopTriggerMethod? StopOrderTriggerMethod { get; }

        /// <summary>
        /// Maximum limit from order target price, where order can be executed.
        /// </summary>
        /// <example>
        /// <code>
        /// var targetPrice = SymbolName.Ask;
        /// var result = PlaceStopLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice, 2.0);
        /// </code>
        /// </example>
        double? StopLimitRangePips { get; }

        /// <summary>Gets the symbol name.</summary>
        /// <value></value>
        /// <remarks></remarks>
        string SymbolName { get; }

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change Stop Loss
        /// </summary>
        TradeResult ModifyStopLossPips(double? stopLossPips);

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change Take Profit
        /// </summary>
        TradeResult ModifyTakeProfitPips(double? takeProfitPips);

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change Stop Limit Range
        /// </summary>
        TradeResult ModifyStopLimitRange(double stopLimitRangePips);

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change Expiration Time
        /// </summary>
        TradeResult ModifyExpirationTime(DateTime? expirationTime);

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change VolumeInUnits
        /// </summary>
        TradeResult ModifyVolume(double volume);

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change Target Price
        /// </summary>
        TradeResult ModifyTargetPrice(double targetPrice);

        /// <summary>Shortcut for Robot.CancelPendingOrder method</summary>
        /// <returns></returns>
        TradeResult Cancel();
    }
}
