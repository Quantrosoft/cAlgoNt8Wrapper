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

#if CTRADER
using cAlgo.API;
using cAlgo.API.Internals;
#else
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Strategies;
using NinjaTrader.NinjaScript.Strategies.Internals;
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TdsCommons;
using static TdsDefs;

namespace RobotLib
{
    #region Enums
    public enum ProfitModes
    {
        Lots,
        Volume,
        Contracts,
        ConstantInvest,
        Reinvest,
        //LotsPro10k,
        //ProfitPercent,
        //ProfitAmmount,
        //RiskConstant,
        //RiskReinvest,
        na
    };

    public enum TradingPlatform
    {
        MetaTrader4,
        MetaTrader5,
        NinjaTrader,
        cTrader,
        KitaTrader,
        TradingView,
        QuantConnect, // Lean Engine
    }

#if CTRADER
    public enum CalculationMode
    {
        Currency,
        Price,
    }
#endif
    #endregion

    #region Delegates
    public delegate Position DelegateOpenTrade(
       Symbol symbol,
       TradeType tradeType,
       TradeDirections allowedDirection,
       string commentVersion,
       string label,
       double volume);

    public delegate TradeResult DelegateCloseTrade(Position pos);
    public delegate void DelegateOnPositionOpened(PositionOpenedEventArgs args);
    public delegate void DelegateOnPositionClosed(PositionClosedEventArgs args);
    #endregion

    public class LogParams
    {
        public Symbol Symbol;
        public double Lots;
        public double Volume;
        public double Minlots;
        public double TradeMargin;
        public double AccountMargin;
        public TradeType TradeType;
        public DateTime EntryTime, ExitTime;
        public double EntryPrice;
        public double ClosingPrice;
        public string Comment;
        public double Commissions;
        public double Swap;
        public double NetProfit;
        public string OrderType;
    }

    public class Drawings
    {
        //double mLastBottomY;
        //double mLastTopY;
        Strategy mBot;
        private AbstractRobot mAbstractRobot;
        private int mPrevOverlayBarCount;
        private double mPrevOverlayData;

        public Drawings(Strategy bot, AbstractRobot abstractRobot)
        {
            mBot = bot;
            mAbstractRobot = abstractRobot;
        }

        public void DrawOverlay(
           Ringbuffer<(DateTime, double)> data,
           string name,
           DateTime botCurrentTime,
           Color color,
           int thickness)
        {
            var isNewChartBar = mPrevOverlayBarCount != mAbstractRobot.QcBars.Count;
            var isInit = 0 == mPrevOverlayBarCount;
            for (int i = 0;
               i < data.Count - 1
                  && data.Last(i).Item1 > mBot.Chart.Bars.OpenTimes[mBot.Chart.FirstVisibleBarIndex];
               i++)
            {
                if (double.IsNaN(data.Last(i + 1).Item2)
                   //|| !data.Last(i + 1).IsBufferValid
                   )
                    break;
                else
                    mBot.Chart.DrawTrendLine("Overlay" + name +
                        (isInit ? i : (mBot.Chart.Bars.Count % mBot.Chart.MaxVisibleBars + (isNewChartBar ? -1 : 0))).ToString(),
                       isInit ? data.Last(i + 1).Item1 : mAbstractRobot.QcBars.OpenTimes.Last((isNewChartBar ? 1 : 0)),
                       isInit ? data.Last(i + 1).Item2 : mPrevOverlayData,
                       isInit ? data.Last(i).Item1 : botCurrentTime,
                       data.Last(i).Item2,
                       color, thickness).IsInteractive = true;

                if (!isInit)
                    break;
            }

            if (isInit || isNewChartBar)
                mPrevOverlayData = data.Last(0).Item2;

            mPrevOverlayBarCount = mAbstractRobot.QcBars.Count;
        }

        public void DrawOverlay(IQcBars indiBars,
           IQcDataSeries data,
           string name,
           DateTime botCurrentTime,
           Color color,
           int thickness,
           int offset = 0)
        {
            var isNewChartBar = mPrevOverlayBarCount != mAbstractRobot.QcBars.Count;
            var isInit = 0 == mPrevOverlayBarCount;

            for (int i = offset;
               i < indiBars.Count - 1
                  && indiBars.OpenTimes.Last(i) > mBot.Chart.Bars.OpenTimes[mBot.Chart.FirstVisibleBarIndex];
               i++)
            {
                if (double.IsNaN(data.Last(i + 1)))
                    break;
                else
                {
                    var last = data.Last(i);
                    mBot.Chart.DrawTrendLine("Overlay" + name
                        + (mBot.Chart.Bars.Count % mBot.Chart.MaxVisibleBars - i + (isNewChartBar ? -1 : 0)).ToString(),
                       isInit
                          ? indiBars.OpenTimes.Last(i + 1)
                          : mAbstractRobot.QcBars.OpenTimes.Last(isNewChartBar ? 1 + offset : 0),
                       isInit
                          ? data.Last(i + 1)
                          : mPrevOverlayData,
                       isInit
                          ? indiBars.OpenTimes.Last(i)
                          : isNewChartBar ? mAbstractRobot.QcBars.OpenTimes.Last(offset) : mBot.Time,
                       last,
                       color, thickness).IsInteractive = true;
                }
                if (!isInit)
                    break;
            }

            if (isInit || isNewChartBar)
                mPrevOverlayData = data.Last(isNewChartBar ? 1 : 0);

            mPrevOverlayBarCount = mAbstractRobot.QcBars.Count;
        }

        public void DrawNoOverlay(
           IQcBars dataBars,
           IQcDataSeries data,
           string name,
           DateTime currentTime,
           Color color,
           int thickness,
           double line1 = double.NaN,
           double line2 = double.NaN)
        {
            if (mBot.Chart.FirstVisibleBarIndex >= 0)
                if (mBot.Chart.LastVisibleBarIndex >= 0)
                    if (mBot.Chart.FirstVisibleBarIndex < mBot.Chart.LastVisibleBarIndex)
                        if (mBot.Chart.FirstVisibleBarIndex < mBot.Chart.Bars.Count)
                        {
                            // find highest/lowest in data; for() is much faster than LINQ
                            var minVal = double.MaxValue;
                            var maxVal = double.MinValue;
                            for (int i = 0;
                               i < dataBars.Count && dataBars.OpenTimes.Last(i)
                                    >= mBot.Chart.Bars.OpenTimes[mBot.Chart.FirstVisibleBarIndex];
                               i++)
                            {
                                CoFu.Max(ref maxVal, data.Last(i));
                                CoFu.Min(ref minVal, data.Last(i));
                            }

                            //if (AbstractRobot.Chart.TopY != mLastTopY || AbstractRobot.Chart.BottomY != mLastBottomY
                            {
                                var drawFact = (maxVal - minVal) / (mBot.Chart.TopY - mBot.Chart.BottomY);

                                if (0 != drawFact)
                                {
                                    for (int i = 0;
                                       i < dataBars.Count && dataBars.OpenTimes.Last(i)
                                            >= mBot.Chart.Bars.OpenTimes[mBot.Chart.FirstVisibleBarIndex];
                                       i++)
                                    {
                                        var prev = data.Last(i + 1);
                                        var current = data.Last(i);
                                        if (!double.IsNaN(current) && !double.IsNaN(prev))
                                            mBot.Chart.DrawTrendLine("NoOverlay" + name + i,
                                               dataBars.OpenTimes.Last(i + 1),
                                               mBot.Chart.BottomY + (prev - minVal) / drawFact,
                                               0 == i ? currentTime : dataBars.OpenTimes.Last(i),
                                               mBot.Chart.BottomY + (current - minVal) / drawFact,
                                               color,
                                               thickness).IsInteractive = mBot.IsBacktesting;
                                    }

                                    if (!double.IsNaN(line1))
                                    {
                                        var absLine = mBot.Chart.BottomY + (line1 - minVal) / drawFact;
                                        mBot.Chart.RemoveObject("NoOvLine1");
                                        mBot.Chart.DrawHorizontalLine("NoOvLine1",
                                           absLine,
                                           Color.White,
                                           1,
                                           LineStyle.DotsRare).IsInteractive = mBot.IsBacktesting;
                                    }

                                    if (!double.IsNaN(line2))
                                    {
                                        var absLine = mBot.Chart.BottomY + (line2 - minVal) / drawFact;
                                        mBot.Chart.RemoveObject("NoOvLine2");
                                        mBot.Chart.DrawHorizontalLine("NoOvLine2",
                                           absLine,
                                           Color.White,
                                           1,
                                           LineStyle.DotsRare).IsInteractive = mBot.IsBacktesting;
                                    }
                                }
                            }
                        }
            //mLastBottomY = AbstractRobot.Chart.BottomY;
            //mLastTopY = AbstractRobot.Chart.TopY;
        }

