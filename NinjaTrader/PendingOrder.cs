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

using NinjaTrader.Cbi;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Strategies;
using System;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class PendingOrder
    {
        internal Order NinjaOrder;
        private Strategy mRobot;
        private StrategyRenderBase mStrategy;

        public PendingOrder(Strategy robot, Order order)
        {
            mRobot = robot;     // whole robot class
            mStrategy = robot;  // just the NinjaTrader base class
            NinjaOrder = order;
        }

        /// <summary>Specifies whether this order is to buy or sell.</summary>
        /// <example>
        /// <code>
        /// PlaceLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice);
        /// Print(LastResult.PendingOrder.TradeType);
        /// </code>
        /// </example>
        public TradeType TradeType => NinjaOrder.IsLong ? TradeType.Buy : TradeType.Sell;

        /// <summary>Volume of this order.</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice);
        /// var order = result.PendingOrder;
        /// Print("The order's volume is: {0}", order.VolumeInUnits);
        ///  </code>
        /// </example>
        public double VolumeInUnits => NinjaOrder.Quantity;

        /// <summary>Unique order Id.</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice);
        /// var order = result.PendingOrder;
        /// Print("The pending order's ID: {0}", order.Id);
        /// </code>
        /// </example>
        public int Id => NinjaOrder.OrderId.GetHashCode();

        /// <summary>Specifies whether this order is Stop or Limit.</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice);
        /// var order = result.PendingOrder;
        /// Print("NinjaOrder type = {0}", order.OrderType);
        /// </code>
        /// </example>
        public PendingOrderType OrderType { get; internal set; }

        /// <summary>The order target price.</summary>
        /// <example>
        /// <code>
        /// var targetPrice = SymbolName.Bid;
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice);
        /// </code>
        /// </example>
        public double TargetPrice { get; internal set; }

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
        public DateTime? ExpirationTime { get; internal set; }

        /// <summary>The order stop loss in price</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,
        /// SymbolName.Bid, null, 10, 10);
        /// 
        /// var order = result.PendingOrder;
        /// Print("NinjaOrder SL price = {0}", order.StopLoss);
        /// </code>
        /// </example>
        public double? StopLoss { get; internal set; }

        /// <summary>The order stop loss in pips</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,
        ///                     SymbolName.Bid, null, 10, 10);
        /// 
        /// var order = result.PendingOrder;
        /// Print("NinjaOrder SL pips = {0}", order.StopLossPips);
        /// </code>
        /// </example>
        public double? StopLossPips { get; internal set; }

        /// <summary>The order take profit in price</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,
        /// SymbolName.Bid, null, 10, 10);
        /// 
        /// var order = result.PendingOrder;
        /// Print("NinjaOrder TP price = {0}", order.TakeProfit);
        /// </code>
        /// </example>
        public double? TakeProfit { get; internal set; }

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
        public double? TakeProfitPips { get; internal set; }

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
        public string Label { get; internal set; }

        /// <summary>User assigned NinjaOrder Comment</summary>
        /// <example>
        /// <code>
        /// var result = PlaceLimitOrder(TradeType.Buy, SymbolName, 10000,
        ///                 SymbolName.Bid, null, 10, 10, null, "this is a comment");
        /// var order = result.PendingOrder;
        /// Print("comment = {0}", order.Comment);
        /// </code>
        /// </example>
        public string Comment { get; internal set; }

        /// <summary>Quantity (lots) of this order</summary>
        public double Quantity => NinjaOrder.Quantity;

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
        public bool HasTrailingStop { get; internal set; }

        /// <summary>Trigger method for position's StopLoss</summary>
        public StopTriggerMethod? StopLossTriggerMethod { get; internal set; }

        /// <summary>
        /// Determines how pending order will be triggered in case it's a StopOrder
        /// </summary>
        public StopTriggerMethod? StopOrderTriggerMethod { get; internal set; }

        /// <summary>
        /// Maximum limit from order target price, where order can be executed.
        /// </summary>
        /// <example>
        /// <code>
        /// var targetPrice = SymbolName.Ask;
        /// var result = PlaceStopLimitOrder(TradeType.Buy, SymbolName, 10000, targetPrice, 2.0);
        /// </code>
        /// </example>
        public double? StopLimitRangePips { get; internal set; }

        /// <summary>Gets the symbol name.</summary>
        /// <value></value>
        /// <remarks></remarks>
        public string SymbolName => NinjaOrder.Instrument.FullName;

        /// <summary>
        ///     Gets the order symbol.
        /// </summary>
        public Symbol Symbol { get; internal set; }

        //
        // Summary:
        //     Gets the order submission time.
        public DateTime SubmittedTime => NinjaOrder.Time;

        //
        // Summary:
        //      CalculationMode of the protection levels (StopLoss and TakeProfit).
        public CalculationMode CalculationMode { get; internal set; }

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change Stop Loss
        /// </summary>
        public TradeResult ModifyStopLossPips(double? stopLossPrice)
        {
            return null;
        }

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change Take Profit
        /// </summary>
        public TradeResult ModifyTakeProfitPips(double? takeProfitPrice)
        {
            return null;
        }

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change Stop Limit Range
        /// </summary>
        public TradeResult ModifyStopLimitRange(double stopLimitRangePips)
        {
            return null;
        }

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change Expiration Time
        /// </summary>
        public TradeResult ModifyExpirationTime(DateTime? expirationTime)
        {
            return null;
        }

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change VolumeInUnits
        /// </summary>
        public TradeResult ModifyVolume(double volume)
        {
            return null;
        }

        /// <summary>
        /// Shortcut for Robot.ModifyPendingOrder method to change Target Price
        /// </summary>
        public TradeResult ModifyTargetPrice(double targetPrice)
        {
            return null;
        }

        /// <summary>Shortcut for Robot.CancelPendingOrder method</summary>
        /// <returns></returns>
        public TradeResult Cancel()
        {
            mStrategy.CancelOrder(NinjaOrder);
            return new TradeResult()
            {
                IsSuccessful = true,
                Position = null,
                PendingOrder = this
            };
        }
    }
}
