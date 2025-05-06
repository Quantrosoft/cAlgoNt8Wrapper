//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using cAlgo.API.Internals;
using NinjaTrader.Cbi;
using NTRes.NinjaTrader.Gui.Tools.Account;

namespace cAlgo.API
{
    //
    // Summary:
    //     Taking or opening a position means buying or selling a trading pair.
    public class Position
    {
        public Order Order;
        private Robot mRobot;

        public Position(Robot robot, Order order)
        {
            mRobot = robot;
            Order = order;
        }

        //
        // Summary:
        //     Gets the symbol name.
        public string SymbolName => Order.Instrument.FullName;

        //
        // Summary:
        //     Trade type (Buy/Sell) of the position.
        public TradeType TradeType => Order.IsLong
            ? TradeType.Buy
            : TradeType.Sell;

        //
        // Summary:
        //     The amount traded by the position.
        public double VolumeInUnits => Order.Quantity;

        //
        // Summary:
        //     The position's unique identifier.
        public int Id => Order.OrderId.GetHashCode();

        //
        // Summary:
        //     Entry price of the position.
        public double EntryPrice => Order.AverageFillPrice;

        //
        // Summary:
        //     The Stop Loss level of the position.
        public double? StopLoss;

        //
        // Summary:
        //     The take profit level of the position.
        public double? TakeProfit;

        //
        // Summary:
        //     Gross profit accrued by the order associated with the position.
        public double GrossProfit
        {
            get
            {
                if (mRobot.Position.MarketPosition == MarketPosition.Flat)
                    return mLastGrossProfit;
                else
                    return mLastGrossProfit = mRobot.Position.GetUnrealizedProfitLoss(
                        PerformanceUnit.Currency,
                        Order.IsLong
                            ? Symbol.Bid
                            : Symbol.Ask);
            }
        }
        //
        // Summary:
        //     The Net profit of the position.
        public double NetProfit
        {
            get
            {
                if (mRobot.Position.MarketPosition == MarketPosition.Flat)
                    return mLastNetProfit;
                else
                    return mLastNetProfit = mRobot.Position.GetUnrealizedProfitLoss(
                        PerformanceUnit.Currency,
                        Order.IsLong
                            ? Symbol.Bid
                            : Symbol.Ask);
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
        public double Commissions;

        //
        // Summary:
        //     Entry time of trade associated with the position. The Timezone used is set in
        //     the cBot attribute.
        // NT's Order entry time is start of new bar time, so we need to set it manually
        public DateTime EntryTime { get; internal set; }

        //
        // Summary:
        //     Represents the winning or loosing pips of the position.
        public double Pips;

        //
        // Summary:
        //     Label can be used to represent the order.
        public string Label;

        //
        // Summary:
        //     Comment can be used as a note for the order.
        public string Comment;

        //
        // Summary:
        //     Quantity of lots traded by the position.
        public double Quantity => Order.Quantity;

        //
        // Summary:
        //     When HasTrailingStop set to true, the server updates the Stop Loss every time
        //     the position moves in your favor.
        public bool HasTrailingStop;

        //
        // Summary:
        //     Trigger method for the position's Stop Loss.
        //StopTriggerMethod? StopLossTriggerMethod;

        //
        // Summary:
        //     The amount of used margin by the position.
        public double Margin;
        private double mLastGrossProfit;
        private double mLastNetProfit;

        //
        // Summary:
        //     Gets the position current market price. If Position's TradeType is Buy it returns
        //     Symbol current Bid price. If position's TradeType is Sell it returns Symbol current
        //     Ask price.
        public double CurrentPrice => Order.IsLong
                            ? Symbol.Bid
                            : Symbol.Ask;

        //TradeType == TradeType.Buy ? Symbol.Bid : Symbol.Ask;

        //
        // Summary:
        //     Gets the position symbol.
        public Symbol Symbol => mRobot.Symbols.GetSymbol(SymbolName);

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
        //   stopLossPips:
        //     New Stop Loss in Pips
        //TradeResult ModifyStopLossPips(double? stopLossPips);

        //
        // Summary:
        //     Shortcut for the Robot.ModifyPosition method to change the Take Profit pips
        //
        // Parameters:
        //   takeProfitPips:
        //     New Take Profit in Pips
        //TradeResult ModifyTakeProfitPips(double? takeProfitPips);

        //
        // Summary:
        //     Shortcut for the Robot.ModifyPosition method to change the Trailing Stop.
        //
        // Parameters:
        //   hasTrailingStop:
        //     If true position will have trailing stop loss
        //TradeResult ModifyTrailingStop(bool hasTrailingStop);

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
            return mRobot.ClosePosition(this);
        }
    }
}
