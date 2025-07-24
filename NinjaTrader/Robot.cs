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

using cAlgo.API.Internals;
using NinjaTrader.Cbi;
using NinjaTrader.Core;
using NinjaTrader.CQG.ProtoBuf;
using NinjaTrader.Data;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.NinjaScript;
using RobotLib;
using RobotLib.Cs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml.Serialization;
using TdsCommons;
using static TdsDefs;

namespace cAlgo.API
{
    #region Delegates
    public delegate void TickHandler();
    public delegate void BarHandler();
    #endregion

    public class Robot : StrategyRenderBase
    {
        #region Members
        [XmlIgnore] public Symbol Symbol;
        // Bars are in AbstractRobot.cs as IQcBars QcBars
        // because they are needed by both platforms now
        [XmlIgnore] public Symbols Symbols;
        [XmlIgnore] public new Account Account;
        [XmlIgnore] public new Positions Positions;
        [XmlIgnore] public PendingOrders PendingOrders;
        [XmlIgnore] public History History;
        [XmlIgnore] public Chart Chart;
        [XmlIgnore] public MarketData MarketData;
        [XmlIgnore] public RunningMode RunningMode;
        [XmlIgnore] public double CommissionPerQuantity;
        [XmlIgnore] public TimeZoneInfo PlatformTimeZoneInfo;
        [XmlIgnore] public MarketDataEventArgs MarketDataEventArgs;
        [XmlIgnore] public AbstractRobot AbstractRobot;
        [XmlIgnore]
        public Dictionary<string, string> Icm2Pepper = new()  // ICM ==> Pepperstone symbol convert
        {
            {"STOXX50", "EUSTX50"},
            {"F40", "FRA40"},
            {"DE40", "GER40"},
            {"JP225", "JPN225"},
            {"ES35", "SPA35"},
            {"USTEC", "NAS100"},
            {"TecDE30", "GERTEC30"},
            {"XBRUSD", "SpotBrent"},
            {"XTIUSD", "SpotCrude"},
            {"XNGUSD", "NatGas"}
        };
        [XmlIgnore]
        public Dictionary<(int, string), NtQcBars> BarsDictionary
            = new Dictionary<(int, string), NtQcBars>();

        [Browsable(false)]
        [XmlIgnore]
        public new DateTime Time =>
            IsTickReplay ? (null == MarketDataEventArgs ? CoFu.TimeInvalid : MarketDataEventArgs.Time) :
            Times[0][0];    // Nt primary data series is used as cTrader data series
        [Browsable(false)][XmlIgnore] public bool IsBacktesting => RunningMode != RunningMode.RealTime;

        private bool mDoTerminate;
        private bool mDoStart;
        private CSRobotFactory mRobotFactory;
        private bool mIsStopped;
        private double mPrefProfit;
        #endregion

        #region Start
        protected void InitDataSeries()
        {
            // Generate NinjaTrader bid and ask data series as pendants of requested cTrader NtQcBars
            int count = 0;
            foreach (KeyValuePair<(int, string), NtQcBars> kvp in BarsDictionary)
            {
                if (0 == kvp.Value.BarsPeriod.Value)
                    throw new Exception($"Error: Bars Period Value may not be 0");

                // skip primary data series
                if (0 == count)
                {
                    AbstractRobot.QcBars = kvp.Value;   // Set default bars

                    // Without TickReplay primary data series must be set to Bid
                    // then we need a companion Ask data series
                    if (!IsTickReplay)
                        AddDataSeries(kvp.Value.SymbolName,
                            kvp.Value.BarsPeriod.BarsPeriodType,
                            kvp.Value.BarsPeriod.Value,
                            MarketDataType.Ask);
                }
                else
                {
                    if (IsTickReplay)
                        // only Last MarketDataType possible with TickReplay
                        AddDataSeries(kvp.Value.SymbolName,
                            kvp.Value.BarsPeriod.BarsPeriodType,
                            kvp.Value.BarsPeriod.Value,
                            MarketDataType.Last);
                    else
                    {
                        AddDataSeries(kvp.Value.SymbolName,
                            kvp.Value.BarsPeriod.BarsPeriodType,
                            kvp.Value.BarsPeriod.Value,
                            MarketDataType.Bid);

                        AddDataSeries(kvp.Value.SymbolName,
                            kvp.Value.BarsPeriod.BarsPeriodType,
                            kvp.Value.BarsPeriod.Value,
                            MarketDataType.Ask);
                    }
                }
                count++;
            }
        }
        #endregion

