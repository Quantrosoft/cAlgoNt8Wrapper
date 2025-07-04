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
    public class CtQcPipeBars : IQcBars
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
        public bool IsNewBar => CoFu.IsNewBar(TimeFrameSeconds, mCtTime, mCtPrevTime);

        private string mSymbolPair;
        private bool mIsInit = true;
        private bool mIsCatchUp = true;
        private DateTime mCtTime;
        private DateTime mCtPrevTime;
        private TickServerReader<TickserverMarketDataArgs> mTickServer;
        TickserverMarketDataArgs mNtTick;
        private long mPeriodTicks;
        private DateTime mNtPrev;
        #endregion

        public CtQcPipeBars(int barPeriodSeconds,
            string symbolName,
            string symbolPair,
            DateTime from)
        {
            TimeFrameSeconds = barPeriodSeconds;
            mSymbolPair = symbolPair;

            TimeFrame = AbstractRobot.Secs2Tf(barPeriodSeconds, out _);
            OpenTimes = new CtQcPipeTimeSeries();
            BidOpenPrices = new CtQcPipeDataSeries();
            BidHighPrices = new CtQcPipeDataSeries();
            BidLowPrices = new CtQcPipeDataSeries();
            BidClosePrices = new CtQcPipeDataSeries();
            BidVolumes = new CtQcPipeDataSeries();
            AskOpenPrices = new CtQcPipeDataSeries();
            AskHighPrices = new CtQcPipeDataSeries();
            AskLowPrices = new CtQcPipeDataSeries();
            AskClosePrices = new CtQcPipeDataSeries();
            AskVolumes = new CtQcPipeDataSeries();

            mTickServer = new TickServerReader<TickserverMarketDataArgs>(symbolPair.Replace(" ", ""));
            mPeriodTicks = TimeFrameSeconds * TimeSpan.TicksPerSecond;

            OnTick(from, from);
        }

        public void OnTick(DateTime ctTime, DateTime ctPrevTime)
        {
            mCtTime = ctTime;
            var ctNative = ctTime.ToNativeSec();
            bool isNewCtBar = ctPrevTime <= CoFu.TimeInvalid
                || CoFu.IsNewBar(TimeFrameSeconds, ctTime, ctPrevTime);

            do
            {
                if (!mTickServer.TryPeek(out mNtTick))
                    return;

                if (mIsInit && mNtTick.Time > ctTime - TimeSpan.FromHours(12))
                    throw new Exception("Set start date of NinjaTrader at least 2 days earlier than cTrader");

                var ntNative = mNtTick.Time.ToNativeSec();
                bool isNewNtBar = CoFu.IsNewBar(TimeFrameSeconds, mNtTick.Time, mNtPrev);

                // If we have a new NT bar but not a new cTrader bar
                // then wait for cTrader to also have a new bar
                if (!mIsInit && !mIsCatchUp && isNewNtBar)
                    if (!isNewCtBar)
                        break;

                // When NT is ahead of cTrader, do not consume the NT tick but return
                // except both have a new bar
                if (ntNative > ctNative && !isNewCtBar && !isNewNtBar)
                    break;

                mTickServer.TryDequeue(out mNtTick);

                if (mIsInit || ((isNewCtBar || mIsCatchUp) && isNewNtBar))
                {
                    InitOpenBar(mNtTick);
                    mIsInit = false;
                }

                #region Bar update
                BidHighPrices.Swap(Math.Max(BidHighPrices.LastValue, mNtTick.Bid));
                BidLowPrices.Swap(Math.Min(BidLowPrices.LastValue, mNtTick.Bid));
                AskHighPrices.Swap(Math.Max(AskHighPrices.LastValue, mNtTick.Ask));
                AskLowPrices.Swap(Math.Min(AskLowPrices.LastValue, mNtTick.Ask));

                BidClosePrices.Swap(mNtTick.Bid);
                AskClosePrices.Swap(mNtTick.Ask);

                if (mNtTick.Ask != mNtTick.Bid)
                {
                    if (mNtTick.Price >= mNtTick.Ask)
                        BidVolumes.Swap(BidVolumes.LastValue + mNtTick.Volume);

                    if (mNtTick.Price <= mNtTick.Bid)
                        AskVolumes.Swap(AskVolumes.LastValue + mNtTick.Volume);
                }
                #endregion

                mNtPrev = mNtTick.Time;
            } while (mNtTick.Time.ToNativeSec() <= ctNative);

            mIsCatchUp = false;
        }

        public void OnStop()
        {
            Close();
        }

        private void InitOpenBar(TickserverMarketDataArgs args)
        {
            // init open stuff
            OpenTimes.Add(GetBarEntryTime(args.Time));

            BidOpenPrices.Add(args.Bid);
            AskOpenPrices.Add(args.Ask);

            BidHighPrices.Add(args.Bid);
            AskHighPrices.Add(args.Ask);
            BidLowPrices.Add(args.Bid);
            AskLowPrices.Add(args.Ask);

            BidClosePrices.Add(args.Bid);
            AskClosePrices.Add(args.Ask);

            BidVolumes.Add(0);
            AskVolumes.Add(0);
        }

        private DateTime GetBarEntryTime(DateTime time)
        {
            return new DateTime(time.Ticks - time.Ticks % mPeriodTicks);
        }

        private void Close()
        {
            mTickServer.Dispose();
        }
    }
}