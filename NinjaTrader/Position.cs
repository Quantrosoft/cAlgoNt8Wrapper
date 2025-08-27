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
using NinjaTrader.NinjaScript;
using RobotLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NinjaTrader.NinjaScript.Strategies
{
    //
    // Summary:
    //     Taking or opening a position means buying or selling a trading pair.
    public class Position
    {
        public Order NinjaOrder;
        private AbstractRobot mAbstractRobot;

        public Position(AbstractRobot robot, Order order)
        {
            mAbstractRobot = robot;
            NinjaOrder = order;
        }

        //
        // Summary:
        //     Gets the symbol name.
        public string SymbolName => NinjaOrder.Instrument.FullName;

        //
        // Summary:
        //     Trade type (Buy/Sell) of the position.
        public TradeType TradeType => NinjaOrder.IsLong ? TradeType.Buy : TradeType.Sell;

        //
        // Summary:
        //     The amount traded by the position.
        public double VolumeInUnits => NinjaOrder.Quantity;

        //
        // Summary:
        //     The position's unique identifier.
        public int Id => NinjaOrder.OrderId.GetHashCode();

        //
        // Summary:
        //     Entry price of the position.
        public double EntryPrice => NinjaOrder.AverageFillPrice;

        //
        // Summary:
        //     The Stop Loss level of the position.
        // NinjaOrder.StopPrice cannot be used here sind it is NOT Stop Loss
        // price but stop price similar to limit price
        public double? StopLoss { get; internal set; }

        //
        // Summary:
        //     The take profit level of the position.
        public double? TakeProfit { get; internal set; }

        //
        // Summary:
        //      CalculationMode of the protection levels (StopLoss and TakeProfit).
        public CalculationMode CalculationMode { get; internal set; }

        //
        // Summary:
        //     Gross profit accrued by the order associated with the position.
        public double GrossProfit
        {
            get
            {
                if (NinjaOrder == null)
                    return 0;

                Trade trade = null;
                if ((NinjaOrder.OrderState == OrderState.Filled
                            || NinjaOrder.OrderState == OrderState.Cancelled
                            || NinjaOrder.OrderState == OrderState.Rejected)
                        && null != (trade = mAbstractRobot.mRobot.SystemPerformance.AllTrades
                            .LastOrDefault(t => t.Entry.Order != null && t.Entry.Order == NinjaOrder)))
                    return trade.ProfitCurrency;
                else
                    return mAbstractRobot.mRobot.Position.GetUnrealizedProfitLoss(
                        PerformanceUnit.Currency,
                        NinjaOrder.IsLong ? Symbol.Bid : Symbol.Ask);
            }
        }

        //
        // Summary:
        //     The Net profit of the position.
        public double NetProfit
        {
            get
            {
                return GrossProfit + Commissions + Swap;
            }
        }

        //
        // Summary:
        //     Swap is the overnight interest rate if any, accrued on the position.
        public double Swap;

        //
        // Summary:
        //     Commission Amount of the request to trade one way (Buy/Sell) associated with
        //     this position.
        public double Commissions
        {
            get
            {
                Trade trade = null;
                if (null != NinjaOrder)
                    if ((NinjaOrder.OrderState == OrderState.Filled
                                || NinjaOrder.OrderState == OrderState.Cancelled
                                || NinjaOrder.OrderState == OrderState.Rejected)
                            && null != (trade = mAbstractRobot.mRobot.SystemPerformance.AllTrades
                                .LastOrDefault(t => t.Entry.Order != null && t.Entry.Order == NinjaOrder)))
                        return trade.Commission;

                return 0;
            }
        }

        //
        // Summary:
        //     Entry time of trade associated with the position. The Timezone used is set in
        //     the cBot attribute.
        // NT's NinjaOrder entry time is start of new bar time, so we need to set it manually

        // NinjaTrader uses open time of the bar as entry time, so we need to set it with current time
        //public DateTime EntryTime => NinjaOrder.Time;
        public DateTime EntryTime { get; internal set; }

        //
        // Summary:
        //     Represents the winning or loosing pips of the position.
        public double Pips;

        //
        // Summary:
        //     Label can be used to represent the order.
        public string Label { get; internal set; }

        //
        // Summary:
        //     Comment can be used as a note for the order.
        public string Comment;

        //
        // Summary:
        //     Quantity of lots traded by the position.
        public double Quantity => NinjaOrder.Quantity;

        //
        // Summary:
        //     When HasTrailingStop set to true, the server updates the Stop Loss every time
        //     the position moves in your favor.
        public bool HasTrailingStop => NinjaOrder.Stopwatch.IsRunning;

        //
        // Summary:
        //     Trigger method for the position's Stop Loss.
        //StopTriggerMethod? StopLossTriggerMethod;

        //
        // Summary:
        //     The amount of used margin by the position.
        public double Margin;

        //
        // Summary:
        //     Gets the position current market price. If Position's TradeType is Buy it returns
        //     Symbol current Bid price. If position's TradeType is Sell it returns Symbol current
        //     Ask price.
        public double CurrentPrice
        {
            get
            {
                return 0 == ClosePrice
                    ? NinjaOrder.IsLong
                        ? Symbol.Bid
                        : Symbol.Ask
                    : ClosePrice;
            }
        }

        // Fake CurrentPrice for closed position since in NinjaTrader 
        // at the close Bid/Ask from OnMarketData are not the close price ?!
        internal double ClosePrice;

        //
        // Summary:
        //     Gets the position symbol.
        public Symbol Symbol => mAbstractRobot.mRobot.Symbols.GetSymbol(SymbolName);

        //
        // Summary:
        //     Shortcut for Robot.ModifyPosition method to change the Stop Loss.
        //
        // Parameters:
        //   stopLoss:
        //     New Stop Loss price
        //TradeResult ModifyStopLossPrice(double? stopLoss);

        //
        // Summary:
        //     Shortcut for Robot.ModifyPosition method to change the Take Profit.
        //
        // Parameters:
        //   takeProfit:
        //     New Take Profit price
        //TradeResult ModifyTakeProfitPrice(double? takeProfit);

        //
        // Summary:
        //     Shortcut for the Robot.ModifyPosition method to change the Stop Loss pips
        //
        // Parameters:
        //   stopLossPrice:
        //     New Stop Loss in Pips
        //TradeResult ModifyStopLossPips(double? stopLossPrice);

        //
        // Summary:
        //     Shortcut for the Robot.ModifyPosition method to change the Take Profit pips
        //
        // Parameters:
        //   takeProfitPrice:
        //     New Take Profit in Pips
        //TradeResult ModifyTakeProfitPips(double? takeProfitPrice);

        //
        // Summary:
        //     Shortcut for the Robot.ModifyPosition method to change the Trailing Stop.
        //
        // Parameters:
        //   hasTrailingStop:
        //     If true position will have trailing stop loss
        public TradeResult ModifyTrailingStop(bool hasTrailingStop)
        {
            mAbstractRobot.mRobot.SetTrailStop(NinjaOrder.Name, CalculationMode.Price, EntryPrice, true);
            return new TradeResult
            {
                IsSuccessful = true,
                Position = this
            };
        }

        //
        // Summary:
        //     Shortcut for the Robot.ModifyPosition method to change the VolumeInUnits.
        //
        // Parameters:
        //   volume:
        //     New Volume in Units
        public TradeResult ModifyVolume(double volume)
        {
            Order.ChangeQuantity(new List<Order>(), (int)volume, QuantityModificationForStocks.IncreaseQuantity);
            return new TradeResult
            {
                IsSuccessful = true,
                Position = this
            };
        }

        //
        // Summary:
        //     Shortcut for the Robot.ReversePosition method to change the direction of the
        //     trade.
        //TradeResult Reverse();

        //
        // Summary:
        //     Shortcut for the Robot.ReversePosition method to change the direction of trade
        //     and the volume.
        //
        // Parameters:
        //   volume:
        //     New Volume in Units
        //TradeResult Reverse(double volume);

        //
        // Summary:
        //     Shortcut for the Robot.ClosePosition method.
        public TradeResult Close()
        {
            return mAbstractRobot.mRobot.ClosePosition(this);
        }
    }
}