        #region Overrides
        protected override void OnStateChange()
        {
            switch (State)
            {
                case State.SetDefaults:
                {
                    // The primary data series must be set to Bid and the wished data rate, i.e. 1 second
                    // The bars timeframe is set in StrategySeriesBarsSeconds as a multiple of the data rate in primary data series
                    // Caution: The Times[0] is the open time of the new bar!!!
                    // NinjaTrader is such a piece of Sh...t!!!

                    Calculate = Calculate.OnEachTick;

                    //IsExitOnSessionCloseStrategy = false; // is controlled by "Exit on session close on GUI"
                    //ExitOnSessionCloseSeconds = 30;

                    EntriesPerDirection = 1;
                    EntryHandling = EntryHandling.AllEntries;
                    IsFillLimitOnTouch = false;
                    MaximumBarsLookBack = MaximumBarsLookBack.TwoHundredFiftySix;
                    OrderFillResolution = OrderFillResolution.High;
                    Slippage = 0;
                    StartBehavior = StartBehavior.WaitUntilFlat;
                    TimeInForce = TimeInForce.Gtc;
                    TraceOrders = false;
                    RealtimeErrorHandling = RealtimeErrorHandling.StopCancelClose;
                    StopTargetHandling = StopTargetHandling.PerEntryExecution;
                    // Disable this property for performance gains in Strategy Analyzer optimizations
                    // See the Help Guide for additional information
                    IsInstantiatedOnEachOptimizationIteration = true;
                    IsUnmanaged = false;

                    OnSetDefaults();    // Call user's bot
                }
                break;

                case State.Configure:
                {
                    if (GetDebugLaunch())
                        Debugger.Launch();

                    #region Init
                    mDoTerminate = mDoStart = true;
                    mRobotFactory = new CSRobotFactory();
                    AbstractRobot = mRobotFactory.CreateRobot();
                    PlatformTimeZoneInfo = Globals.GeneralOptions.TimeZoneInfo;

                    Symbols = new Symbols(this);
                    Account = new Account(this);
                    Positions = new Positions(this);
                    PendingOrders = new PendingOrders(this);
                    History = new History(this);
                    Chart = new Chart(this);

                    var dataRateSeconds = BarsPeriod.Value;
                    if (BarsPeriod.BarsPeriodType == BarsPeriodType.Minute)
                        dataRateSeconds *= SEC_PER_MINUTE;
                    else if (BarsPeriod.BarsPeriodType == BarsPeriodType.Day)
                        dataRateSeconds *= SEC_PER_DAY;

                    AbstractRobot.ConfigInit(this); // set time zone, open/close callbacks, etc.

                    // Set default QcBars and Symbol as pendant of NinjaTrader primary data series
                    var timeframe = AbstractRobot.Secs2Tf(dataRateSeconds, out _);
                    AbstractRobot.QcBars = AbstractRobot.GetQcBars(timeframe,
                        Instrument.FullName,
                        Instrument.FullName);
                    Symbol = Symbols.GetSymbol(Instrument.FullName);

                    // New NtQcBars and Symbols can and must be added here
                    OnConfigure();      // Call user's bot init 1st time OnConfigure
                    InitDataSeries();   // Add NtQcDataSeries as reqested GetQcBars()
                    #endregion
                }
                break;

                case State.DataLoaded:
                {
                    if (IsTickReplay)
                        Debug.Assert(BarsArray.Length == BarsDictionary.Count,
                            "Error: Number of BarsArray does not match number of MarketData.BarsDictionary");
                    else
                    {
                        Debug.Assert(BarsArray.Length == 2 * BarsDictionary.Count,
                            "Error: Number of BarsArray does not match number of MarketData.BarsDictionary");

                        MarketDataEventArgs = new MarketDataEventArgs();
                    }

                    // Init bars and their series 
                    foreach (var bars in BarsDictionary)
                        bars.Value.OnBarsDataLoaded();
                }
                break;

                case State.Historical:
                {
                    if (ChartControl != null)
                        RunningMode = RunningMode.VisualBacktesting;
                    else
                        RunningMode = RunningMode.SilentBacktesting;
                }
                break;

                case State.Realtime:
                {
                    RunningMode = RunningMode.RealTime;
                }
                break;

                case State.Terminated:
                DoStop();
                break;
            }
        }

