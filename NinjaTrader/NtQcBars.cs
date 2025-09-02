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

using NinjaTrader.Data;
using System;
using TdsCommons;
using static TdsDefs;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class NtQcBars : IQcBars
    {
        #region Members
        public IQcTimeSeries OpenTimes { get; private set; }
        public IQcDataSeries BidOpenPrices { get; private set; }
        public IQcDataSeries BidHighPrices { get; private set; }
        public IQcDataSeries BidLowPrices { get; private set; }
        public IQcDataSeries BidClosePrices { get; private set; }
        public IQcVolumeSeries BidVolumes { get; private set; }
        public IQcDataSeries AskOpenPrices { get; private set; }
        public IQcDataSeries AskHighPrices { get; private set; }
        public IQcDataSeries AskLowPrices { get; private set; }
        public IQcDataSeries AskClosePrices { get; private set; }
        public IQcVolumeSeries AskVolumes { get; private set; }
        public int BarsBarIndex { get; private set; }
        public int TimeFrameSeconds { get; private set; }
        public bool IsNewBar { get; set; }
        public int Count => OpenTimes.Count;
        public string SymbolName => mSymbolName;

        public bool IsNewInternalBar { get; private set; }

        //     Gets the average prices data (Open + High + Low + Close) / 4.
        //public IQcDataSeries AveragePrices => mBars.AveragePrices;

        //     Gets the Median prices data (High + Low) / 2.
        //public IQcDataSeries MedianPrices;

        //     Gets the Typical prices data (High + Low + Close) / 3.
        //public IQcDataSeries TypicalPrices;

        //     Gets the Weighted prices data (High + Low + 2 * Close) / 4.
        //public IQcDataSeries WeightedPrices;

        internal const int TickReplaySize = 1000;
        public int LastBarsIndex = -1;
        public int BidBarsIndex = -1;
        public int AskBarsIndex = -1;
        public BarsPeriod BarsPeriod;
        public Strategy Robot;
        public int BarsSeconds;

        private DateTime mPrevTime;
        private string mSymbolName;
        private double mPriceLevelSize;
        #endregion

        public NtQcBars(Strategy robot,
            string symbolPair,
            int barPeriodSeconds,
            double cashPriceLevelSize)
        {
            Robot = robot;
            mSymbolName = symbolPair;
            BarsSeconds = barPeriodSeconds;
            BarsPeriod = new BarsPeriod();
            mPriceLevelSize = cashPriceLevelSize;

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
                    OpenTimes = new NtQcTimeSeries(this, Robot.Times[i]);

                    BidOpenPrices = new NtQcDataSeries(this, symbol, Robot.Opens[i]);
                    BidHighPrices = new NtQcDataSeries(this, symbol, Robot.Highs[i]);
                    BidLowPrices = new NtQcDataSeries(this, symbol, Robot.Lows[i]);
                    BidClosePrices = new NtQcDataSeries(this, symbol, Robot.Closes[i]);
                    BidVolumes = new NtQcVolumeSeries(this, symbol, BidAsk.Bid, Robot.Volumes[i],
                        mPriceLevelSize);

                    AskOpenPrices = new NtQcDataSeries(this, symbol, Robot.Opens[i]);
                    AskHighPrices = new NtQcDataSeries(this, symbol, Robot.Highs[i]);
                    AskLowPrices = new NtQcDataSeries(this, symbol, Robot.Lows[i]);
                    AskClosePrices = new NtQcDataSeries(this, symbol, Robot.Closes[i]);
                    AskVolumes = new NtQcVolumeSeries(this, symbol, BidAsk.Ask, Robot.Volumes[i],
                        mPriceLevelSize);

                    BarsBarIndex = i;
                    if (-1 != mPriceLevelSize)
                        symbol.SymbolBarIndex = i;
                }
            }
        }

        public void OnBarsMarketData()
        {
            IsNewInternalBar = (mPrevTime <= CoFu.TimeInvalid
                    || CoFu.IsNewBar(BarsSeconds,
                            Robot.MarketDataEventArgs.Time,
                            mPrevTime));

            // Postpone reset of clients new bar until bots OnTick was called
            if (!IsNewBar)
                IsNewBar = IsNewInternalBar;

            if (-1 != mPriceLevelSize)
            {
                OpenTimes.OnMarketData();

                BidOpenPrices.OnMarketData();
                BidHighPrices.OnMarketData();
                BidLowPrices.OnMarketData();
                BidClosePrices.OnMarketData();
                BidVolumes.OnMarketData();

                AskOpenPrices.OnMarketData();
                AskHighPrices.OnMarketData();
                AskLowPrices.OnMarketData();
                AskClosePrices.OnMarketData();
                AskVolumes.OnMarketData();
            }
            mPrevTime = Robot.MarketDataEventArgs.Time;
        }

        public override int GetHashCode()
        {
            return BarsSeconds / SEC_PER_MINUTE;
        }

        public void OnTick() { }
        public void Add(double value) { }
        public void Bump() { }
        public void Swap(double value) { }
        public void OnStop() { }
    }
}
