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
        private bool mIsInit = true;
        private TickServerReader<TickserverMarketDataArgs> mTickServer;
        TickserverMarketDataArgs mServerTick;
        private long mPeriodTicks;
        private long mNtPrev;
        private ulong mMessageId;
        private bool mIsNewServerBar;
        #endregion

        public CtQcTickBars(int barPeriodSeconds,
            string symbolName,
            string symbolPair,
            DateTime from)
        {
            TimeFrameSeconds = barPeriodSeconds;
            mSymbolPair = symbolPair;

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

            mTickServer = new TickServerReader<TickserverMarketDataArgs>(symbolPair.Replace(" ", ""));
            mPeriodTicks = TimeFrameSeconds * TimeSpan.TicksPerSecond;

            OnTick(from, from);
        }

        public void OnTick(DateTime ctTime, DateTime ctPrevTime)
        {
            var ctNative = ctTime.ToNativeSec();
            var isNewCtBar = ctPrevTime <= CoFu.TimeInvalid
                || CoFu.IsNewBar(TimeFrameSeconds, ctTime, ctPrevTime);

            long ntNative;
            do
            {
                if (!mTickServer.TryPeek(out mServerTick))
                    return;

                if (mIsInit && mServerTick.Time > ctTime - TimeSpan.FromSeconds(10 * TimeFrameSeconds))
                    throw new Exception("Set start date of NinjaTrader earlier than cTrader");

                ntNative = mServerTick.Time.ToNativeSec();
                var isNewServerBar = CoFu.IsNewBar(TimeFrameSeconds, ntNative, mNtPrev);
                mIsNewServerBar = isNewServerBar && ntNative == ctNative;

                // When NT is ahead of cTrader, do not consume the NT tick but return
                // except both have a new bar
                if (ntNative > ctNative)
                    break;

                mTickServer.TryDequeue(out mServerTick);
                UpdateNtBar(mIsInit || isNewServerBar);
                if (mServerTick.MessageId != mMessageId++)
                { }

                mNtPrev = ntNative;
            } while (ntNative < ctNative);  // loop til NinjaTrader time is >= cTrader time

            if (ntNative != ctNative)
            { }
            // When we are leaving here, NinjaTrader time is >= cTrader time
        }

        private void UpdateNtBar(bool isNewBar)
        {
            if (isNewBar)
            {
                InitOpenBar(mServerTick);
                mIsInit = false;
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

        public void OnStop()
        {
            Close();
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