        private void DoStop()
        {
            if (mDoTerminate)
            {
                try
                {
                    OnStop(); // Call user's bot
                }
                catch (Exception)
                {
                    var error = new Error()
                    {
                        Code = ErrorCode.TechnicalError
                    };
                    OnError(error);
                }
                mDoTerminate = false;
                Print("Done\n");
            }
        }

        protected override void OnMarketData(MarketDataEventArgs args)
        {
            // Realtime trading or Tick Replay must be active for OnBarsMarketData() to get called
            // OnBarsMarketData() is not called in backtests unless Tick Replay is enabled
            // Only Last can and must be used in Data Series when Tick Replay is enabled
            if (BarsInProgress != 0
                    || args.Bid <= 0
                    || args.Ask <= 0
                    || mIsStopped
                    || (IsTickReplay && MarketDataType.Last != args.MarketDataType))
                return;

            // ToDo: Put into symbol corresponding with args.Instrument
            // when having several symbols
            MarketDataEventArgs = args;

            // we have to postpone OnStart etc, til here because earlier we do not have a valid Time
            if (CurrentBar >= 0)
            {
                try
                {
                    // Update the bars with the new market data
                    foreach (var bar in BarsDictionary)
                        bar.Value.OnBarsMarketData();

                    if (mDoStart)
                    {
                        OnStart();  // Call user's bot OnStart
                        mDoStart = false;
                    }

                    // Call user bot
                    AbstractRobot.PreTick();
                    OnTick();
                    AbstractRobot.PostTick();
                }
                catch (Exception)
                {
                    var error = new Error()
                    {
                        Code = ErrorCode.TechnicalError
                    };
                    OnError(error);
                }
            }
        }

        protected override void OnBarUpdate()
        {
            // Only process the primary data series
            if (CurrentBar <= 0
                || mIsStopped
                || BarsInProgress != 0
                || IsTickReplay)
                return;

            try
            {
                // we have to postpone OnStart until here because earlier we do not have a valid Time
                if (mDoStart)
                {
                    OnStart();  // Call user's bot init 2nd time OnStart
                    mDoStart = false;
                }

                AbstractRobot.PreTick();
                OnTick();
                AbstractRobot.PostTick();
            }
            catch (Exception)
            {
                var error = new Error()
                {
                    Code = ErrorCode.TechnicalError
                };
                OnError(error);
            }
        }

