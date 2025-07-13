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
        public bool IsNewBar => mIsNewServerBar;

        private string mSymbolPair;
        TickserverMarketDataArgs mServerTick;
        private long mPeriodTicks;
        private long mNtPrev;
        private bool mIsNewServerBar;
        #endregion

        public CtQcTickBars(int barPeriodSeconds,
            string symbolName,
            string symbolPair,
            DateTime from,
            TickServerReader<TickserverMarketDataArgs> tickServerReader)
        {
            TimeFrameSeconds = barPeriodSeconds;
            mSymbolPair = symbolPair;
            tickServerReader.RegisterBar(OnTick);

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
        }

        public void OnTick(DateTime time, DateTime _)
        {
            var ntNative = time.ToNativeSec();
            UpdateNtBar(0 == mNtPrev || CoFu.IsNewBar(TimeFrameSeconds, ntNative, mNtPrev));
        }

        public void OnStop() 
        {
            // Nothing to do here, all done in TickServerReader
        }

        private void UpdateNtBar(bool isNewBar)
        {
            if (isNewBar)
            {
                // init open stuff
                OpenTimes.Add(GetBarEntryTime(mServerTick.Time));

                BidOpenPrices.Add(mServerTick.Bid);
                AskOpenPrices.Add(mServerTick.Ask);

                BidHighPrices.Add(mServerTick.Bid);
                AskHighPrices.Add(mServerTick.Ask);
                BidLowPrices.Add(mServerTick.Bid);
                AskLowPrices.Add(mServerTick.Ask);

                BidClosePrices.Add(mServerTick.Bid);
                AskClosePrices.Add(mServerTick.Ask);

                BidVolumes.Add(0);
                AskVolumes.Add(0);
            }

            #region Bar update
            BidHighPrices.Swap(Math.Max(BidHighPrices.LastValue, mServerTick.Bid));
            BidLowPrices.Swap(Math.Min(BidLowPrices.LastValue, mServerTick.Bid));
            AskHighPrices.Swap(Math.Max(AskHighPrices.LastValue, mServerTick.Ask));
            AskLowPrices.Swap(Math.Min(AskLowPrices.LastValue, mServerTick.Ask));

            BidClosePrices.Swap(mServerTick.Bid);
            AskClosePrices.Swap(mServerTick.Ask);

            if (mServerTick.Ask != mServerTick.Bid)
            {
                if (mServerTick.Price >= mServerTick.Ask)
                    AskVolumes.Swap(AskVolumes.LastValue + mServerTick.Volume);

                if (mServerTick.Price <= mServerTick.Bid)
                    BidVolumes.Swap(BidVolumes.LastValue + mServerTick.Volume);
            }
            #endregion
        }

        private DateTime GetBarEntryTime(DateTime time)
        {
            return new DateTime(time.Ticks - time.Ticks % mPeriodTicks);
        }
    }
}