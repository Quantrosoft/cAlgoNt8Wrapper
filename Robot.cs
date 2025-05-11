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

using cAlgo.API.Internals;
using NinjaTrader.Cbi;
using NinjaTrader.Core;
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
        [XmlIgnore] public new Bars Bars;
        [XmlIgnore] public Symbol Symbol;
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
        [XmlIgnore] public CSRobotFactory mRobotFactory;
        [XmlIgnore] public IRobot mRobot;
        [XmlIgnore] public MarketDataEventArgs mMarketDataEventArgs;
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
        [Browsable(false)]
        [XmlIgnore]
        public new DateTime Time =>
            //IsTickReplay
            //? (null == mMarketDataEventArgs ? CoFu.TimeInvalid : mMarketDataEventArgs.Time)
            //: 
            Times[0][0];    // Nt primary data series is used as cTrader data series
        [Browsable(false)][XmlIgnore] public bool IsBacktesting => RunningMode != RunningMode.RealTime;

        private bool mDoTerminate;
        private bool mDoStart;
        private Dictionary<NinjaTrader.Cbi.Instrument, MarketPosition> lastPositions
            = new Dictionary<NinjaTrader.Cbi.Instrument, MarketPosition>();
        #endregion

        #region Start
        protected void InitDataSeries()
        {
            // Generate NinjaTrader bid and ask data series as pendants of requested cTrader Bars
            int count = 0;
            foreach (KeyValuePair<(int, string), Bars> kvp in MarketData.BarsDictionary)
            {
                // skip primary data series
                if (0 == count)
                {
                    Bars = kvp.Value;   // Set default bars

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
                    OrderFillResolution = OrderFillResolution.Standard;
                    Slippage = 0;
                    StartBehavior = StartBehavior.WaitUntilFlat;
                    TimeInForce = TimeInForce.Gtc;
                    TraceOrders = false;
                    RealtimeErrorHandling = RealtimeErrorHandling.StopCancelClose;
                    StopTargetHandling = StopTargetHandling.PerEntryExecution;
                    // Disable this property for performance gains in Strategy Analyzer optimizations
                    // See the Help Guide for additional information
                    IsInstantiatedOnEachOptimizationIteration = true;

                    OnSetDefaults();    // Call user's bot
                }
                break;

                case State.Configure:
                {
                    if (GetDebugLaunch())
                        Debugger.Launch();

                    #region Init
                    mRobotFactory = new CSRobotFactory();
                    mRobot = mRobotFactory.CreateRobot();
                    PlatformTimeZoneInfo = Globals.GeneralOptions.TimeZoneInfo;

                    Symbols = new Symbols(this);
                    MarketData = new MarketData(this);
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

                    // Set default cTrader Bars and Symbol as pendant of NinjaTrader primary data series
                    Bars = MarketData.GetBars(new TimeFrame(dataRateSeconds), Instrument.FullName);
                    Symbol = Symbols.GetSymbol(Instrument.FullName);

                    mRobot.ConfigInit(this); // set time zone, open/close callbacks, etc.

                    // New Bars and Symbols can and must be added here
                    OnConfigure();      // Call user's bot init 1st time OnConfigure
                    InitDataSeries();   // Add DataSeries as reqested in MarketData.GetBars
                    #endregion
                }
                break;

                case State.DataLoaded:
                {
                    if (IsTickReplay)
                        Debug.Assert(BarsArray.Length == MarketData.BarsDictionary.Count,
                            "Error: Number of BarsArray does not match number of MarketData.BarsDictionary");
                    else
                    {
                        Debug.Assert(BarsArray.Length == 2 * MarketData.BarsDictionary.Count,
                            "Error: Number of BarsArray does not match number of MarketData.BarsDictionary");

                        mMarketDataEventArgs = new MarketDataEventArgs();
                    }

                    // Init bars and their series 
                    foreach (var bars in MarketData.BarsDictionary)
                        bars.Value.OnBarsDataLoaded();

                    mDoTerminate = mDoStart = true;
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
                {
                    if (mDoTerminate)
                    {
                        OnStop(); // Call user's bot
                        mDoTerminate = false;
                        Print("Done\n");
                    }
                }
                break;
            }
        }

        protected override void OnMarketData(MarketDataEventArgs args)
        {
            // Realtime trading or Tick Replay must be active for OnMarketData() to get called
            // OnMarketData() is not called in backtests unless Tick Replay is enabled
            // Only Last can and must be used in Data Series when Tick Replay is enabled
            if (BarsInProgress != 0 || args.Bid <= 0 || args.Ask <= 0
                || (IsTickReplay && MarketDataType.Last != args.MarketDataType))
                return;

            // ToDo: Put into symbol corresponding with args.Instrument
            // when having several symbols
            mMarketDataEventArgs = args;

            // we have to postpone OnStart etc, til here because earlier we do not have a valid Time
            if (CurrentBar >= 0)
            {
                if (mDoStart)
                {
                    OnStart();  // Call user's bot OnStart
                    mDoStart = false;
                }

                // Update the bars with the new market data
                foreach (var bar in MarketData.BarsDictionary)
                    bar.Value.OnMarketData(args);

                // Call user bot
                mRobot.PreTick();
                OnTick();
                mRobot.PostTick();
            }
        }

        protected override void OnBarUpdate()
        {
            // Only process the primary data series
            if (CurrentBar <= 0 || BarsInProgress != 0 || IsTickReplay)
                return;

            // we have to postpone OnStart until here because earlier we do not have a valid Time
            if (mDoStart)
            {
                OnStart();  // Call user's bot init 2nd time OnStart
                mDoStart = false;
            }

            mRobot.PreTick();
            OnTick();
            mRobot.PostTick();
        }

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

            MarketPosition previous = lastPositions.ContainsKey(execution.Instrument)
                ? lastPositions[execution.Instrument]
                : MarketPosition.Flat;

            // Detect transition from flat to non-flat = position opend
            // FYI: Position = Positions[base.BarsInProgress] => each Bars can only have one open position = Netting
            if (previous != MarketPosition.Flat && Position.MarketPosition == MarketPosition.Flat)
            {
                var position = Positions.Where(p => p.SymbolName == execution.Instrument.FullName).FirstOrDefault();
                if (null != position)
                {
                    Positions.RaiseClosed(new PositionClosedEventArgs(position, PositionCloseReason.Closed));
                    Positions.Remove(position);
                }
                else
                { }
            }

            // Update the tracking dictionary
            lastPositions[execution.Instrument] = Position.MarketPosition;
        }

        protected override void OnOrderUpdate(
            NinjaTrader.Cbi.Order order,
            double limitPrice,
            double stopPrice,
            int quantity,
            int filled,
            double averageFillPrice,
            OrderState orderState,
            DateTime time,
            ErrorCode error,
            string comment)
        {
        }
        #endregion

        #region Methods
        public TradeResult ExecuteMarketOrder(TradeType tradeType,
            string symbolName,
            double volume,
            string label,
            double? stopLossPips,
            double? takeProfitPips,
            string comment)
        {
            double? stopPrice = null;
            double? targetPrice = null;

            var botSymbol = Symbols.GetSymbol(symbolName);
            var currentClosePrice = tradeType == TradeType.Buy ? botSymbol.Bid : botSymbol.Ask;

            NinjaTrader.Cbi.Order order = null;
            var signal = label + "|" + comment;
            if (tradeType == TradeType.Buy)
            {
                // store label + comment in the order for later restart
                order = EnterLong((int)volume, signal);

                if (null != stopLossPips && 0 != stopLossPips)
                {
                    stopPrice = currentClosePrice - (double)stopLossPips * TickSize;
                    SetStopLoss(signal, CalculationMode.Price, (double)stopPrice, false);
                }

                if (null != takeProfitPips && 0 != takeProfitPips)
                {
                    targetPrice = currentClosePrice + (double)takeProfitPips * TickSize;
                    SetProfitTarget(signal, CalculationMode.Price, (double)targetPrice);
                }
            }
            else // TradeType.Sell
            {
                // store label + comment in the order for later restart
                order = EnterShort((int)volume, signal);
                if (null != stopLossPips && 0 != stopLossPips)
                {
                    stopPrice = currentClosePrice + (double)stopLossPips * TickSize;
                    SetStopLoss(label, CalculationMode.Price, (double)stopPrice, false);
                }

                if (null != takeProfitPips && 0 != takeProfitPips)
                {
                    targetPrice = currentClosePrice - (double)takeProfitPips * TickSize;
                    SetProfitTarget(signal, CalculationMode.Price, (double)targetPrice);
                }
            }

            var position = new Position(this, order)
            {
                EntryTime = Time,
                Comment = comment,
                Label = signal,
                Swap = 0,                              // Not tracked by NinjaTrader by default
                StopLoss = null,                       // If you set SL/TP, track externally
                TakeProfit = null,
                Pips = 0,
                Margin = 0,
                HasTrailingStop = false
            };
            Positions.Add(position);
            Positions.RaiseOpened(new PositionOpenedEventArgs(position));
            return new TradeResult() { Position = position, IsSuccessful = true };
        }

        public TradeResult PlaceLimitOrder(TradeType tradeType,
            string symbolName,
            double volume,
            double limitPrice,
            string label,
            double? stopLossPips,
            double? takeProfitPips,
            DateTime? expiration,
            string comment)
        {
            double? stopPrice = null;
            double? tpPrice = null;

            var botSymbol = Symbols.GetSymbol(symbolName);
            var currentClosePrice = tradeType == TradeType.Buy ? botSymbol.Bid : botSymbol.Ask;

            NinjaTrader.Cbi.Order order = null;
            var signal = label + "|" + comment;
            if (tradeType == TradeType.Buy)
            {
                // public Order EnterLongLimit(int barsInProgressIndex, bool isLiveUntilCancelled, int quantity, double limitPrice, string signalName)
                order = EnterLongLimit(botSymbol.SymbolIsOnBarIndex,
                    true,
                    (int)volume,
                    limitPrice,
                    signal);

                if (null != stopLossPips && 0 != stopLossPips)
                {
                    stopPrice = currentClosePrice - (double)stopLossPips * TickSize;
                    SetStopLoss(signal, CalculationMode.Price, (double)stopPrice, false);
                }

                if (null != takeProfitPips && 0 != takeProfitPips)
                {
                    tpPrice = currentClosePrice + (double)takeProfitPips * TickSize;
                    SetProfitTarget(signal, CalculationMode.Price, (double)tpPrice);
                }
            }
            else // TradeType.Sell
            {
                order = EnterShortLimit(botSymbol.SymbolIsOnBarIndex,
                    true,
                    (int)volume,
                    limitPrice,
                    signal);
                if (null != stopLossPips && 0 != stopLossPips)
                {
                    stopPrice = currentClosePrice + (double)stopLossPips * TickSize;
                    SetStopLoss(signal, CalculationMode.Price, (double)stopPrice, false);
                }

                if (null != takeProfitPips && 0 != takeProfitPips)
                {
                    tpPrice = currentClosePrice - (double)takeProfitPips * TickSize;
                    SetProfitTarget(signal, CalculationMode.Price, (double)tpPrice);
                }
            }

            var position = new Position(this, order)
            {
                EntryTime = Time,
                Comment = comment,
                Label = signal,
                Swap = 0,                              // Not tracked by NinjaTrader by default
                StopLoss = null,                       // If you set SL/TP, track externally
                TakeProfit = null,
                Pips = 0,
                Margin = 0,
                HasTrailingStop = false
            };

            return new TradeResult() { IsSuccessful = false, };
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

            if (position.TradeType == TradeType.Buy)
                ExitLong((int)position.Quantity, position.Label, position.Label);
            else
                ExitShort((int)position.Quantity, position.Label, position.Label);

            return new TradeResult() { IsSuccessful = true };
        }

        public void Stop() => throw new Exception("Bot stoped");

        // Must be ovverridden in derived bot class
        protected virtual void OnSetDefaults() { }
        protected virtual void OnConfigure() { }
        protected virtual void OnStart() { }
        protected virtual bool GetDebugLaunch() { return false; }
        protected virtual void OnTick() { }
        protected virtual void OnBar() { }
        protected virtual void OnStop() { }
        protected virtual double GetFitness(GetFitnessArgs args) { return 0; }
        #endregion
    }
}