        // here we dispatch OrderState.Filled and raise the corresponding events
        // OnExecutionUpdate gives us IsEntry, IsExit, IsEntryStrategy, IsExitStrategy, 
        // OnOrderUpdate does not
        protected override void OnExecutionUpdate(
                    Execution execution,
                    string executionId,
                    double price,
                    int quantity,
                    MarketPosition marketPosition,
                    string orderId,
                    DateTime time)
        {
            // Only consider filled executions
            if (execution.Order == null || execution.Order.OrderState != OrderState.Filled)
                return;

            if (execution.IsEntry || execution.IsEntryStrategy)
            {
                Position position = null;
                if (execution.Order.IsLimit)
                {
                    var pendingOrder = PendingOrders.Where(p => GetSignal(p.Label, p.Comment)
                        == execution.Order.Name).FirstOrDefault();
                    position = new Position(AbstractRobot, execution.Order)
                    {
                        EntryTime = Time,
                        Label = pendingOrder.Label,
                        Comment = pendingOrder.Comment,
                        StopLoss = pendingOrder.StopLoss,
                        TakeProfit = pendingOrder.TakeProfit
                    };

                    PendingOrders.RaiseFilled(new PendingOrderFilledEventArgs(pendingOrder, position));
                    PendingOrders.Remove(pendingOrder);
                    Positions.Add(position);
                }

                if (execution.Order.IsMarket)
                {
                    position = Positions.Where(p => GetSignal(p.Label, p.Comment) == execution.Order.Name
                        || GetSignal(p.Label, p.Comment) == execution.Order.FromEntrySignal).FirstOrDefault();
                }

                // From Ninja website:
                // Assign entryOrder in OnOrderUpdate() to ensure the assignment occurs when expected.
                // This is more reliable than assigning NinjaOrder objects in OnBarUpdate, as the assignment
                // is not gauranteed to be complete if it is referenced immediately after submitting
                position.NinjaOrder = execution.Order;
                Positions.RaiseOpened(new PositionOpenedEventArgs(position));
            }
            else if (execution.IsExit || execution.IsExitStrategy)
            {
                // Automatically closed positions have special text in Label like "Stop Loss" 
                // then the signal is in FromEntrySignal
                var position = Positions.Where(p => GetSignal(p.Label, p.Comment) == execution.Order.Name
                    || GetSignal(p.Label, p.Comment) == execution.Order.FromEntrySignal).FirstOrDefault();
                if (null != position)
                {
                    Positions.RaiseClosed(new PositionClosedEventArgs(position, PositionCloseReason.Closed));
                    Positions.Remove(position);
                }
            }
        }

        // here we dispatch limit orders and raise the corresponding events
        protected override void OnOrderUpdate(
            NinjaTrader.Cbi.Order order,
            double limitPrice,
            double stopPrice,
            int quantity,
            int filled,
            double averageFillPrice,
            OrderState orderState,
            DateTime time,
            NinjaTrader.Cbi.ErrorCode error,
            string comment)
        {
            // Ignore these
            if (OrderState.Submitted == orderState
                    || OrderState.Accepted == orderState
                    || OrderState.CancelPending == orderState
                    || OrderState.CancelSubmitted == orderState
                )
                return;

            if (order.IsLimit)
            {
                // Test if it is an opening pending order
                var pendingOrder = PendingOrders.Where(p => GetSignal(p.Label, p.Comment) == order.Name
                    || GetSignal(p.Label, p.Comment) == order.FromEntrySignal).FirstOrDefault();
                if (null != pendingOrder)
                {
                    // From Ninja website:
                    // Assign entryOrder in OnOrderUpdate() to ensure the assignment occurs when expected.
                    // This is more reliable than assigning NinjaOrder objects in OnBarUpdate, as the assignment
                    // is not gauranteed to be complete if it is referenced immediately after submitting
                    pendingOrder.NinjaOrder = order;

                    switch (orderState)
                    {
                        // Limit order is active and waiting for the limit price to be reached
                        case OrderState.Working:
                        PendingOrders.RaiseCreated(new PendingOrderCreatedEventArgs(pendingOrder));
                        break;

                        // Limit order has been cancelled
                        case OrderState.Cancelled:
                        PendingOrders.RaiseCancelled(new PendingOrderCancelledEventArgs(pendingOrder,
                            PendingOrderCancellationReason.Cancelled));
                        PendingOrders.Remove(pendingOrder);
                        break;

                        // Filled is handled via OnExecutionUpdate()
                    }
                    return;
                }
#if false
                // Test if it is a closing pending order
                if (OrderState.Filled == orderState)
                {
                    var position = Positions.Where(p => GetSignal(p.Label, p.Comment) == order.Name
                        || GetSignal(p.Label, p.Comment) == order.FromEntrySignal).FirstOrDefault();
                    if (null != position)
                    {
                        Positions.RaiseClosed(new PositionClosedEventArgs(position, PositionCloseReason.Closed));
                        Positions.Remove(position);
                    }
                }
#endif
            }
        }
        #endregion