        public void DrawHistogram(List<double> data, string name, Color color)
        {
            var histograms = mBot.Chart.FindAllObjects(ChartObjectType.TrendLine);
            foreach (var histogram in histograms)
            {
                if (histogram.Name.Contains("Histogram" + name))
                    mBot.Chart.RemoveObject(histogram.Name);
            }

            var middle = (mBot.Chart.TopY + mBot.Chart.BottomY) / 2;
            mBot.Chart.RemoveObject("NoOvMiddle");
            mBot.Chart.DrawHorizontalLine("NoOvMiddle",
               middle,
               Color.White,
               1,
               LineStyle.DotsRare);

            var drawFact = 100 / (mBot.Chart.TopY - middle);

            var barsbaseCnt = mBot.Chart.Bars.Count - data.Count;
            for (int i = 0; i < data.Count; i++)
                mBot.Chart.DrawTrendLine("Histogram" + name + i,
                   barsbaseCnt + i, middle,
                   barsbaseCnt + i, middle + data[i] / drawFact,
                   color,
                   3).IsInteractive = true;
        }
    }

    public abstract class AbstractRobot
    {
        #region Members
        public IQcBars QcBars;
        public TradingPlatform TradingPlatform;
        public bool IsNinjaTrader;
        public bool IsCtrader;
        // Own accounting since cTrader does have a bug in limit accounting
        public double AccountWinProfit, AccountLossProfit;
        public double AccountEquity, AccountBalance;
        public int AccountWinningTrades, AccountLoosingTrades, TotalTrades;
        public int SameTimeOpen, SameTimeOpenCount, MaxEquityDrawdownCount;
        public int MaxBalanceDrawdownCount, OpenDurationCount;

        public double StartBalance, MaxEquityDrawdownValue, Calmar, TradesPerMonth, mChanceRiskRatio;
        public double ProfitConstanceness, ProfitFactor, MaxEquity, MaxMargin, MaxBalanceDrawdownValue;
        public double MaxBalance;
        public DateTime SameTimeOpenDateTime, MaxBalanceDrawdownTime, MaxEquityDrawdownTime;
        public TimeSpan MinOpenDuration = new TimeSpan(long.MaxValue);
        public TimeSpan OpenDurationSum = new TimeSpan(0);
        public TimeSpan AvgOpenDuration = new TimeSpan(0);
        public TimeSpan MaxOpenDuration = new TimeSpan(0);
        public Strategy mRobot;

        protected ILogger mLogger;
        protected bool mValidateTickData, mIsInit, mIsSwapLongInit, mIsSwapShortInit, mIsCommissionsInit, mIs1stTick = true;
        protected int mLoggingTradeCount;
        protected double mLoggingSaldo, mSwapLong, mSwapShort, mCommissions;
        protected string mTimeZoneId, cCommentTab;
        protected DateTime mPrevTime, mInitialTime;
        protected Color[] cTradeColor;
        protected ChartIconType[] cTradeIcon;
        protected DateTime cInvalidTime;

        private string[] mHeaderSplit;
        private DataRateId mDataRateId;
        private Dictionary<string, TickServerReader<TickserverMarketDataArgs>> TickServerDictionary
            = new Dictionary<string, TickServerReader<TickserverMarketDataArgs>>();

        public double LotPoint(Symbol symbol)
        {
            return MinLot(symbol);
        }

        public double MinLot(Symbol symbol)
        {
            return symbol.VolumeInUnitsToQuantity(symbol.VolumeInUnitsMin);
        }

        public double MaxLot(Symbol symbol)
        {
            return symbol.VolumeInUnitsToQuantity(symbol.VolumeInUnitsMax);
        }

        public double StepLot(Symbol symbol)
        {
            return symbol.VolumeInUnitsToQuantity(symbol.VolumeInUnitsStep);
        }

        public double CommissionPerLot(Symbol symbol)
        {
#if CTRADER
            switch (symbol.CommissionType)
            {
                //     Commission is in USD per millions USD normalizedVolume.
                case SymbolCommissionType.UsdPerMillionUsdVolume:
                    return 2 * symbol.Commission * symbol.LotSize / mRobot.Account.Asset.Convert("USD", 1e6);

                //     Commission is in USD per one symbol lot.
                case SymbolCommissionType.UsdPerOneLot:
                    return 2 * symbol.Commission / mRobot.Account.Asset.Convert("USD", 1e6);

                //     Commission is in Percentage of trading normalizedVolume.
                case SymbolCommissionType.PercentageOfTradingVolume:
                    return symbol.Commission;

                //     Commission is in symbol quote asset / currency per one lot.
                case SymbolCommissionType.QuoteCurrencyPerOneLot:
                    //Debugger.Break();
                    throw new NotImplementedException();
            }
#endif
            return 0;
        }

        public double SwapPerLot(Symbol symbol, bool isLongNotShort)
        {
#if CTRADER
            switch (symbol.SwapCalculationType)
            {
                //     SymbolName SWAP Long/Short is in Pips
                case SymbolSwapCalculationType.Pips:
                    return (isLongNotShort ? symbol.SwapLong : symbol.SwapShort) * symbol.PipValue * symbol.LotSize;

                //     SymbolName SWAP Long/Short is in Percent (%) of margin
                case SymbolSwapCalculationType.Percentage:
                    var margin = symbol.GetEstimatedMargin(TradeType.Buy, symbol.QuantityToVolumeInUnits(1));
                    return (isLongNotShort ? symbol.SwapLong : symbol.SwapShort) / 100 * margin;
            }
#endif
            return 0;
        }

        public int LotDigits(Symbol symbol)
        {
            return (int)(0.5 + Math.Log10(1.0 / LotPoint(symbol)));
        }

        public double InitialAccountBalance
        {
            get { return StartBalance; }
        }

        public DateTime InitialTime
        {
            get { return mInitialTime; }
        }

        public int SpreadInPoints(Symbol symbol)
        {
            return iPrice(symbol.Ask - symbol.Bid, symbol);
        }

        public bool Is1stTick => mIs1stTick;

        public DateTime PrevTime => mPrevTime;

        public DataRateId DataRateId => mDataRateId;

        public bool IsVisualMode => mRobot.RunningMode == RunningMode.VisualBacktesting;

        public bool IsRealtime => mRobot.RunningMode == RunningMode.RealTime;

        public bool IsVisible => IsVisualMode || IsRealtime;

        public string CommentTab => cCommentTab;

        public bool IsValidateTickData
        {
            get { return mValidateTickData; }
            set { mValidateTickData = value; }
        }
        #endregion

