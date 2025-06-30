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
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TdsCommons;

namespace cAlgo.API
{
    public class CtQcMmfBars : IQcBars
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
        private DateTime mNtPrevTime;
        private DateTime mCtTime;
        private DateTime mCtPrevTime;
        private TickServer<TickserverMarketDataArgs> mTickServer;
        #endregion

        public CtQcMmfBars(int barPeriodSeconds,
            string symbolName,
            string symbolPair,
            DateTime from)
        {
            TimeFrameSeconds = barPeriodSeconds;
            mSymbolPair = symbolPair;

            TimeFrame = AbstractRobot.Secs2Tf(barPeriodSeconds, out _);
            OpenTimes = new CtQcMmfTimeSeries();
            BidOpenPrices = new CtQcMmfDataSeries();
            BidHighPrices = new CtQcMmfDataSeries();
            BidLowPrices = new CtQcMmfDataSeries();
            BidClosePrices = new CtQcMmfDataSeries();
            BidVolumes = new CtQcMmfDataSeries();
            AskOpenPrices = new CtQcMmfDataSeries();
            AskHighPrices = new CtQcMmfDataSeries();
            AskLowPrices = new CtQcMmfDataSeries();
            AskClosePrices = new CtQcMmfDataSeries();
            AskVolumes = new CtQcMmfDataSeries();

            mTickServer = new TickServer<TickserverMarketDataArgs>(symbolPair.Replace(" ", ""));
        }

        public void OnTick(DateTime fromTime, DateTime prevTime)
        {
            mCtTime = fromTime;

            var fromNative = fromTime.ToNativeSec();
            TickserverMarketDataArgs args = default;
            do
            {
                if (mIsInit || fromNative >= args.Time.ToNativeSec())
                {
                    if (!mIsInit)
                    {
                        BidHighPrices.Swap(Math.Max(BidHighPrices.LastValue, args.Bid));
                        BidLowPrices.Swap(Math.Min(BidLowPrices.LastValue, args.Bid));
                        AskHighPrices.Swap(Math.Max(AskHighPrices.LastValue, args.Ask));
                        AskLowPrices.Swap(Math.Min(AskLowPrices.LastValue, args.Ask));

                        BidClosePrices.Swap(args.Bid);
                        AskClosePrices.Swap(args.Ask);

                        // Accumulate volumes
                        if (args.Ask != args.Bid)   // do not add volume if prices are equal
                        {
                            if (args.Price >= args.Ask)
                                BidVolumes.Swap(BidVolumes.LastValue + args.Volume);

                            if (args.Price <= args.Bid)
                                AskVolumes.Swap(AskVolumes.LastValue + args.Volume);
                        }

                        mNtPrevTime = args.Time;
                    }

                    mTickServer.TryDequeue(out args);
                    fromTime = args.Time;

                    if (mIsInit || CoFu.IsNewBar(TimeFrameSeconds, args.Time, mNtPrevTime))
                    {
                        InitOpenBar(args);
                        mIsInit = false;
                    }
                }
            } while (args.Time.ToNativeSec() < fromNative);
        }

        private void InitOpenBar(TickserverMarketDataArgs args)
        {
            // init open stuff
            long periodTicks = TimeFrameSeconds * TimeSpan.TicksPerSecond;
            OpenTimes.Add(new DateTime(args.Time.Ticks
                - args.Time.Ticks % periodTicks));

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
        private void Close()
        {
            mTickServer.Dispose();
        }
    }
}