        #region Methods
        public TradeResult ExecuteMarketOrder(TradeType tradeType,
            string symbolName,
            double volume,
            string label,
            double? stopLossPrice,
            double? takeProfitPrice,
            string comment)
        {
            NinjaTrader.Cbi.Order order = null;
            var botSymbol = Symbols.GetSymbol(symbolName);
            var signal = GetSignal(label, comment);
            var isLong = tradeType == TradeType.Buy;

            var position = new Position(AbstractRobot, order)
            {
                EntryTime = Time,
                Label = label,
                Comment = comment,
                Swap = 0,
                StopLoss = stopLossPrice,
                TakeProfit = takeProfitPrice,
                Pips = 0,
                Margin = 0,
            };

            Positions.Add(position);

            // NinjaTrader wants to have SL and TP set BEFORE EnterXxxLimit()
            position.StopLoss = stopLossPrice;
            position.TakeProfit = takeProfitPrice;
            if ((null != stopLossPrice && 0 != stopLossPrice)
                    || (null != takeProfitPrice && 0 != takeProfitPrice))
                SetSlTp(isLong,
                    botSymbol,
                    signal,
                    stopLossPrice,
                    takeProfitPrice,
                    ProtectionType.Absolute);

            order = isLong
                ? EnterLong((int)volume, signal)
                : EnterShort((int)volume, signal);

            return new TradeResult() { Position = position, IsSuccessful = true };
        }

        public TradeResult PlaceLimitOrder(TradeType tradeType,
            string symbolName,
            double volume,
            double limitPrice,
            string label,
            double? stopLossPrice,
            double? takeProfitPrice,
            ProtectionType protectionType,
            DateTime? expiration,
            string comment)
        {
            NinjaTrader.Cbi.Order order = null;
            var botSymbol = Symbols.GetSymbol(symbolName);
            var signal = GetSignal(label, comment);
            var isLong = tradeType == TradeType.Buy;
            var currentClosePrice = isLong ? botSymbol.Bid : botSymbol.Ask;

            // NinjaTrader wants to have SL and TP set BEFORE EnterXxxLimit()
            if ((null != stopLossPrice && 0 != stopLossPrice)
                    || (null != takeProfitPrice && 0 != takeProfitPrice))
                SetSlTp(isLong,
                    botSymbol,
                    signal,
                    stopLossPrice,
                    takeProfitPrice,
                    ProtectionType.Absolute);

            // The limit order lives til end of day or til canceled
            order = isLong
                ? EnterLongLimit(botSymbol.SymbolBarIndex, true, (int)volume, limitPrice, signal)
                : EnterShortLimit(botSymbol.SymbolBarIndex, true, (int)volume, limitPrice, signal);

            if (null != order)
            {
                // In OnOrderUpdate() EnterXxxLimit() generates Submitted => Accepted => Working
                // and then after the limit price is reached: Filled
                // "Working" means the order is pending and waiting for the limit price to be reached
                // We must add the order to the PendingOrders list BEFORE we call EnterXxxLimit()
                // so that we can handle it in OnOrderUpdate()
                var pendingOrder = new PendingOrder(this, order)
                {
                    // TradeType => NinjaOrder.IsLong ? TradeType.Buy : TradeType.Sell;
                    Comment = comment,
                    Label = label,
                    StopLoss = stopLossPrice,
                    StopLossPips = null,
                    TakeProfit = takeProfitPrice,
                    TakeProfitPips = null,
                    HasTrailingStop = false,
                    OrderType = PendingOrderType.Limit,
                    StopOrderTriggerMethod = null,
                    StopLimitRangePips = null,
                    StopLossTriggerMethod = null,
                    TargetPrice = limitPrice,
                    ExpirationTime = expiration,
                    Symbol = botSymbol,
                };

                PendingOrders.Add(pendingOrder);
            }

            return new TradeResult() { IsSuccessful = null != order };
        }

        public TradeResult ModifyPosition(Position position,
            double? stopLoss,
            double? takeProfit,
            ProtectionType? protectionType)
        {
            if (ProtectionType.Absolute != protectionType)
            {
                // Calculate absolute prices for stop loss and take profit
            }

            if (position.StopLoss == stopLoss)
                stopLoss = null; // no change
            else
                position.StopLoss = stopLoss;

            if (position.TakeProfit == takeProfit)
                takeProfit = null; // no change
            else
                position.TakeProfit = takeProfit;

            return SetSlTp(position.TradeType == TradeType.Buy,
                    position.Symbol,
                    GetSignal(position.Label, position.Comment),
                    stopLoss,
                    takeProfit,
                    protectionType);
        }