        #region ctor
        public AbstractRobot()
        {
#if CTRADER
            TradingPlatform = TradingPlatform.cTrader;
            IsCtrader = true;
#else
            TradingPlatform = TradingPlatform.NinjaTrader;
            IsNinjaTrader = true;
#endif
            cCommentTab = "\n\t\t";

            cTradeColor = new Color[2];
            cTradeColor[0] = Color.SteelBlue;
            cTradeColor[1] = Color.HotPink;

            cTradeIcon = new ChartIconType[2];
            cTradeIcon[0] = ChartIconType.UpArrow;
            cTradeIcon[1] = ChartIconType.DownArrow;

            cInvalidTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);   // needed to convert DateTime and Mt4 datetime
        }
        #endregion

        #region Methods
        public virtual string ConfigInit(Strategy robot, string timeZoneId = "")
        {
            mRobot = robot;
            mTimeZoneId = timeZoneId;
            AccountEquity = AccountBalance = StartBalance = Account.Balance;
            mRobot.Positions.Closed += OnPositionClosed;
            return "";
        }

        public virtual void Start()
        {
            mInitialTime = mRobot.Time;

            mIsInit = true;
            PreTick();
            PostTick();
            mIsInit = false;
            mIs1stTick = true;
        }

        private void OnPositionClosed(PositionClosedEventArgs args)
        {
            (var grossProfit, var netProfit, var openPrice) = GetProfitsPrice(args.Position);
            if (netProfit >= 0)
            {
                AccountWinProfit += netProfit;
                AccountWinningTrades++;
            }
            else
            {
                AccountLossProfit += -netProfit;
                AccountLoosingTrades++;
            }
            AccountBalance += netProfit;
        }

        public virtual void PreTick()
        {
            if (!mIsInit)
                if (DataRateId.Undefined == DataRateId)
                    if (mRobot.RunningMode == RunningMode.RealTime
                          || Time - mPrevTime < TimeSpan.FromSeconds(50))
                        mDataRateId = DataRateId.Ticks;
                    else if (Time - mPrevTime < TimeSpan.FromSeconds(115))
                        mDataRateId = DataRateId.Minutes;
                    else
                        mDataRateId = DataRateId.Timeframe;

            // Update all tick server readers
            foreach (var tickServerReader in TickServerDictionary.Values)
                tickServerReader.TickServerReaderOnTick();

            UpdateProfit();
        }

        public string OnStopPreTick(string version, int configsCount)
        {
            var infoText = "";

            PreTick();

            TotalTrades = AccountWinningTrades + AccountLoosingTrades;
            ProfitFactor = AccountWinProfit / AccountLossProfit;
            var netProfit = AccountWinProfit - AccountLossProfit;
            TradesPerMonth = ((double)TotalTrades / ((Time - InitialTime).TotalDays / 365)) / 12;
            ProfitConstanceness = CalculateGoodnessOfFit(mRobot.History) * 100; // R² = 1 means perfect fit, R² = 0 means no fit

            var annualProfit = netProfit / ((Time - InitialTime).TotalDays / 365);
            var annualProfitPercent = 0 == TotalTrades ? 0 : 100.0 * annualProfit / InitialAccountBalance;
            var maxCurrentEquityDdPercent = 100 * MaxEquityDrawdownValue / MaxEquity;
            var maxStartEquityDdPercent = 100 * MaxEquityDrawdownValue / InitialAccountBalance;
            Calmar = 0 == MaxEquityDrawdownValue ? 0 : annualProfit / MaxEquityDrawdownValue;
            var winningRatioPercent = 0 == TotalTrades ? 0 : 100 * (double)AccountWinningTrades / TotalTrades;
#if !CTRADER
            //var AverageEtd = SystemPerformance.AllTrades.TradesPerformance.Currency.AverageEtd;
            //var AverageMae = SystemPerformance.AllTrades.TradesPerformance.Currency.AverageMae;
            //var AverageMfe = SystemPerformance.AllTrades.TradesPerformance.Currency.AverageMfe;
            //var AverageProfit = SystemPerformance.AllTrades.TradesPerformance.Currency.AverageProfit;
            //var CumProfit = SystemPerformance.AllTrades.TradesPerformance.Currency.CumProfit;
            //var Drawdown = SystemPerformance.AllTrades.TradesPerformance.Currency.Drawdown;
            //var LargestLoser = SystemPerformance.AllTrades.TradesPerformance.Currency.LargestLoser;
            //var LargestWinner = SystemPerformance.AllTrades.TradesPerformance.Currency.LargestWinner;
            //var ProfitPerMonth = SystemPerformance.AllTrades.TradesPerformance.Currency.ProfitPerMonth;
            //var StdDev = SystemPerformance.AllTrades.TradesPerformance.Currency.StdDev;
            //var Turnaround = SystemPerformance.AllTrades.TradesPerformance.Currency.Turnaround;
            //var Ulcer = SystemPerformance.AllTrades.TradesPerformance.Currency.Ulcer;
#endif
            infoText += DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") + " | " + version;
            infoText += "\n# of config files: " + configsCount.ToString();
            infoText += "\nMaxMargin: " + Account.Asset + " "
                + ConvertUtils.DoubleToString(MaxMargin, 2);
            infoText += "\nMaxSameTimeOpen: " + SameTimeOpen.ToString()
                + "; @ " + SameTimeOpenDateTime.ToString("dd.MM.yyyy HH:mm:ss")
                + "; Count# " + SameTimeOpenCount.ToString();
            infoText += "\nMax Balance Drawdown Value: " + Account.Asset
                + " " + ConvertUtils.DoubleToString(MaxBalanceDrawdownValue, 2)
                + "; @ " + MaxBalanceDrawdownTime.ToString("dd.MM.yyyy HH:mm:ss")
                + "; Count# " + MaxBalanceDrawdownCount.ToString();
            infoText += "\nMax Balance Drawdown%: " + (0 == MaxBalance
                ? "NaN"
                : ConvertUtils.DoubleToString(100 * MaxBalanceDrawdownValue / MaxBalance, 2));
            infoText += "\nMax Equity Drawdown Value: " + Account.Asset
                + " " + ConvertUtils.DoubleToString(MaxEquityDrawdownValue, 2)
               + "; @ " + MaxEquityDrawdownTime.ToString("dd.MM.yyyy HH:mm:ss")
               + "; Count# " + MaxEquityDrawdownCount.ToString();
            infoText += "\nMax Current Equity Drawdown %: "
                + ConvertUtils.DoubleToString(maxCurrentEquityDdPercent, 2);
            infoText += "\nMax Start Equity Drawdown %: "
                + ConvertUtils.DoubleToString(maxStartEquityDdPercent, 2);
            infoText += "\nNet Profit: " + Account.Asset + " "
                + ConvertUtils.DoubleToString(netProfit, 2);
            infoText += "\nProfit Factor: " + (0 == AccountLoosingTrades
                ? "-"
                : ConvertUtils.DoubleToString(ProfitFactor, 2));
            //infoText += "\nSharpe Ratio: " + ConvertUtils.DoubleToString(SharpeRatio, 2);
            //infoText += "\nSortino Ratio: " + ConvertUtils.DoubleToString(SortinoRatio, 2);
            infoText += "\nCalmar Ratio: " + ConvertUtils.DoubleToString(Calmar, 2);
            infoText += "\nProfit Constanceness %: " + ConvertUtils.DoubleToString(ProfitConstanceness, 2);
            infoText += "\nWinning Ratio: " + ConvertUtils.DoubleToString(winningRatioPercent, 2);
            infoText += "\nTrades Per Month: " + ConvertUtils.DoubleToString(TradesPerMonth, 2);
            infoText += "\nAverage Annual Profit Percent: "
                + ConvertUtils.DoubleToString(annualProfitPercent, 2);

            if (0 != OpenDurationCount)
            {
                infoText += "\nMin / Avg / Max Tradeopen Duration (Day.Hour.Min.Sec): "
                   + MinOpenDuration.ToString(@"dd\.hh\.mm\.ss") + " / "
                   + (AvgOpenDuration = TimeSpan.FromTicks(OpenDurationSum.Ticks / OpenDurationCount))
                   .ToString(@"dd\.hh\.mm\.ss")
                   + " / " + MaxOpenDuration.ToString(@"dd\.hh\.mm\.ss");
            }

            return infoText;
        }

