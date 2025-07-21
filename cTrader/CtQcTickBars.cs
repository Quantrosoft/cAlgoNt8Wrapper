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

using RobotLib;
using System;
using TdsCommons;

namespace cAlgo.API
{
    public class CtQcTickBars : IQcBars
    {
        #region Members
        public const int QcBarsSize = 1000;
        public IQcTimeSeries OpenTimes { get; internal set; }
        public IQcDataSeries BidOpenPrices { get; internal set; }
        public IQcDataSeries BidHighPrices { get; internal set; }
        public IQcDataSeries BidLowPrices { get; internal set; }
        public IQcDataSeries BidClosePrices { get; internal set; }
        public IQcDataSeries BidVolumes { get; internal set; }
        public IQcDataSeries AskOpenPrices { get; internal set; }
        public IQcDataSeries AskHighPrices { get; internal set; }
        public IQcDataSeries AskLowPrices { get; internal set; }
        public IQcDataSeries AskClosePrices { get; internal set; }
        public IQcDataSeries AskVolumes { get; internal set; }
        public int TimeFrameSeconds { get; internal set; }
        public TimeFrame TimeFrame { get; internal set; }
        public int Count => OpenTimes.Count;
        public string SymbolName => mSymbolPair;
        public bool IsNewBar => CoFu.IsNewBar(TimeFrameSeconds, mBot.Time, mBot.PrevTime);

        private string mSymbolPair;
        private TickServerReader<TickserverMarketDataArgs> mTickServerReader;
        private long mPeriodTicks;
        private DateTime mNtPrevTime;
        private AbstractRobot mBot;
        #endregion

        public CtQcTickBars(AbstractRobot abstractRobot,
            int barPeriodSeconds,
            string symbolName,
            string symbolPair,
            TickServerReader<TickserverMarketDataArgs> tickServerReader)
        {
            mBot = abstractRobot;
            TimeFrameSeconds = barPeriodSeconds;
            mSymbolPair = symbolPair;
            mTickServerReader = tickServerReader;

            TimeFrame = AbstractRobot.Secs2Tf(barPeriodSeconds, out _);
            OpenTimes = new CtQcTickTimeSeries();
            BidOpenPrices = new CtQcTickDataSeries();
            BidHighPrices = new CtQcTickDataSeries();
            BidLowPrices = new CtQcTickDataSeries();
            BidClosePrices = new CtQcTickDataSeries();
            BidVolumes = new CtQcTickDataSeries();
            AskOpenPrices = new CtQcTickDataSeries();
            AskHighPrices = new CtQcTickDataSeries();
            AskLowPrices = new CtQcTickDataSeries();
            AskClosePrices = new CtQcTickDataSeries();
            AskVolumes = new CtQcTickDataSeries();

            mPeriodTicks = TimeFrameSeconds * TimeSpan.TicksPerSecond;

            mTickServerReader.RegisterBar(OnTick);
        }

        public void OnTick()
        {
            UpdateNtBar();
            mNtPrevTime = mTickServerReader.ServerTick.Time;
        }

        public void OnStop()
        {
            // Nothing to do here, all done in TickServerReader.Dispose()
        }

        private void UpdateNtBar()
        {
            if (mNtPrevTime <= CoFu.TimeInvalid 
                || CoFu.IsNewBar(TimeFrameSeconds, mTickServerReader.ServerTick.Time, mNtPrevTime))
            {
                // init open stuff
                OpenTimes.Add(GetBarEntryTime(mTickServerReader.ServerTick.Time));

                BidOpenPrices.Add(mTickServerReader.ServerTick.Bid);
                AskOpenPrices.Add(mTickServerReader.ServerTick.Ask);

                BidHighPrices.Add(mTickServerReader.ServerTick.Bid);
                AskHighPrices.Add(mTickServerReader.ServerTick.Ask);
                BidLowPrices.Add(mTickServerReader.ServerTick.Bid);
                AskLowPrices.Add(mTickServerReader.ServerTick.Ask);

                BidClosePrices.Add(mTickServerReader.ServerTick.Bid);
                AskClosePrices.Add(mTickServerReader.ServerTick.Ask);

                BidVolumes.Add(0);
                AskVolumes.Add(0);
            }

            #region Bar update
            BidHighPrices.Swap(Math.Max(BidHighPrices.LastValue, mTickServerReader.ServerTick.Bid));
            BidLowPrices.Swap(Math.Min(BidLowPrices.LastValue, mTickServerReader.ServerTick.Bid));
            AskHighPrices.Swap(Math.Max(AskHighPrices.LastValue, mTickServerReader.ServerTick.Ask));
            AskLowPrices.Swap(Math.Min(AskLowPrices.LastValue, mTickServerReader.ServerTick.Ask));

            BidClosePrices.Swap(mTickServerReader.ServerTick.Bid);
            AskClosePrices.Swap(mTickServerReader.ServerTick.Ask);

            if (mTickServerReader.ServerTick.Ask != mTickServerReader.ServerTick.Bid)
            {
                if (mTickServerReader.ServerTick.Price >= mTickServerReader.ServerTick.Ask)
                    AskVolumes.Swap(AskVolumes.LastValue + mTickServerReader.ServerTick.Volume);

                if (mTickServerReader.ServerTick.Price <= mTickServerReader.ServerTick.Bid)
                    BidVolumes.Swap(BidVolumes.LastValue + mTickServerReader.ServerTick.Volume);
            }
            #endregion
        }

        private DateTime GetBarEntryTime(DateTime time)
        {
            return new DateTime(time.Ticks - time.Ticks % mPeriodTicks);
        }
    }
}