        public TradeResult SetSlTp(bool isLong,
            Symbol symbol,
            string signal,
            double? stopLossPrice,
            double? takeProfitPrice,
            ProtectionType? protection)
        {
            var currentClosePrice = isLong ? symbol.Bid : symbol.Ask;

            if (null != stopLossPrice && 0 != stopLossPrice)
                // Use simulated stop loss
                //SetStopLoss(signal, CalculationMode.Price, (double)stopLossPrice, true);
                SetStopLoss(signal, CalculationMode.Price, (double)stopLossPrice, false);

            if (null != takeProfitPrice && 0 != takeProfitPrice)
                SetProfitTarget(signal, CalculationMode.Price, (double)takeProfitPrice);

            return new TradeResult() { IsSuccessful = true };
        }

        // Summary:
        //     Close a position
        //
        // Parameters:
        //   position:
        //     Position to close
        //
        // Returns:
        //     Trade Result
        public TradeResult ClosePosition(Position position)
        {
            return ClosePosition(position, position.VolumeInUnits);
        }

        //
        // Summary:
        //     Close a position
        //
        // Parameters:
        //   position:
        //     Position to close
        //
        //   volume:
        //     Volume which is closed
        //
        // Returns:
        //     Trade Result
        public TradeResult ClosePosition(Position position, double volume)
        {
            Account.Balance += position.GrossProfit;

            var histPos = new HistoricalTrade()
            {
                ClosingTime = Time,
                Symbol = position.Symbol,
                SymbolName = position.SymbolName,
                TradeType = position.TradeType,
                VolumeInUnits = position.VolumeInUnits,
                EntryPrice = position.EntryPrice,
                ClosingPrice = position.CurrentPrice,
                EntryTime = position.EntryTime,
                GrossProfit = position.GrossProfit,
                NetProfit = position.NetProfit,
                Pips = position.Pips,
                Swap = position.Swap,
                Commissions = position.Commissions,
                Quantity = position.Quantity,
                Comment = position.Comment,
                Label = position.Label,
                Balance = -1,
                ClosingDealId = -1,
                PositionId = -1
            };
            History.Add(histPos);

            var signal = GetSignal(position.Label, position.Comment);
            NinjaTrader.Cbi.Order order = null;
            if (position.TradeType == TradeType.Buy)
            {
                if (position.GrossProfit > 0)
                    order = ExitLongLimit(position.Symbol.SymbolBarIndex, true, (int)volume, position.CurrentPrice, signal, signal);
                else
                    order = ExitLong(position.Symbol.SymbolBarIndex, (int)volume, signal, signal);
            }
            else
            {
                if (position.GrossProfit > 0)
                    order = ExitShortLimit(position.Symbol.SymbolBarIndex, true, (int)volume, position.CurrentPrice, signal, signal);
                else
                    order = ExitShort(position.Symbol.SymbolBarIndex, (int)volume, signal, signal);
            }

            return new TradeResult() { IsSuccessful = true };
        }

        // Since NinjaTrader does not support separate label and comment,
        // we add the via a '|' separator and use this as a signal name
        internal string GetSignal(string label, string comment)
        {
            return label + "|" + comment;
        }

        public void Stop()
        {
            mIsStopped = true;
            DoStop();
            //CloseStrategy("Stop() called");
            throw new Exception("Stop() called, strategy stopped");
        }

        // Must be ovverridden in derived bot class
        protected virtual void OnSetDefaults() { }
        protected virtual void OnConfigure() { }
        protected virtual void OnStart() { }
        protected virtual bool GetDebugLaunch() { return false; }
        protected virtual void OnTick() { }
        protected virtual void OnBar() { }
        protected virtual void OnStop() { }
        protected virtual double GetFitness(GetFitnessArgs args) { return 0; }
        protected virtual void OnError(Error error) { }
        #endregion
    }
}