        public void OnStop()
        {
            foreach (var tickServerReader in TickServerDictionary.Values)
                tickServerReader.Dispose();
        }

        public void UpdateProfit()
        {
            var grossProfit = 0.0;
            var netProfit = 0.0;
            var openPrice = 0.0;
            foreach (var pos in mRobot.Positions)
            {
                if (pos.EntryTime > CoFu.TimeInvalid)
                {
                    (grossProfit, netProfit, openPrice) = GetProfitsPrice(pos);
                    AccountEquity = AccountBalance + netProfit;
                }
            }

            if (Account.Balance != AccountBalance)
            { }
            //Debugger.Break();

            if (Account.Equity != AccountEquity)
            { }
            //Debugger.Break();

            CoFu.Max(ref MaxMargin, Account.Margin);
            if (CoFu.Max(ref SameTimeOpen, mRobot.Positions.Count))
            {
                SameTimeOpenDateTime = Time;
                SameTimeOpenCount = mRobot.History.Count;
            }

            var balance = AccountBalance;
            CoFu.Max(ref MaxBalance, balance);
            if (CoFu.Max(ref MaxBalanceDrawdownValue, MaxBalance - balance))
            {
                MaxBalanceDrawdownTime = Time;
                MaxBalanceDrawdownCount = mRobot.History.Count;
            }

            var accountEquity = AccountEquity;
            CoFu.Max(ref MaxEquity, accountEquity);
            if (CoFu.Max(ref MaxEquityDrawdownValue, MaxEquity - accountEquity))
            {
                MaxEquityDrawdownTime = Time;
                MaxEquityDrawdownCount = mRobot.History.Count;
            }
        }

        public virtual void PostTick()
        {
            mIs1stTick = false;
            mPrevTime = Time;
        }

        public double GetBidAskPrice(Symbol symbol, BidAsk bidAsk)
        {
            return bidAsk == BidAsk.Bid ? symbol.Bid : symbol.Ask;
        }

        public string CalcProfitMode2Lots(
           Symbol symbol,
           ProfitModes profitMode,
           double value,
           int tpPts,
           int riskTicks,
           out double desiMon,
           out double volumeLotSize,
           double lotsProContract = 1)
        {
            desiMon = 0;
            volumeLotSize = 0;

            if (double.IsNaN(symbol.TickValue))
                return "Invalid TickValue";

            switch (profitMode)
            {
#if CTRADER
                case ProfitModes.Lots:
                    desiMon = CalcTicksAndLot2Money(symbol, tpPts, volumeLotSize = value);
                    break;

                case ProfitModes.Volume:
                    desiMon = CalcTicksAndVolume2Money(symbol, tpPts, volumeLotSize = value);
                    break;

                case ProfitModes.Contracts:
                    desiMon = CalcTicksAndVolume2Money(symbol, tpPts, volumeLotSize = value * lotsProContract);
                    break;
#else
                case ProfitModes.Lots:
                    desiMon = CalcTicksAndLot2Money(symbol, tpPts, volumeLotSize = value / lotsProContract);
                    break;

                case ProfitModes.Volume:
                    desiMon = CalcTicksAndVolume2Money(symbol, tpPts, volumeLotSize = value / lotsProContract);
                    break;

                case ProfitModes.Contracts:
                    desiMon = CalcTicksAndVolume2Money(symbol, tpPts, volumeLotSize = value);
                    break;
#endif
                //case ProfitMode.LotsPro10k:
                //volumeLotSize = (AbstractRobot.Account.Balance - AbstractRobot.Account.Margin) / 10000 * value;
                //desiMon = CalcTicksAndLot2Money(symbol, tpPts, volumeLotSize);
                //break;

                //case ProfitMode.ProfitPercent:
                //desiMon = (AbstractRobot.Account.Balance - AbstractRobot.Account.Margin) * value / 100;
                //volumeLotSize = CalcMoneyAndTicks2Lots(symbol, desiMon, tpPts, CommissionPerLot(symbol));
                //break;

                //case ProfitMode.ProfitAmmount:
                //volumeLotSize = CalcMoneyAndTicks2Lots(symbol, desiMon = value, tpPts, CommissionPerLot(symbol));
                //break;

                //case ProfitMode.RiskConstant:
                //case ProfitMode.RiskReinvest:
                //var balance = ProfitMode.RiskReinvest == profitMode
                //   ? AbstractRobot.Account.Balance
                //   : StartBalance;
                //double moneyToRisk = (balance - AbstractRobot.Account.Margin) * value / 100;
                //volumeLotSize = CalcMoneyAndTicks2Lots(symbol, moneyToRisk, riskTicks, CommissionPerLot(symbol));
                //desiMon = CalcTicksAndLot2Money(symbol, tpPts, volumeLotSize);
                //break;

                case ProfitModes.ConstantInvest:
                case ProfitModes.Reinvest:
                    var investMoney = (profitMode == ProfitModes.ConstantInvest
                       ? StartBalance
                       : mRobot.Account.Balance) * value / 100;
                    var units = investMoney * symbol.TickSize / symbol.TickValue / symbol.Bid;
                    volumeLotSize = symbol.VolumeInUnitsToQuantity(units);
                    desiMon = CalcTicksAndLot2Money(symbol, tpPts, volumeLotSize);
                    break;
            }
            return "";
        }

        public string CalcProfitMode2Volume(
           Symbol symbol,
           ProfitModes profitMode,
           double value,
           int tpPts,
           int riskTicks,
           out double desiMon,
           out double normalizedVolume)
        {
            var retVal = CalcProfitMode2Lots(symbol,
                profitMode,
                value,
                tpPts,
                riskTicks,
                out desiMon,
                out double rawVolume);

            normalizedVolume = symbol.NormalizeVolumeInUnits(
               symbol.QuantityToVolumeInUnits(
               //Math.Max(ParentBot.AbstractRobot.AbstractRobot.LotPoint(ParentBot.BotSymbol),
               rawVolume));

            return retVal;
        }

        public double CalcTicksAndLot2Money(Symbol symbol, int ticks, double lot)
        {
            return symbol.TickValue * ticks * symbol.LotSize * lot;
        }

        public double CalcTicksAndVolume2Money(Symbol symbol, int ticks, double volume)
        {
            return symbol.TickValue * ticks * volume;
        }

        public double CalcTicksAndContract2Money(Symbol symbol, int ticks, double volume)
        {
            return symbol.TickValue * ticks * volume;
        }

        public double Calc1TickAnd1Lot2Money(Symbol symbol, bool reverse = false)
        {
            var retVal = CalcTicksAndLot2Money(symbol, 1, 1);
            if (reverse)
                retVal *= symbol.Bid;
            return retVal;
        }

        public int CalcMoneyAndLot2Ticks(Symbol symbol, double money, double lot)
        {
            return (int)(0.5 + money / (lot * symbol.TickValue * symbol.LotSize));
        }

        public int CalcMoneyAndVolume2Ticks(Symbol symbol, double money, double volume)
        {
            return (int)(0.5 + money / (volume * symbol.TickValue));
        }

