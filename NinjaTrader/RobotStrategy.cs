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
using NinjaTrader.Core;
using NinjaTrader.Data;
using NinjaTrader.Gui;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.Strategies.Internals;
using RobotLib;
using RobotLib.Cs;
using Rules1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using TdsCommons;
using static TdsDefs;

namespace NinjaTrader.NinjaScript.Strategies
{
    #region Delegates
    public delegate void TickHandler();
    public delegate void BarHandler();
    #endregion

    public partial class Strategy : StrategyRenderBase
    {
        #region Members
        // Bars are not here any more but in AbstractRobot.cs as IQcBars QcBars
        // because they are needed by both platforms
        [XmlIgnore, Browsable(false)] public Symbol Symbol;
        [XmlIgnore, Browsable(false)] public Symbols Symbols;
        [XmlIgnore, Browsable(false)] public new Account Account;
        [XmlIgnore, Browsable(false)] public new Positions Positions;
        [XmlIgnore, Browsable(false)] public PendingOrders PendingOrders;
        [XmlIgnore, Browsable(false)] public History History;
        [XmlIgnore, Browsable(false)] public Chart Chart;
        [XmlIgnore, Browsable(false)] public MarketData MarketData;
        [XmlIgnore, Browsable(false)] public RunningMode RunningMode;
        [XmlIgnore, Browsable(false)] public double CommissionPerQuantity;
        [XmlIgnore, Browsable(false)] public TimeZoneInfo PlatformTimeZoneInfo;
        [XmlIgnore, Browsable(false)] public MarketDataEventArgs MarketDataEventArgs;
        [XmlIgnore, Browsable(false)] public AbstractRobot AbstractRobot;
        [XmlIgnore, Browsable(false)]
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
        [XmlIgnore, Browsable(false)]
        public Dictionary<(int, string), NtQcBars> BarsDictionary
            = new Dictionary<(int, string), NtQcBars>();

        [XmlIgnore, Browsable(false)]
        public new DateTime Time =>
            IsTickReplay ? (null == MarketDataEventArgs ? CoFu.TimeInvalid : MarketDataEventArgs.Time) :
            Times[0][0];    // Nt primary data series is used as cTrader data series
        [XmlIgnore, Browsable(false)] public bool IsBacktesting => RunningMode != RunningMode.RealTime;

        [XmlIgnore, Browsable(false)] public bool IsAnalyzer => IsInStrategyAnalyzer;

        [XmlIgnore, Browsable(false)]
        public bool IsPlayback =>
            Connection.PlaybackConnection != null &&
            Connection.PlaybackConnection.Status == ConnectionStatus.Connected;

        [XmlIgnore, Browsable(false)] public bool IsRealtimeStreaming => State == State.Realtime && !IsAnalyzer;

        // Name-based check per NT guidance (Sim101 or custom sim accounts usually start with "Sim")
        [XmlIgnore, Browsable(false)] public bool IsSimAccount => Account != null && base.Account.Name.StartsWith("Sim", StringComparison.OrdinalIgnoreCase);
        [XmlIgnore, Browsable(false)] public bool IsLiveAccount => Account != null && !IsSimAccount;
        [XmlIgnore, Browsable(false)] public bool IsLiveTrading => IsRealtimeStreaming && !IsPlayback && IsLiveAccount;
        [XmlIgnore, Browsable(false)] public bool IsSimTrading => IsRealtimeStreaming && !IsPlayback && IsSimAccount;
        [XmlIgnore, Browsable(false)] public bool IsAnyBacktest => IsAnalyzer || IsPlayback;
        [XmlIgnore, Browsable(false)] public State State => base.State;

        private bool mDoTerminate;
        private bool mDoStart;
        private CSRobotFactory mRobotFactory;
        private bool mIsStopped;
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

                        if (IsAnalyzer)
                            RunningMode = RunningMode.SilentBacktesting;
                        else if (IsPlayback)
                            RunningMode = RunningMode.VisualBacktesting;
                        else
                            RunningMode = RunningMode.RealTime;


                        AbstractRobot.ConfigInit(this); // set time zone, open/close callbacks, etc.

