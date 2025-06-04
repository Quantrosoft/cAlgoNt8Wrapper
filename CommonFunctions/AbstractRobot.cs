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

#define REPURCHASE_NOT_ORDERSx

using cAlgo.API;
using cAlgo.API.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TdsCommons;
using static TdsDefs;

namespace RobotLib
{
    public enum ProfitMode
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

#if !CTRADER
    //     The protection type for orders and positions.
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
#endif

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
        Robot mBot;
        private int mPrevOverlayBarCount;
        private double mPrevOverlayData;

        public Drawings(Robot bot)
        {
            mBot = bot;
        }

        public void DrawOverlay(
           Ringbuffer<(DateTime, double)> data,
           string name,
           DateTime botCurrentTime,
           Color color,
           int thickness)
        {
            var isNewChartBar = mPrevOverlayBarCount != mBot.Bars.Count;
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
                       isInit ? data.Last(i + 1).Item1 : mBot.Bars.OpenTimes.Last((isNewChartBar ? 1 : 0)),
                       isInit ? data.Last(i + 1).Item2 : mPrevOverlayData,
                       isInit ? data.Last(i).Item1 : botCurrentTime,
                       data.Last(i).Item2,
                       color, thickness).IsInteractive = true;

                if (!isInit)
                    break;
            }

            if (isInit || isNewChartBar)
                mPrevOverlayData = data.Last(0).Item2;

            mPrevOverlayBarCount = mBot.Bars.Count;
        }

        public void DrawOverlay(Bars indiBars,
           DataSeries data,
           string name,
           DateTime botCurrentTime,
           Color color,
           int thickness,
           int offset = 0)
        {
            var isNewChartBar = mPrevOverlayBarCount != mBot.Bars.Count;
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
                          : mBot.Bars.OpenTimes.Last(isNewChartBar ? 1 + offset : 0),
                       isInit
                          ? data.Last(i + 1)
                          : mPrevOverlayData,
                       isInit
                          ? indiBars.OpenTimes.Last(i)
                          : isNewChartBar ? mBot.Bars.OpenTimes.Last(offset) : mBot.Time,
                       last,
                       color, thickness).IsInteractive = true;
                }
                if (!isInit)
                    break;
            }

            if (isInit || isNewChartBar)
                mPrevOverlayData = data.Last(isNewChartBar ? 1 : 0);

            mPrevOverlayBarCount = mBot.Bars.Count;
        }

        public void DrawNoOverlay(
           Bars dataBars,
           DataSeries data,
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

                            //if (mBot.Chart.TopY != mLastTopY || mBot.Chart.BottomY != mLastBottomY
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
            //mLastBottomY = mBot.Chart.BottomY;
            //mLastTopY = mBot.Chart.TopY;
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

    public abstract class AbstractRobot : IRobot
    {
        #region Members
        public TradingPlatform TradingPlatform { get; }
        public bool IsNinjaTrader { get; }
        public bool IsCtrader { get; }

        protected Robot mRobot; // MQL needs fully qualified path for Robot
        protected ILogger mLogger;
        protected bool mValidateTickData, mIsInit, mIsSwapLongInit, mIsSwapShortInit, mIsCommissionsInit, mIs1stTick;
        protected int mLoggingTradeCount;
        protected double mInitialAccountBalance, mLoggingSaldo, mSwapLong, mSwapShort, mCommissions;
        protected string mTimeZoneId, cCommentTab;
        protected DateTime mPrevTime, mCurrentTime, mInitialTime;
        protected Color[] cTradeColor;
        protected ChartIconType[] cTradeIcon;
        protected DateTime cInvalidTime;

        private string[] mHeaderSplit;
        private Dictionary<string, string> mSymbolDictionary = new Dictionary<string, string>
        {
            { "SURVEY", "Unknown" },
            { "MONKEY", "Unknown" },
            { "DASH", "Unknown" },        //{ "DASH", "DSHUSD" },
            { "CANOPY", "Unknown" },
            { "APHRIA", "Unknown" },
            { "GAZPROM", "Unknown" },
            { "NOVATEK", "Unknown" },
            { "SPOTIFY", "Unknown" },
            { "USDRUB", "Unknown" },
            { "EURRUB", "Unknown" },
            { "SLACK", "Unknown" },
            { "NORILSK", "Unknown" },
            { "NOLISK", "Unknown" },
            { "NICKEL", "Unknown" },
            { "RUSSIA50", "Unknown" },
            { "FACEBOOK", "Unknown" },
            { "RALPH", "Unknown" },
            { "LAUREN", "Unknown" },
            { "USDUPY", "USDJPY" },

            { "DOW", "US30" },            //{ "DOW", "YM" },
            { "APPLE", "AAPL.US" },       //{ "APPLE", "Apple" },
            { "BRENT", "SpotBrent" },     //{ "BRENT", "BRN" },
            //{ "BRENT", "Brent-F" },     //{ "BRENT", "BRN" },
            //{ "BRENT", "Cude-F" },     //{ "BRENT", "BRN" },
            { "BMW", "BMWd.DE" },         //{ "BMW", "BMW" },
            { "DEUTSCHE", "DBKd.DE" },    // { "DEUTSCHE", "Deutsche_Bank" }
            { "BANK", "DBKd.DE" },        // { "DEUTSCHE", "Deutsche_Bank" }
            { "VOLKSWAGEN", "VOWd.DE" },  // { "VOLKSWAGEN", "Volkswagen" }
            { "BAYER", "BAYNd.DE" },      // { "BAYER", "Bayer" }
            { "SAP", "SAPd.DE" },
            { "AMAZON", "AMZN.US" },      // { "AMAZON", "Amazom" }
            { "DAX", "GER40-F" },         // { "DAX", "FDAX" }
            { "NQ", "NAS100" },
            { "SP500", "US500" },
            { "S&P", "US500" },
            { "NASDAQ", "NAS100" },
            { "CAC", "FRA40" },
            { "SNAP", "SNAP.US" },
            { "AURORA", "ACB.US" },
            { "TILRAY", "TLRY.US" },
            { "CRONOS", "CRON.US" },
            { "TESLA", "TSLA.US" },
            { "AIRBUS", "AIR.FR" },
            { "KAFFEE", "Coffee" },
            { "KAKAO", "Cocoa" },
            { "WTI", "SpotCrude" },
            { "SUGAR", "Sugar" },

            { "NETFLIX", "NFLX.US" },
            { "DAIMLER", "MBGd.DE" },
            { "MASTERCARD", "MA.US" },
            { "BOEING", "BA.US" },
            { "BEOING", "BA.US" },
            { "AMERICAN", "AXP.US" },
            { "EXPRESS", "AXP.US" },
            { "MODERNA", "MRNA.US" },
            { "TRIPADVISOR", "TRIP.US" },
            { "STARBUCKS", "SBUX.US" },
            { "PINTEREST", "PINS.US" },
            { "GOOGLE", "GOOGL.US" },
            { "MCDONALDS", "MCD.US" },

            { "GOLD", "XAUUSD" },
            { "BITCOIN", "BTCUSD" },
            { "ETHEREUM", "ETHUSD" },
            { "RIPPLE", "XRPUSD" },
            { "LITECOIN", "LTCUSD" },
            { "FTSE", "UK100" },
            { "SILVER", "XAGUSD" },
            { "SILBER", "XAGUSD" },
      };
        private DataRateId mDataRateId;

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
            get { return mInitialAccountBalance; }
        }

        public DateTime InitialTime
        {
            get { return mInitialTime; }
        }

        public int SpreadInPoints(Symbol symbol)
        {
            return iPrice(symbol.Ask - symbol.Bid, symbol.TickSize);
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
        public virtual string ConfigInit(Robot robot, string timeZoneId)
        {
            mRobot = robot;
            mTimeZoneId = timeZoneId;
            mInitialAccountBalance = mRobot.Account.Balance;

            return "";
        }

        public virtual void DataLoadedInit()
        {
            mInitialTime = mRobot.Time;

            mIsInit = true;
            PreTick();
            PostTick();
            mIsInit = false;
            mIs1stTick = true;
        }

        public virtual void PreTick()
        {
            mCurrentTime = mRobot.Time.ManageTimeZones(mTimeZoneId, mIsInit);

            if (!mIsInit)
                if (DataRateId.Undefined == DataRateId)
                    if (mRobot.RunningMode == RunningMode.RealTime
                          || mCurrentTime - mPrevTime < TimeSpan.FromSeconds(50))
                        mDataRateId = DataRateId.Ticks;
                    else if (mCurrentTime - mPrevTime < TimeSpan.FromSeconds(115))
                        mDataRateId = DataRateId.Minutes;
                    else
                        mDataRateId = DataRateId.Timeframe;
        }

        public virtual void PostTick()
        {
            mIs1stTick = false;
            mPrevTime = mCurrentTime;
        }

        public double GetBidAskPrice(Symbol symbol, BidAsk bidAsk)
        {
            return bidAsk == BidAsk.Bid ? symbol.Bid : symbol.Ask;
        }

        public string CalcProfitMode2Lots(
           Symbol symbol,
           ProfitMode profitMode,
           double value,
           int tpPts,
           int riskPoints,
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
                case ProfitMode.Lots:
                desiMon = CalcPointsAndLot2Money(symbol, tpPts, volumeLotSize = value);
                break;

                case ProfitMode.Volume:
                desiMon = CalcPointsAndVolume2Money(symbol, tpPts, volumeLotSize = value);
                break;

                case ProfitMode.Contracts:
                desiMon = CalcPointsAndVolume2Money(symbol, tpPts, volumeLotSize = value * lotsProContract);
                break;
#else
                case ProfitMode.Lots:
                desiMon = CalcPointsAndLot2Money(symbol, tpPts, volumeLotSize = value / lotsProContract);
                break;

                case ProfitMode.Volume:
                desiMon = CalcPointsAndVolume2Money(symbol, tpPts, volumeLotSize = value / lotsProContract);
                break;

                case ProfitMode.Contracts:
                desiMon = CalcPointsAndVolume2Money(symbol, tpPts, volumeLotSize = value);
                break;
#endif
                //case ProfitMode.LotsPro10k:
                //volumeLotSize = (mRobot.Account.Balance - mRobot.Account.Margin) / 10000 * value;
                //desiMon = CalcPointsAndLot2Money(symbol, tpPts, volumeLotSize);
                //break;

                //case ProfitMode.ProfitPercent:
                //desiMon = (mRobot.Account.Balance - mRobot.Account.Margin) * value / 100;
                //volumeLotSize = CalcMoneyAndPoints2Lots(symbol, desiMon, tpPts, CommissionPerLot(symbol));
                //break;

                //case ProfitMode.ProfitAmmount:
                //volumeLotSize = CalcMoneyAndPoints2Lots(symbol, desiMon = value, tpPts, CommissionPerLot(symbol));
                //break;

                //case ProfitMode.RiskConstant:
                //case ProfitMode.RiskReinvest:
                //var balance = ProfitMode.RiskReinvest == profitMode
                //   ? mRobot.Account.Balance
                //   : mInitialAccountBalance;
                //double moneyToRisk = (balance - mRobot.Account.Margin) * value / 100;
                //volumeLotSize = CalcMoneyAndPoints2Lots(symbol, moneyToRisk, riskPoints, CommissionPerLot(symbol));
                //desiMon = CalcPointsAndLot2Money(symbol, tpPts, volumeLotSize);
                //break;

                case ProfitMode.ConstantInvest:
                case ProfitMode.Reinvest:
                var investMoney = (profitMode == ProfitMode.ConstantInvest
                   ? mInitialAccountBalance
                   : mRobot.Account.Balance) * value / 100;
                var units = investMoney * symbol.TickSize / symbol.TickValue / symbol.Bid;
                volumeLotSize = symbol.VolumeInUnitsToQuantity(units);
                desiMon = CalcPointsAndLot2Money(symbol, tpPts, volumeLotSize);
                break;
            }
            return "";
        }

        public string CalcProfitMode2Volume(
           Symbol symbol,
           ProfitMode profitMode,
           double value,
           int tpPts,
           int riskPoints,
           out double desiMon,
           out double normalizedVolume)
        {
            var retVal = CalcProfitMode2Lots(symbol,
                profitMode,
                value,
                tpPts,
                riskPoints,
                out desiMon,
                out double rawVolume);

            normalizedVolume = symbol.NormalizeVolumeInUnits(
               symbol.QuantityToVolumeInUnits(
               //Math.Max(ParentBot.mBot.mRobot.LotPoint(ParentBot.BotSymbol),
               rawVolume));

            return retVal;
        }

        public double CalcPointsAndLot2Money(Symbol symbol, int points, double lot)
        {
            return symbol.TickValue * points * symbol.LotSize * lot;
        }

        public double CalcPointsAndVolume2Money(Symbol symbol, int points, double volume)
        {
            return symbol.TickValue * points * volume;
        }

        public double CalcPointsAndContract2Money(Symbol symbol, int points, double volume)
        {
            return symbol.TickValue * points * volume;
        }

        public double Calc1PointAnd1Lot2Money(Symbol symbol, bool reverse = false)
        {
            var retVal = CalcPointsAndLot2Money(symbol, 1, 1);
            if (reverse)
                retVal *= symbol.Bid;
            return retVal;
        }

        public int CalcMoneyAndLot2Points(Symbol symbol, double money, double lot)
        {
            return (int)(0.5 + money / (lot * symbol.TickValue * symbol.LotSize));
        }

        public int CalcMoneyAndVolume2Points(Symbol symbol, double money, double volume)
        {
            return (int)(0.5 + money / (volume * symbol.TickValue));
        }

        // https://ctrader.com/api/reference/internals/symbol/tickvalue
        // var normalizedVolume = ((Account.Balance*Risk)/StopLoss)/SymbolName.PointValue;
        public double CalcMoneyAndPoints2Lots(Symbol symbol, double money, int points, double xProLot)
        {
            double retVal = Math.Abs(money / (points * symbol.TickValue * symbol.LotSize + xProLot));
            retVal = Math.Max(retVal, MinLot(symbol));
            retVal = Math.Min(retVal, MaxLot(symbol));
            return retVal;
        }

        public void PrintComment(string version, string comment, int avgSpreadPts, bool normNyTime = false)
        {
            // Warning: DrawStaticText causes exception when Optimizing !!!
            string sCurrency = " " + mRobot.Account.Asset.Name;
            string sCommission = ConvertUtils.DoubleToString(CommissionPerLot(mRobot.Symbol), 2) + sCurrency;
            var sSwapLong = ConvertUtils.DoubleToString(SwapPerLot(mRobot.Symbol, true), 2) + sCurrency;
            var sSwapShort = ConvertUtils.DoubleToString(SwapPerLot(mRobot.Symbol, false), 2) + sCurrency;

            //var com = mRobot.SymbolName.com

            string sB2NyTime = "";
            DateTime tNyt = mCurrentTime;// CreateTime(TimeUtils.TimeUtc2Nyt(mCurrentTime.ToNativeSec(), false));

            if (normNyTime)
                tNyt = tNyt + TimeSpan.FromHours(7);

            string sNyTime = tNyt.ToString("dd/MM/yyyy HH:mm:ss");
            sB2NyTime = ", Broker-NYT: " + ConvertUtils.DoubleToString((mCurrentTime - tNyt).TotalHours, 0) + "h";

            mRobot.Chart.DrawStaticText(
               "Comment1",
               cCommentTab + version,
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);

#if CTRADER
            var pointFactor = Math.Pow(10, mRobot.Symbol.Digits);
            var pointValue = mRobot.Symbol.TickValue * pointFactor; // TickValue is in USD per 1 Point
#else
            // public double TickValue => mRobot.Instrument.MasterInstrument.TickSize
            // * mRobot.Instrument.MasterInstrument.PointValue;
            var pointValue = mRobot.Symbol.TickValue / mRobot.Symbol.TickSize;
#endif
            mRobot.Chart.DrawStaticText(
               "Comment2",
               "\n" + cCommentTab + "PointValue: "
               + ConvertUtils.DoubleToString(pointValue, 2) + sCurrency
               + ", TickSize: " + ConvertUtils.DoubleToString(mRobot.Symbol.TickSize, mRobot.Symbol.Digits)
               + ", Digits: " + mRobot.Symbol.Digits,
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);

            mRobot.Chart.DrawStaticText("Comment3",
               "\n\n" + cCommentTab + "Spread: " + ConvertUtils.IntegerToString(SpreadInPoints(mRobot.Symbol)) // IntegerToString() needed for MT5 to avoid warnings
               + (-1 == avgSpreadPts ? "" : ", AvgSpread: " + ConvertUtils.IntegerToString(avgSpreadPts))
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
               "\n\n\n" + cCommentTab + "Account-Leverage: 1:" + ConvertUtils.DoubleToString(mRobot.Account.PreciseLeverage, 0)
               //+ ", " + mRobot.SymbolName.Name + "-Leverage: " + sSymLev,  // cTrader does not have SymbolName-Leverages
               , VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);

            mRobot.Chart.DrawStaticText(
               "Comment5",
               "\n\n\n\n" + cCommentTab + "Commission/Lot: " + sCommission,
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);

            mRobot.Chart.DrawStaticText(
               "Comment6",
               "\n\n\n\n\n" + cCommentTab + "SwapLong/Lot: " + sSwapLong + ", SwapShort/Lot: " + sSwapShort,
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);

            /*mRobot.Chart.DrawStaticText(
               "Comment7",
               "\n\n\n\n\n\n" + cCommentTab + "New York Time" + (normNyTime ? " normalized: " : ": ") + sNyTime + sB2NyTime,
               VerticalAlignment.Top,
               HorizontalAlignment.Left,
               mRobot.Chart.ColorSettings.ForegroundColor);*/
#endif
            string[] lines = comment.Split('\n');
            string skipLines = "";
            for (int i = 0; i < lines.Length; ++i)
            {
                var line = skipLines + lines[i];

                mRobot.Chart.DrawStaticText(
                   "Comment8" + ConvertUtils.IntegerToString(i),
                   "\n\n\n\n\n\n\n" + line,
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
               + ";" + ConvertUtils.IntegerToString(iPrice(symbol.Ask, symbol.TickSize))
               + "," + ConvertUtils.IntegerToString(iPrice((symbol.Ask - symbol.Bid), symbol.TickSize));
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
                   + ",OpenSpreadPts"                                       // 10. Spread in points at open
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
                   + ",CloseSpreadPts"                                      // 10. Spread in points at close
                );
                logHeader += (0 != (mLogger.Mode & LogFlags.OneLine) ? "," : ",\n");

                logHeader += ("Number"                                      // 1. trade number
                   + ",Dur. d.h.m.s"                                        // 2. Tradeopen duration HH.mm:ss
                   + ",Saldo"                                               // 3. Saldo
                   + ",PointValue"                                          // 4. tickvalue
                   + ",DiffPts"                                             // 5. DiffActPts
                   + ",DiffGross"                                           // 6. DiffActCurrency
                   + ",NetProfit"                                           // 7. net profit
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
            int pointDiff = iPrice(priceDiff, lp.Symbol.TickSize);
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
                    mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(iPrice((openAsk - openBid), lp.Symbol.TickSize), 0));
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
                    mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(Calc1PointAnd1Lot2Money(lp.Symbol), 5));
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
                       - GetBidAskPrice(lp.Symbol, BidAsk.Bid), lp.Symbol.TickSize), 0));
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
                    mLogger.AddText((isComma ? "," : "") + ConvertUtils.DoubleToString(CalcPointsAndLot2Money(lp.Symbol, pointDiff, lp.Lots), 2));
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

        public void LoggerClose(string preText)
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

        protected bool IsNewBar(int timeframe)
        {
            return mPrevTime.ToNativeSec() / (int)timeframe != mCurrentTime.ToNativeSec() / timeframe;
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

        public uint UiPrice(double dPrice, double tickSize)
        {
            return (uint)(0.5 + dPrice / tickSize);
        }

        public int iPrice(double dPrice, double tickSize)
        {
            return (int)(Math.Sign(dPrice) * (0.5 + Math.Abs(dPrice) / tickSize));
        }

        public double dPrice(uint uiPrice, double tickSize)
        {
            return tickSize * uiPrice;
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
            double volume)
        {
            if ((tradeType == TradeType.Buy && (allowedDirection == TradeDirections.FromConfigFiles
                  || allowedDirection == TradeDirections.Long))
               || (tradeType == TradeType.Sell && (allowedDirection == TradeDirections.FromConfigFiles
                  || allowedDirection == TradeDirections.Short)))
            {
                var orderComment = MakeLogComment(symbol, commentVersion);
                volume = symbol.NormalizeVolumeInUnits(volume);
                var result = mRobot.ExecuteMarketOrder(tradeType, symbol.Name, volume, label, 0, 0, orderComment);
                if (result.IsSuccessful)
                    return result.Position;
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
            double targetPrice)
        {
            if ((tradeType == TradeType.Buy && (allowedDirection == TradeDirections.FromConfigFiles
                  || allowedDirection == TradeDirections.Long))
               || (tradeType == TradeType.Sell && (allowedDirection == TradeDirections.FromConfigFiles
                  || allowedDirection == TradeDirections.Short)))
            {
                var orderComment = MakeLogComment(symbol, commentVersion);
                volume = symbol.NormalizeVolumeInUnits(volume);
                var result = mRobot.PlaceLimitOrder(tradeType,
                    symbol.Name,
                    volume,
                    targetPrice,
                    label,
                    0,
                    0,
                    ProtectionType.None,
                    null,
                    orderComment);

                if (result.IsSuccessful)
                    return result.Position;
            }
            return null;
        }

        public TradeResult CloseTrade(Position pos)
        {
            return pos.Close();
        }

        public TradeResult CancelPending(PendingOrder pending)
        {
            return pending.Cancel();
        }

        public ChartStaticText TextXy(Robot mBot, string text, int x, int y, Color color)
        {
            var xOffset = new string(' ', x);
            var yOffset = new string('\n', y);
            return mBot.Chart.DrawStaticText("StaticText" + x + '_' + y,
               yOffset + xOffset + text, VerticalAlignment.Top, HorizontalAlignment.Left, color);
        }
#endregion

        #region cTrader Api
        public void Print(string message, params object[] parameters) => mRobot.Print(message + parameters);
        public void Stop() => mRobot.Stop();

        public Symbols Symbols => mRobot.Symbols;
        public MarketData MarketData => mRobot.MarketData;
        public DateTime Time => mRobot.Time;
        public Chart Chart => mRobot.Chart;
        public Bars Bars => mRobot.Bars;
        public IAccount Account => mRobot.Account;
        #endregion
    }
}