        public double CalcMoneyAndVolume2Price(Symbol symbol, double money, double volume)
        {
            return money / (volume * symbol.TickValue) * symbol.TickSize;
        }

        // https://ctrader.com/api/reference/internals/symbol/tickvalue
        // var normalizedVolume = ((Account.Balance*Risk)/StopLoss)/SymbolName.PointValue;
        public double CalcMoneyAndTicks2Lots(Symbol symbol, double money, int ticks, double xProLot)
        {
            double retVal = Math.Abs(money / (ticks * symbol.TickValue * symbol.LotSize + xProLot));
            retVal = Math.Max(retVal, MinLot(symbol));
            retVal = Math.Min(retVal, MaxLot(symbol));
            return retVal;
        }

        public void PrintComment(string version, string comment, int avgSpreadPts = -1, bool normNyTime = false)
        {
            // Warning: DrawStaticText causes exception when Optimizing !!!
            string sCurrency = " " + mRobot.Account.Asset.Name;
            string sCommission = ConvertUtils.DoubleToString(CommissionPerLot(mRobot.Symbol), 2) + sCurrency;
            var sSwapLong = ConvertUtils.DoubleToString(SwapPerLot(mRobot.Symbol, true), 2) + sCurrency;
            var sSwapShort = ConvertUtils.DoubleToString(SwapPerLot(mRobot.Symbol, false), 2) + sCurrency;

            //var com = AbstractRobot.SymbolName.com

            string sB2NyTime = "";
            DateTime tNyt = Time;   // CreateTime(TimeUtils.TimeUtc2Nyt(mCurrentTime.ToNativeSec(), false));

            if (normNyTime)
                tNyt = tNyt + TimeSpan.FromHours(7);

            string sNyTime = tNyt.ToString("dd/MM/yyyy HH:mm:ss");
            sB2NyTime = ", Broker-NYT: " + ConvertUtils.DoubleToString((Time - tNyt).TotalHours, 0) + "h";

            mRobot.Chart.DrawStaticText(
               "Comment1",
               cCommentTab + version,
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);

            var platform = IsCtrader ? "Lot" : "Contract";
            mRobot.Chart.DrawStaticText("Comment2",
               "\n" + cCommentTab + $"PointValue/{platform}: "
                        + ConvertUtils.DoubleToString(mRobot.Symbol.TickValue / mRobot.Symbol.TickSize * mRobot.Symbol.LotSize, 2) + sCurrency
               + ", Digits: " + mRobot.Symbol.Digits,
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);

            mRobot.Chart.DrawStaticText("Comment2.5",
                "\n\n" + cCommentTab + $"TickValue/{platform}: " + ConvertUtils.DoubleToString(
                    mRobot.Symbol.TickValue * mRobot.Symbol.LotSize, 2) + sCurrency
                + ", TickSize: " + ConvertUtils.DoubleToString(mRobot.Symbol.TickSize, mRobot.Symbol.Digits),
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);

            mRobot.Chart.DrawStaticText("Comment3",
               "\n\n\n" + cCommentTab + "Spread: " + ConvertUtils.IntegerToString(SpreadInPoints(mRobot.Symbol))
               + (-1 == avgSpreadPts ? "" : (", AvgSpread: " + ConvertUtils.IntegerToString(avgSpreadPts)))
#if CTRADER
               + ", MaxLot: " + ConvertUtils.DoubleToString(mRobot.Symbol.VolumeInUnitsToQuantity(mRobot.Symbol.VolumeInUnitsMax), 2)
#endif
               ,
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);
#if CTRADER
            mRobot.Chart.DrawStaticText(
               "Comment4",
               "\n\n\n\n" + cCommentTab + "Account-Leverage: 1:" + ConvertUtils.DoubleToString(mRobot.Account.PreciseLeverage, 0)
               //+ ", " + mAbstractRobot.SymbolName.Name + "-Leverage: " + sSymLev,  // cTrader does not have SymbolName-Leverages
               , VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);

            mRobot.Chart.DrawStaticText(
               "Comment5",
               "\n\n\n\n\n" + cCommentTab + "Commission/Lot: " + sCommission,
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);

            mRobot.Chart.DrawStaticText(
               "Comment6",
               "\n\n\n\n\n\n" + cCommentTab + "SwapLong/Lot: " + sSwapLong + ", SwapShort/Lot: " + sSwapShort,
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);

            /*mAbstractRobot.Chart.DrawStaticText(
               "Comment7",
               "\n\n\n\n\n\n" + cCommentTab + "New York Time" + (normNyTime ? " normalized: " : ": ") + sNyTime + sB2NyTime,
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mAbstractRobot.Chart.ColorSettings.ForegroundColor);*/
#endif
            string[] lines = comment.Split('\n');
            string skipLines = "";
            for (int i = 0; i < lines.Length; ++i)
            {
                var line = skipLines + lines[i];

                mRobot.Chart.DrawStaticText(
                   "Comment8" + ConvertUtils.IntegerToString(i),
                   "\n\n\n\n\n\n\n\n" + line,
                   VerticalAlignment.Top,
                   HorizontalAlignment.Left,
                   mRobot.Chart.ColorSettings.ForegroundColor);

                skipLines += "\n";
            }
        }

        public string MakeLogComment(Symbol symbol, string firstPart)
        {
            // comment must contain the following string:
            // orgComment;123456,aaa,+-ppp,yyy     meaning:
            // openAskInPts,openSpreadInPts,openPrevBidOrAskDiff
            return firstPart
               + ";" + ConvertUtils.IntegerToString(iPrice(symbol.Ask, symbol))
               + "," + ConvertUtils.IntegerToString(iPrice((symbol.Ask - symbol.Bid), symbol));
        }

        public virtual void OpenLogfile(ILogger logger,
           string filename = "",
           LogFlags mode = LogFlags.HeaderAndSeveralLines,
           string header = "")
        {
            mLogger = logger;
            mLogger.Mode = mode;
            if (mRobot.RunningMode != RunningMode.Optimization)
            {
                // if RealTime trading ==> append to existing file
                var logFile = "";
                if (0 != (mLogger.Mode & LogFlags.LogFile))
                    // returns filename if file already exists
                    logFile = mLogger.LogOpen(mLogger.MakeLogPath(), filename, mRobot.RunningMode == RunningMode.RealTime, mode);

                if ("" == logFile)
                    LoggerWriteHeader(header);
                else
                {
                    var firstLine = File.ReadLines(logFile).First();
                    mHeaderSplit = firstLine.Split(',');
                }
            }
        }