                        if (BarsPeriod.BarsPeriodType == BarsPeriodType.Minute
                            || BarsPeriod.BarsPeriodType == BarsPeriodType.Day)
                        {
                            var dataRateSeconds = GetBarsSeconds(BarsPeriod);

                            // Set default QcBars and Symbol as pendant of NinjaTrader primary data series
                            AbstractRobot.QcBars = AbstractRobot.GetQcBars(dataRateSeconds,
                                1,
                                Instrument.FullName,
                                Instrument.FullName);
                        }
                        Symbol = Symbols.GetSymbol(Instrument.FullName);

                        // New NtQcBars and Symbols can and must be added here
                        OnConfigure();      // Call user's bot init 1st time OnConfigure
                                            // Add NtQcDataSeries as reqested GetQcBars()
                                            // Generate NinjaTrader bid and ask data series as pendants of requested cTrader NtQcBars
                        int count = 0;
                        foreach (KeyValuePair<(int, string), NtQcBars> kvp in BarsDictionary)
                        {
                            if (0 == kvp.Value.BarsPeriod.Value)
                                throw new Exception($"Error: Bars Period Value may not be 0");

                            // skip primary data series
                            if (0 == count && (BarsPeriod.BarsPeriodType == BarsPeriodType.Minute
                                    || BarsPeriod.BarsPeriodType == BarsPeriodType.Day))
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
                        #endregion
                    }
                    break;

                case State.DataLoaded:
                    {
                        if (!IsTickReplay)
                            MarketDataEventArgs = new MarketDataEventArgs();

                        // Init bars and their series 
                        foreach (var bars in BarsDictionary)
                            bars.Value.OnBarsDataLoaded();

                        OnDataLoaded();     // Call user's bot
                    }
                    break;

                case State.Historical:
                    break;

                case State.Realtime:
                    break;

