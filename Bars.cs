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

using NinjaTrader.Data;
using NinjaTrader.Gui.NinjaScript;
using System;
using TdsCommons;
using static TdsDefs;

namespace cAlgo.API
{
    public class Bars
    {
        internal const int TickReplaySize = 1000;
        public int LastBarsIndex = -1;
        public int BidBarsIndex = -1;
        public int AskBarsIndex = -1;
        public BarsPeriod BarsPeriod;
        public Robot Robot;
        public int BarsSeconds;

        private DateTime mPrevTime;
        private string mSymbolName;
        private TimeFrame mTimeFrame;
        private StrategyRenderBase mStrategy;

        public Bars(Robot robot, TimeFrame timeFrame, string symbolName)
        {
            Robot = robot;
            mStrategy = robot;
            mTimeFrame = timeFrame;
            BarsSeconds = timeFrame.GetPeriodSeconds();
            mSymbolName = symbolName;
            BarsPeriod = new BarsPeriod();

            if (BarsSeconds >= SEC_PER_MINUTE)
            {
                var minutes = BarsSeconds / SEC_PER_MINUTE;
                BarsPeriod.BarsPeriodType = BarsPeriodType.Minute;
                BarsPeriod.Value = minutes;
            }
            else
            {
                BarsPeriod.BarsPeriodType = BarsPeriodType.Second;
                BarsPeriod.Value = BarsSeconds;
            }
        }

        public void OnBarsDataLoaded()
        {
            if (!Robot.IsTickReplay)   // With IsTickReplay there only can be MarketDataType.Last
            {
                // If not IsTickReplay, the primary data series must be based on Bid
                if (Robot.BarsArray[0].BarsPeriod.MarketDataType != MarketDataType.Bid)
                    throw new Exception($"Error: Primary Data Series must be based on Bid w/o TickReplay or Last w. TickReplay");
            }

            int i = 0;
            for (; i < Robot.BarsArray.Length; i++)
            {
                var ninjaBarsSeconds = 0;
                switch (Robot.BarsArray[i].BarsPeriod.BarsPeriodType)
                {
                    case BarsPeriodType.Second:
                    ninjaBarsSeconds = Robot.BarsArray[i].BarsPeriod.Value;
                    break;

                    case BarsPeriodType.Minute:
                    ninjaBarsSeconds = Robot.BarsArray[i].BarsPeriod.Value * SEC_PER_MINUTE;
                    break;

                    case BarsPeriodType.Day:
                    ninjaBarsSeconds = Robot.BarsArray[i].BarsPeriod.Value * SEC_PER_DAY;
                    break;

                    case BarsPeriodType.Week:
                    ninjaBarsSeconds = Robot.BarsArray[i].BarsPeriod.Value * SEC_PER_WEEK;
                    break;

                    case BarsPeriodType.Month:
                    ninjaBarsSeconds = Robot.BarsArray[i].BarsPeriod.Value * 30 * SEC_PER_DAY; // Approximate (30 days)
                    break;

                    default:
                    ninjaBarsSeconds = -1; // Unknown or non-time-based type (e.g., Tick, Volume)
                    break;
                }

                if (Robot.BarsArray[i].Instrument.FullName == mSymbolName
                        && ninjaBarsSeconds == BarsSeconds)
                {
                    switch (Robot.BarsArray[i].BarsPeriod.MarketDataType)
                    {
                        case MarketDataType.Last:
                        LastBarsIndex = i;
                        break;

                        case MarketDataType.Bid:
                        BidBarsIndex = i;
                        break;

                        case MarketDataType.Ask:
                        AskBarsIndex = i;
                        break;
                    }

                    var symbol = Robot.Symbols.SymbolDictionary[mSymbolName];
                    OpenTimes = new TimeSeries(this, Robot.Times[i]);
                    OpenPrices = new DataSeries(this, symbol, Robot.Opens[i]);
                    HighPrices = new DataSeries(this, symbol, Robot.Highs[i]);
                    LowPrices = new DataSeries(this, symbol, Robot.Lows[i]);
                    ClosePrices = new DataSeries(this, symbol, Robot.Closes[i]);
                    TickVolumes = new VolumeSeries(this, Robot.Volumes[i]);

                    symbol.SymbolBarIndex = i;
                }
            }
        }

        public void OnBarsMarketData()
        {
            IsNewBar = CoFu.IsNewBar(BarsSeconds, Robot.MarketDataEventArgs.Time, mPrevTime)
                || mPrevTime <= CoFu.TimeInvalid;

            OpenTimes.OnMarketData();
            OpenPrices.OnMarketData();
            HighPrices.OnMarketData();
            LowPrices.OnMarketData();
            ClosePrices.OnMarketData();
            TickVolumes.OnMarketData();

            mPrevTime = Robot.MarketDataEventArgs.Time;
        }

        //
        // Summary:
        //     Gets the number of bars.
        public int Count => OpenTimes.Count;

        //
        // Summary:
        //     Get the timeframe.
        public TimeFrame TimeFrame => mTimeFrame;

        //
        // Summary:
        //     Gets the symbol name.
        public string SymbolName => mSymbolName;

        public bool IsNewBar { get; private set; }

        //
        // Summary:
        //     Gets the Open price bars data.
        public DataSeries OpenPrices;

        //
        // Summary:
        //     Gets the High price bars data.
        public DataSeries HighPrices;

        //
        // Summary:
        //     Gets the Low price bars data.
        public DataSeries LowPrices;

        //
        // Summary:
        //     Gets the Close price bars data.
        public DataSeries ClosePrices;

        //
        // Summary:
        //     Gets the Tick volumes data.
        public VolumeSeries TickVolumes;

        //
        // Summary:
        //     Gets the open bar time data.
        public TimeSeries OpenTimes;

        //
        // Summary:
        //     Gets the average prices data (Open + High + Low + Close) / 4.
        //public DataSeries AveragePrices => mBars.AveragePrices;

        //
        // Summary:
        //     Gets the Median prices data (High + Low) / 2.
        //public DataSeries MedianPrices;

        //
        // Summary: 
        //     Gets the Typical prices data (High + Low + Close) / 3.
        //public DataSeries TypicalPrices;

        //
        // Summary:
        //     Gets the Weighted prices data (High + Low + 2 * Close) / 4.
        //public DataSeries WeightedPrices;


        public int GetBar(DateTime time) => Robot.Bars.GetBar(time);

        public override int GetHashCode()
        {
            return BarsSeconds / SEC_PER_MINUTE;
        }
    }
}