        public void LoggerWriteHeader(string header = "")
        {
            var logHeader = "";
            if (0 != (LogFlags.NoHeader & mLogger.Mode))
                return;

            if (0 != (LogFlags.SelfMade & mLogger.Mode))
                logHeader = header;
            else
            {
                logHeader += ("\nOpenDate"                                  // 1. open date; separate date and openTime... 
                   + ",OpenUTC"                                      // 2. open openTime; ...to show seconds in openTime 
                   + ",Symbol"                                              // 3. SymbolName
                   + ",Lots"                                                // 4. Lots
                   + ",OpenPrice"                                           // 5. open actual price
                   + ",Swap"                                                // 6. swap
                   + ",Swap/Lot"                                            // 7. swap per lot
                   + ",OpenAsk"                                             // 8. open ask
                   + ",OpenBid"                                             // 9. open bid
                   + ",OpenSpreadPts"                                       // 10. Spread in ticks at open
                );
                logHeader += (0 != (mLogger.Mode & LogFlags.OneLine) ? "," : ",\n");

                logHeader += ("CloseDate"                                   // 1. close date
                   + ",CloseUTC"                                     // 2. close openTime
                   + ",Mode"                                                // 3. Long/Short
                   + ",Volume"                                              // 4. normalizedVolume
                   + ",ClosePrice"                                          // 5. close actual price
                   + ",Commission"                                          // 6. commission
                   + ",Comm/Lot"                                            // 7. commission per lot
                   + ",CloseAsk"                                            // 8. close ask
                   + ",CloseBid"                                            // 9. close bid
                   + ",CloseSpreadPts"                                      // 10. Spread in ticks at close
                );
                logHeader += (0 != (mLogger.Mode & LogFlags.OneLine) ? "," : ",\n");

                logHeader += ("Number"                                      // 1. trade number
                   + ",Dur. d.h.m.s"                                        // 2. Tradeopen duration HH.mm:ss
                   + ",Saldo"                                               // 3. Saldo
                   + ",PointValue"                                          // 4. tickvalue
                   + ",DiffPts"                                             // 5. DiffActPts
                   + ",DiffGross"                                           // 6. DiffActCurrency
                   + ",NetProfit"                                    // 7. net profit
                   + ",NetProf/Lot"                                         // 8. net profit per lot
                   + ",AccountMargin"                                       // 9. Account Margin
                   + ",TradeMargin"                                         // 10. Margin of this single trade
                );
                //if (0 == (mLogger.Mode & OneLine)) logHeader += (",\n");
            }

            var backUp = mLogger.Mode;
            mLogger.Mode &= ~LogFlags.LogPrint;
            mLogger.AddText(logHeader);
            mLogger.Mode = backUp;
            mLogger.Flush();
            mHeaderSplit = logHeader.Split(',');
        }

        public void LoggerAddText(string s)
        {
            if (null == mLogger)
                return;

            mLogger.AddText(s);
        }