                case State.Terminated:
                    DoStop();
                    break;
            }
        }

        protected override void OnMarketData(MarketDataEventArgs args)
        {
            // Realtime trading or Tick Replay must be active for OnBarsMarketData() to get called
            // OnBarsMarketData() is not called in backtests unless Tick Replay is enabled
            // Only Last can and must be used in Data Series when Tick Replay is enabled
            if (args.Bid <= 0
                    || args.Ask <= 0
                    || mIsStopped
                    || (IsTickReplay && MarketDataType.Last != args.MarketDataType)
                    || CurrentBar < 0)
                return;

            // ToDo: Put into symbol corresponding with args.Instrument when having several symbols
            MarketDataEventArgs = args;

            // we have to postpone OnStart etc, til here because earlier we do not have a valid Time
            try
            {
                // Update all bars with the new market data
                var allInized = true;
                foreach (var bars in BarsDictionary)
                {
                    if (BarsInProgress == bars.Value.BarsBarIndex)
                        bars.Value.OnBarsMarketData();

                    if (0 == bars.Value.Count)
                        allInized = false;
                }

                // Wait for all bars to be updated and call user 's bot only once on the last bars update
                if (allInized && BarsInProgress == BarsArray.Length - 1)
                {
                    // Must user's bot OnStart here since we do not have a valid Time before
                    if (mDoStart)
                    {
                        OnStart();
                        mDoStart = false;
                    }
                    else
                    {
                        AbstractRobot.PreTick();
                        OnTick();
                        AbstractRobot.PostTick();
                    }

                    // Reset IsNewBar for all bars after bot has handled this tick
                    foreach (var bars in BarsDictionary)
                        bars.Value.IsNewBar = false;
                }
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
#if false
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
#endif
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
                        TakeProfit = pendingOrder.TakeProfit,
                        CalculationMode = pendingOrder.CalculationMode,
                    };

                    PendingOrders.RaiseFilled(new PendingOrderFilledEventArgs(pendingOrder, position));
                    PendingOrders.Remove(pendingOrder);
                    Positions.Add(position);
                }

                if (execution.Order.IsMarket)
                {
                    position = Positions.Where(p => GetSignal(p.Label, p.Comment) == execution.Order.Name
                        || GetSignal(p.Label, p.Comment) == execution.Order.FromEntrySignal).FirstOrDefault();
                    position.EntryTime = Time;

                    // remove invalid positions left behind by previous unfilled opens
                    Position invalidPos = null;
                    do
                    {
                        invalidPos = Positions.Where(p => p.EntryTime < CoFu.TimeInvalid).FirstOrDefault();
                        if (null != invalidPos)
                            Positions.Remove(invalidPos);
                    } while (null != invalidPos);
                }

                // From Ninja website:
                // Assign entryOrder in OnOrderUpdate() or later in OnExecutionUpdate() to ensure the assignment occurs when expected.
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
                    || GetSignal(p.Label, p.Comment) == execution.Order.FromEntrySignal
                    || (execution.Order.Name.Contains("session close")
                        && p.SymbolName == execution.Order.Instrument.FullName)).FirstOrDefault();
                if (null != position)
                {
                    position.ClosePrice = price;
                    var trade = SystemPerformance.AllTrades[SystemPerformance.AllTrades.Count - 1];
                    Account.Balance += trade.ProfitCurrency;
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
                        GrossProfit = trade.ProfitCurrency,
                        NetProfit = trade.ProfitCurrency - trade.Commission,
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
                    Positions.RaiseClosed(new PositionClosedEventArgs(position, PositionCloseReason.Closed));
                    Positions.Remove(position);
                }
            }
        }

        // here we dispatch limit orders and raise the corresponding events
        protected override void OnOrderUpdate(
            Order order,
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
                    || OrderState.ChangePending == orderState
                    || OrderState.ChangeSubmitted == orderState)
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
            string comment,
            CalculationMode calculationMode = CalculationMode.Price)
        {
            Order order = null;
            var botSymbol = Symbols.GetSymbol(symbolName);
            var signal = GetSignal(label, comment);
            var isLong = tradeType == TradeType.Buy;

            var position = new Position(AbstractRobot, order)
            {
                // Real EntryTime will be set by OnExecutionUpdate()
                EntryTime = default,
                Label = label,
                Comment = comment,
                StopLoss = stopLossPrice,
                TakeProfit = takeProfitPrice,
            };
            Positions.Add(position);

            SetInitialProtection(botSymbol, signal, position.StopLoss, position.TakeProfit, calculationMode);

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
            CalculationMode calculationMode,
            DateTime? expiration,
            string comment)
        {
            Order order = null;
            var botSymbol = Symbols.GetSymbol(symbolName);
            var signal = GetSignal(label, comment);
            var isLong = tradeType == TradeType.Buy;
            var currentClosePrice = isLong ? botSymbol.Bid : botSymbol.Ask;

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
                CalculationMode = calculationMode
            };
            PendingOrders.Add(pendingOrder);

            SetInitialProtection(pendingOrder.Symbol, signal, pendingOrder.StopLoss, pendingOrder.TakeProfit, calculationMode);

            // The limit order lives til end of day or til canceled
            order = isLong
                ? EnterLongLimit(botSymbol.SymbolBarIndex, true, (int)volume, limitPrice, signal)
                : EnterShortLimit(botSymbol.SymbolBarIndex, true, (int)volume, limitPrice, signal);

            return new TradeResult() { IsSuccessful = null != order };
        }

        public TradeResult ModifyPosition(Position position,
            double? stopLoss,
            double? takeProfit,
            CalculationMode calculationMode)
        {
            string signal = GetSignal(position.Label, position.Comment);
            int quantity = (int)position.Quantity;
            bool isLong = position.TradeType == TradeType.Buy;

            if (stopLoss.HasValue && 0 != stopLoss)
                if (stopLoss != position.StopLoss)
                {
                    position.StopLoss = stopLoss;
                    SetStopLoss(signal, calculationMode, (double)stopLoss, false);
                }

            if (takeProfit.HasValue && 0 != takeProfit)
                if (takeProfit != position.TakeProfit)
                {
                    position.TakeProfit = takeProfit;
                    SetProfitTarget(signal, calculationMode, (double)takeProfit);
                }

            return new TradeResult { IsSuccessful = true, Position = position };
        }

        public TradeResult PlaceTrailProtection(Position position,
            double trailDistance,
            CalculationMode calculationMode)
        {
            // NT is such a piece of SHIT !!! NT easily could compute ticks from currency itself!
            // From https://ninjatrader.com/support/helpguides/nt8/NT%20HelpGuide%20English.html?settrailstop.htm
            // CalculationMode.Price and CalculationMode.Currency are irrelevant for trail stops.
            // Attempting to use one of these modes will log a message and the stop order be ignored.
            // Please use SetStopLoss() for these modes instead.
            var ticks = AbstractRobot.CalcMoneyAndVolume2Ticks(position.Symbol,
                trailDistance, 1);
            //SetTrailStop(position.NinjaOrder.FromEntrySignal, CalculationMode.Ticks, ticks, false);
            SetTrailStop(position.NinjaOrder.Name, CalculationMode.Ticks, ticks, false);

            return new TradeResult { IsSuccessful = true, Position = position };
        }

        private void SetInitialProtection(Symbol symbol,
            string signal,
            double? stopLoss,
            double? takeProfit,
            CalculationMode calculationMode)
        {
            if (stopLoss.HasValue && 0 != stopLoss.Value)
                if (-1 == takeProfit)
                {
                    // NT is such a piece of SHIT !!! NT easily could compute ticks from currency itself!
                    // From https://ninjatrader.com/support/helpguides/nt8/NT%20HelpGuide%20English.html?settrailstop.htm
                    // CalculationMode.Price and CalculationMode.Currency are irrelevant for trail stops.
                    // Attempting to use one of these modes will log a message and the stop order be ignored.
                    // Please use SetStopLoss() for these modes instead.
                    var ticks = AbstractRobot.CalcMoneyAndVolume2Ticks(symbol, stopLoss.Value, 1);
                    SetTrailStop(signal, CalculationMode.Ticks, ticks, false);
                }
                else
                    SetStopLoss(signal, calculationMode, stopLoss.Value, true);

            if (takeProfit.HasValue && takeProfit > 0)
                SetProfitTarget(signal, calculationMode, takeProfit.Value);
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
            var signal = GetSignal(position.Label, position.Comment);
            Order order = null;
            if (position.TradeType == TradeType.Buy)
            {
                if (position.GrossProfit > 0)
                    // From NT Web Site: Methods that generate orders to exit a position will be ignored if:
                    // A position is open and an order submitted by an enter method(EnterLongLimit() for example)
                    // is active and the order is used to open a position in the opposite direction
                    //order = ExitLongLimit(position.Symbol.SymbolBarIndex, true, (int)volume, position.CurrentPrice, signal, signal);
                    order = ExitLong(position.Symbol.SymbolBarIndex, (int)volume, signal, signal);
                else
                    order = ExitLong(position.Symbol.SymbolBarIndex, (int)volume, signal, signal);
            }
            else
            {
                if (position.GrossProfit > 0)
                    order = ExitShort(position.Symbol.SymbolBarIndex, (int)volume, signal, signal);
                //order = ExitShortLimit(position.Symbol.SymbolBarIndex, true, (int)volume, position.CurrentPrice, signal, signal);
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

        private int GetBarsSeconds(BarsPeriod barsPeriod)
        {
            if (barsPeriod.BarsPeriodType == BarsPeriodType.Tick)
                return 0;

            var dataRateSeconds = barsPeriod.Value;
            if (barsPeriod.BarsPeriodType == BarsPeriodType.Minute)
                dataRateSeconds *= SEC_PER_MINUTE;
            else if (barsPeriod.BarsPeriodType == BarsPeriodType.Day)
                dataRateSeconds *= SEC_PER_DAY;

            return dataRateSeconds;
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

        // Must be ovverridden in derived bot class
        protected virtual void OnSetDefaults() { }
        protected virtual void OnConfigure() { }
        protected virtual void OnDataLoaded() { }
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

namespace NinjaTrader.NinjaScript.Indicators
{
    public static class Extensions
    {
#if !GITHUB
        public static double GetLast(this SMA indi, int index)
        {
            return indi.Value[index];
        }

        public static double GetLast(this WMA indi, int index)
        {
            return indi.Value[index];
        }

        public static double GetLast(this EMA indi, int index)
        {
            return indi.Value[index];
        }

        public static double GetLast(this EdgeTradingOscillator indi, int macdSignal, int index)
        {
            return indi.Values[macdSignal][index];
        }

        public static double GetLast(this DoubleStochastics indi, int index)
        {
            return indi.K[index];
        }

        public static double GetLast(this OrderFlowCumulativeDelta indi, int ohlc, int index)
        {
            // 0 == open, 1 == high, 2 == low, 3 == close
            return indi.Values[ohlc][index];
        }
#endif
    }
}