        public void LoggerClosingTrade(LogParams lp)
        {
            if (null == mLogger
                    || (0 != (mLogger.Mode & LogFlags.LogFile) && !mLogger.IsOpen)
                    || 0 != (mLogger.Mode & LogFlags.NoCloseLog)
                    || null == mHeaderSplit)
                return;

            // orgComment;123456,aaa,+-ppp     meaning:
            // openAskInPts,openSpreadInPts
            double openBid = 0, openAsk = 0;
            if (null != lp.Comment)
            {
                var bidAsks = lp.Comment.Split(';');
                if (bidAsks.Length >= 2)
                {
                    bidAsks = bidAsks[1].Split(',');
                    if (2 == bidAsks.Length)
                    {
                        int iAsk = ConvertUtils.StringToInteger(bidAsks[0]);
                        openAsk = lp.Symbol.TickSize * iAsk;
                        openBid = lp.Symbol.TickSize * (iAsk - ConvertUtils.StringToInteger(bidAsks[1]));
                    }
                }
            }

            double priceDiff = (TradeType.Buy == lp.TradeType ? 1 : -1)
               * (lp.ClosingPrice - lp.EntryPrice);
            int pointDiff = iPrice(priceDiff, lp.Symbol);
            var lotDigits = (int)(0.5 + Math.Log10(1 / lp.Minlots));
            mLoggingSaldo += lp.NetProfit;

            foreach (var part in mHeaderSplit)
            {
                var isComma = true;
                var changePart = part;
                if (part.Contains('\n'))
                {
                    changePart = part.Replace("\n", "");
                    isComma = false;
                }

                switch (changePart)
                {
                    case "OpenDate":
                        mLogger.AddText((isComma ? "," : "") + lp.EntryTime.ToString("yyyy.MM.dd"));
                        continue;

                    case "OpenUTC":
                        mLogger.AddText((isComma ? "," : "") + lp.EntryTime.ToString("HH:mm:ss"));
                        continue;

                    case "Symbol":
                        mLogger.AddText((isComma ? "," : "") + lp.Symbol.Name);
                        continue;

                    case "Lots":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.Lots, lotDigits));
                        continue;

                    case "OpenPrice":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.EntryPrice, lp.Symbol.Digits));
                        continue;

                    case "Swap":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.Swap, 2));
                        continue;

                    case "Swap/Lot":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.Swap / lp.Lots, 2));
                        continue;

                    case "OpenAsk":
                        mLogger.AddText((isComma ? "," : "") + (TradeType.Buy == lp.TradeType ? ConvertUtils.DoubleToString(openAsk, lp.Symbol.Digits) : ""));
                        continue;

                    case "OpenBid":
                        mLogger.AddText((isComma ? "," : "") + (TradeType.Sell == lp.TradeType ? ConvertUtils.DoubleToString(openBid, lp.Symbol.Digits) : ""));
                        continue;

                    case "OpenSpreadPts":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(iPrice((openAsk - openBid), lp.Symbol), 0));
                        continue;

                    case "CloseDate":
                        mLogger.AddText((isComma ? "," : "") + lp.ExitTime.ToString("yyyy.MM.dd"));
                        continue;

                    case "CloseUTC":
                        mLogger.AddText((isComma ? "," : "") + lp.ExitTime.ToString("HH:mm:ss"));
                        continue;

                    case "Mode":
                        mLogger.AddText((isComma ? "," : "")
                            + (TradeType.Sell == lp.TradeType ? "Short " : "Long ")
                            + lp.OrderType);
                        continue;

                    case "PointValue":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(Calc1TickAnd1Lot2Money(lp.Symbol), 5));
                        continue;

                    case "ClosePrice":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.ClosingPrice, lp.Symbol.Digits));
                        continue;

                    case "Commission":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.Commissions, 2));
                        continue;

                    case "Comm/Lot":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.Commissions / lp.Lots, 2));
                        continue;

                    case "CloseAsk":
                        mLogger.AddText((isComma ? "," : "") + (TradeType.Sell == lp.TradeType ? ConvertUtils.DoubleToString(GetBidAskPrice(lp.Symbol, BidAsk.Ask), lp.Symbol.Digits) : ""));
                        continue;

                    case "CloseBid":
                        mLogger.AddText((isComma ? "," : "") + (TradeType.Buy == lp.TradeType ? ConvertUtils.DoubleToString(GetBidAskPrice(lp.Symbol, BidAsk.Bid), lp.Symbol.Digits) : ""));
                        continue;

                    case "CloseSpreadPts":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(iPrice(GetBidAskPrice(lp.Symbol, BidAsk.Ask)
                           - GetBidAskPrice(lp.Symbol, BidAsk.Bid), lp.Symbol), 0));
                        continue;

                    case "Saldo":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(mLoggingSaldo, 2));
                        continue;

                    case "Dur. d.h.m.s":
                        mLogger.AddText((isComma ? "," : "") + (lp.EntryTime - lp.ExitTime).ToString(@"dd\:hh\:mm\:ss"));
                        continue;

                    case "Number":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.IntegerToString(++mLoggingTradeCount));
                        continue;

                    case "Volume":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.Volume, 1));
                        continue;

                    case "DiffPts":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(pointDiff, 0));
                        continue;

                    case "DiffGross":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(CalcTicksAndLot2Money(lp.Symbol, pointDiff, lp.Lots), 2));
                        continue;

                    case "NetProfit":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.NetProfit, 2));
                        continue;

                    case "NetProf/Lot":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.NetProfit / lp.Lots, 2));
                        continue;

                    case "AccountMargin":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.AccountMargin, 2));
                        continue;

                    case "TradeMargin":
                        mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(lp.TradeMargin, 2));
                        continue;

                    default:
                        break;
                }
            }
        }

        public void LoggerFlush()
        {
            if (null == mLogger)
                return;

            mLogger.Flush();
        }

        public void LoggerClose(string preText = "")
        {
            if (null == mLogger || !mLogger.IsOpen)
                return;

            if (RunningMode.RealTime == mRobot.RunningMode)
                preText = "";

            mLogger.Close(preText);
        }

        public abstract string GetSymbolTrail(string cTsymbol);

        public abstract string GetSymbolPlain(string cTsymbol);

        public abstract string DecodeStringFromLong(long lSymbol);

        static public int Tf2Secs(TimeFrame tf)
        {
            return SEC_PER_MINUTE * tf.GetHashCode();
        }

        static public TimeFrame Secs2Tf(int tfSecs, out bool isExact)
        {
#if CTRADER
            isExact = false;
            int closest = int.MaxValue;

            TimeFrame[] allTimeFrames = new TimeFrame[22];

            allTimeFrames[0] = TimeFrame.Minute;
            allTimeFrames[1] = TimeFrame.Minute2;
            allTimeFrames[2] = TimeFrame.Minute3;
            allTimeFrames[3] = TimeFrame.Minute4;
            allTimeFrames[4] = TimeFrame.Minute5;
            allTimeFrames[5] = TimeFrame.Minute6;
            allTimeFrames[6] = TimeFrame.Minute10;
            allTimeFrames[7] = TimeFrame.Minute15;
            allTimeFrames[8] = TimeFrame.Minute20;
            allTimeFrames[9] = TimeFrame.Minute30;
            allTimeFrames[10] = TimeFrame.Minute45;

            allTimeFrames[11] = TimeFrame.Hour;
            allTimeFrames[12] = TimeFrame.Hour2;
            allTimeFrames[13] = TimeFrame.Hour3;
            allTimeFrames[14] = TimeFrame.Hour4;
            allTimeFrames[15] = TimeFrame.Hour6;
            allTimeFrames[16] = TimeFrame.Hour8;
            allTimeFrames[17] = TimeFrame.Hour12;

            allTimeFrames[18] = TimeFrame.Daily;
            allTimeFrames[19] = TimeFrame.Day2;
            allTimeFrames[20] = TimeFrame.Weekly;
            allTimeFrames[21] = TimeFrame.Monthly;

            TimeFrame retVal = TimeFrame.Monthly;

            for (int i = 0; i < allTimeFrames.Length; i++)
            {
                TimeFrame tf = allTimeFrames[i];
                var diff = Math.Abs(Tf2Secs(tf) - tfSecs);
                if (diff < closest)
                {
                    retVal = tf;
                    closest = diff;
                    if (0 == diff)
                    {
                        isExact = true;
                        return retVal;
                    }
                }
            }
#else
            isExact = true;
            var retVal = new TimeFrame(tfSecs);
#endif
            return retVal;
        }

        public void DrawOnOpenedPosition(
           string symName,
           TradeType tt,
           DateTime entryTime, double entryPrice,
           string label)
        {
            if (!IsVisible || symName != mRobot.Symbol.Name)
                return;

            var isFreeze = label.ToLower().Contains("freeze");
            if (0 != entryPrice)
                mRobot.Chart.DrawIcon("OpenIcon" + entryTime.ToString(),
                   cTradeIcon[(int)tt],
                   entryTime,
                   entryPrice,
                   isFreeze ? Color.Gray : cTradeColor[(int)tt]).IsInteractive = mRobot.IsBacktesting;
        }

        public void DrawOnClosedPosition(
           string symName,
           TradeType tt,
           DateTime entryTime, double entryPrice,
           DateTime closeTime, double closePrice,
           bool profitLoss,
           string label)
        {
            if (!IsVisible || symName != mRobot.Symbol.Name)
                return;

            var isFreeze = label.ToLower().Contains("freeze");
            var icon = mRobot.Chart.DrawIcon(
               "CloseIcon" + entryTime.ToString(),
               isFreeze ? ChartIconType.Circle : ChartIconType.Diamond,
               closeTime,
               closePrice,
               isFreeze ? Color.Gray : (profitLoss ? Color.White : Color.Red));
            icon.IsInteractive = mRobot.IsBacktesting;
            icon.Comment = closeTime.ToString();

            if (0 != entryPrice)
            {
                var line = mRobot.Chart.DrawTrendLine(
                   "CloseLine" + entryTime.ToString(),
                   entryTime, entryPrice,
                   closeTime, closePrice,
                   isFreeze ? Color.Gray : cTradeColor[(int)tt], 1, LineStyle.Solid);
                line.IsInteractive = mRobot.IsBacktesting;
                line.Comment = closeTime.ToString();
            }
#if CTRADER
            var diamonds = mRobot.Chart.FindAllObjects(ChartObjectType.Icon);
            if (mRobot.Chart.FirstVisibleBarIndex < mRobot.Chart.LastVisibleBarIndex)
            {
                foreach (var tl in diamonds)
                    if ("" != tl.Comment)
                        if (DateTime.Parse(tl.Comment) < mRobot.Chart.Bars[mRobot.Chart.FirstVisibleBarIndex].OpenTime)
                            mRobot.Chart.RemoveObject(tl.Name);
                        else
                            break;

                var trendlines = mRobot.Chart.FindAllObjects(ChartObjectType.TrendLine);
                foreach (var tl in trendlines)
                    if ("" != tl.Comment)
                        if (DateTime.Parse(tl.Comment) < mRobot.Chart.Bars[mRobot.Chart.FirstVisibleBarIndex].OpenTime)
                            mRobot.Chart.RemoveObject(tl.Name);
                        else
                            break;
            }
#endif
        }

        public uint UiPrice(double dPrice, Symbol symbol)
        {
            return (uint)(0.5 + dPrice * Math.Pow(10, symbol.Digits));
        }

        public int iPrice(double dPrice, Symbol symbol)
        {
            return (int)(Math.Sign(dPrice) * (0.5 + Math.Abs(dPrice) * Math.Pow(10, symbol.Digits)));
        }

        public double dPrice(uint uiPrice, Symbol symbol)
        {
            return symbol.TickSize * uiPrice;
        }

        public int iTicks(double price, Symbol symbol)
        {
            return (int)(0.5 + price / symbol.TickSize);
        }

        public double dPips(double price, Symbol symbol)
        {
            return price / symbol.PipSize;
        }

        public double MathSign(double value)
        {
            return Math.Sign(value);
        }

        //public bool IsNewBar(int timeframe, DateTime current, DateTime prev)
        //{
        //   var longCurrent = current.ToNativeSec();
        //   var longPrev = prev.ToNativeSec();
        //   if (longCurrent <= 0 || longPrev <= 0) return false;
        //   return longPrev / timeframe != longCurrent / timeframe;
        //}

        public Position OpenTrade(
            Symbol symbol,
            TradeType tradeType,
            TradeDirections allowedDirection,
            string commentVersion,
            string label,
            double volume,
            double stopLoss,
            double takeProfit,
            CalculationMode calculationMode)
        {
            var isLong = tradeType == TradeType.Buy;
            if ((isLong && (allowedDirection == TradeDirections.na
                  || allowedDirection == TradeDirections.Long))
               || (!isLong && (allowedDirection == TradeDirections.na
                  || allowedDirection == TradeDirections.Short)))
            {
                var orderComment = MakeLogComment(symbol, commentVersion);
                volume = symbol.NormalizeVolumeInUnits(volume);
#if CTRADER
                // In cTrader sl/tp cannot be set in ExecuteMarketOrder() since only Pips are allowed there
                var result = mRobot.ExecuteMarketOrder(tradeType,
                    symbol.Name,
                    volume,
                    label,
                    null,
                    null,
                    orderComment);

                if (result.IsSuccessful)
                {
                    result = SetProtection(result.Position,
                        stopLoss,
                        takeProfit,
                        calculationMode);

                    if (result.IsSuccessful)
                        return result.Position;
                }
#else
                var result = mRobot.ExecuteMarketOrder(tradeType,
                    symbol.Name,
                    volume,
                    label,
                    stopLoss,
                    takeProfit,
                    orderComment,
                    calculationMode);
#endif
            }
            return null;
        }

        public Position OpenLimitTrade(
            Symbol symbol,
            TradeType tradeType,
            TradeDirections allowedDirection,
            string commentVersion,
            string label,
            double volume,
            double targetPrice,
            double? stopLossPrice,
            double? takeProfitPrice,
            CalculationMode calculationMode)
        {
            var isLong = tradeType == TradeType.Buy;
            if ((isLong && (allowedDirection == TradeDirections.na
                  || allowedDirection == TradeDirections.Long))
               || (!isLong && (allowedDirection == TradeDirections.na
                  || allowedDirection == TradeDirections.Short)))
            {
                volume = symbol.NormalizeVolumeInUnits(volume);
                var orderComment = MakeLogComment(symbol, commentVersion);
#if CTRADER
                var result = mRobot.PlaceLimitOrder(tradeType,
                    symbol.Name,
                    volume,
                    targetPrice,
                    label,
                    stopLossPrice,
                    takeProfitPrice,
                    ProtectionType.Absolute,
                    null,
                    orderComment);
#else
                var result = mRobot.PlaceLimitOrder(tradeType,
                    symbol.Name,
                    volume,
                    targetPrice,
                    label,
                    stopLossPrice,
                    takeProfitPrice,
                    calculationMode,
                    null,
                    orderComment);
#endif
                if (result.IsSuccessful)
                    return result.Position;
            }

            return null;
        }

        public TradeResult SetProtection(Position position,
            double? sl,
            double? tp,
            CalculationMode calculationMode)
        {
#if CTRADER
            if (CalculationMode.Currency == calculationMode)
            {
                if (sl.HasValue && 0 != sl)
                {
                    var slDiff = CalcMoneyAndVolume2Price(position.Symbol, sl.Value, position.VolumeInUnits);
                    sl = CoFu.SubLong(position.TradeType == TradeType.Buy, position.CurrentPrice, slDiff);
                }

                if (tp.HasValue && tp > 0)
                {
                    var tpDiff = CalcMoneyAndVolume2Price(position.Symbol, tp.Value, position.VolumeInUnits);
                    tp = CoFu.AddLong(position.TradeType == TradeType.Buy, position.CurrentPrice, tpDiff);
                }
            }

            var retVal = mRobot.ModifyPosition(position, sl, tp, ProtectionType.Absolute);

            if (-1 == tp)
                retVal = position.ModifyTrailingStop(true);

            return retVal;
#else
            if (-1 == tp)
                return mRobot.PlaceTrailProtection(position, position.NetProfit + sl.Value, calculationMode);
            else
                return mRobot.ModifyPosition(position, sl, tp, calculationMode);
#endif
        }
#if false
        public void SetTrailProtection(Position position,
            double trailDistance,
            CalculationMode calculationMode)
        {
#if CTRADER
            SetProtection(position, 0, null, calculationMode);
            position.ModifyTrailingStop(true);
#else
            mRobot.PlaceTrailProtection(position, trailDistance, calculationMode);
#endif
        }
#endif
        public TradeResult CloseTrade(Position position)
        {
            return position.Close();
        }

        public TradeResult CancelPending(PendingOrder pending)
        {
            return pending.Cancel();
        }

        public ChartStaticText TextXy(Strategy mBot, string text, int x, int y, Color color)
        {
            var xOffset = new string(' ', x);
            var yOffset = new string('\n', y);
            return mBot.Chart.DrawStaticText("StaticText" + x + '_' + y,
               yOffset + xOffset + text, VerticalAlignment.Top, HorizontalAlignment.Left, color);
        }

        public IQcBars GetQcBars(int barsSeconds,
            double cashPriceLevelSize,
            string symbolName,
            string SymbolPair = "")
        {
            IQcBars bars = null;
#if CTRADER
            if (SymbolPair.Contains(">>"))
            {
                var tickServerReader = TickServerDictionary.GetValueOrDefault(SymbolPair);
                if (default == tickServerReader)
                {
                    tickServerReader = new TickServerReader<TickserverMarketDataArgs>(this, SymbolPair);
                    TickServerDictionary.Add(SymbolPair, tickServerReader);
                }
                bars = new CtQcTickBars(this, barsSeconds, symbolName, SymbolPair, tickServerReader);
            }
            else
                bars = new CtOrgBars(this, barsSeconds, symbolName);
#else
            if (!mRobot.BarsDictionary.ContainsKey((barsSeconds, symbolName)))
            {
                var ntBars = new NtQcBars(mRobot, symbolName, barsSeconds, cashPriceLevelSize);
                mRobot.BarsDictionary.Add((barsSeconds, symbolName), ntBars);
                bars = ntBars;
            }
            else
                bars = mRobot.BarsDictionary[(barsSeconds, symbolName)];
#endif
            return bars;
        }

        public (double, double, double) GetProfitsPrice(Position position)
        {
            var trueOpenPrice = 0.0;
            var grossProfit = 0.0;
            var netProfit = 0.0;
            if (IsCtrader)
            {
                trueOpenPrice = double.Parse(position.Label.Split(',')[0], CultureInfo.InvariantCulture);
                grossProfit = Math.Round(CoFu.DiffLong(TradeType.Buy == position.TradeType,
                    position.CurrentPrice, trueOpenPrice)
                        * position.VolumeInUnits * position.Symbol.TickValue / position.Symbol.TickSize,
                    position.Symbol.Digits);
                netProfit = grossProfit + position.Commissions + position.Swap;
            }
            else
            {
                trueOpenPrice = position.EntryPrice;
                grossProfit = position.GrossProfit;
                netProfit = position.NetProfit;
            }

            return (grossProfit, netProfit, trueOpenPrice);
        }

        // returns R² of HistoricalTrade AccountNetProfits over the time
        // R² = 1 means perfect fit, R² = 0 means no fit
        public double CalculateGoodnessOfFit(History trades)
        {
            var sortedTrades = trades.OrderBy(t => t.ClosingTime).ToList();
            if (sortedTrades.Count < 2)
                return double.NaN;

            // X = time (double), Y = cumulative NetProfit
            var x = sortedTrades.Select(t => t.ClosingTime.ToOADate()).ToArray();
            var y = sortedTrades
                .Select(t => t.NetProfit)
                .Aggregate(new List<double>(), (acc, profit) =>
                {
                    acc.Add((acc.Count > 0 ? acc[acc.Count - 1] : 0) + profit);
                    return acc;
                })
                .ToArray();

            // Get regression parameters: intercept and slope
            // Moved from Math.Numerics to CoFu since NinjaTrader cannot work with Math.Numerics :-(
            var (intercept, slope) = CoFu.Fit(x, y);

            // Calculate predicted y-values
            var yPredicted = x.Select(xi => slope * xi + intercept);

            // Return R²
            // 1.0      = Perfect fit — all data points lie exactly on the regression line
            // 0.0      = No linear correlation — model explains none of the variation in the data
            // < 0.0    =  Worse than a horizontal line — model fits worse than using the mean
            return CoFu.RSquared(y, yPredicted);
        }
        #endregion

        #region cTrader Api
        public void Print(string message, params object[] parameters) => mRobot.Print(message + parameters);
        public Symbols Symbols => mRobot.Symbols;
        public DateTime Time => mRobot.Time;
        public Chart Chart => mRobot.Chart;
        public IAccount Account => mRobot.Account;
        #endregion
    }
}
