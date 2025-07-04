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

#if CTRADER // ToDo: Put all cTrader only files in a separate dll; out of cAlgoNt8Wrapper
using cAlgo.API;
using RobotLib;
using System;

namespace TdsCommons
{
    public class CtOrgBars : IQcBars
    {
        #region Members
        public IQcTimeSeries OpenTimes { get; }
        public IQcDataSeries BidOpenPrices { get; }
        public IQcDataSeries BidHighPrices { get; }
        public IQcDataSeries BidLowPrices { get; }
        public IQcDataSeries BidClosePrices { get; }
        public IQcDataSeries BidVolumes { get; }
        public IQcDataSeries AskOpenPrices { get; }
        public IQcDataSeries AskHighPrices { get; }
        public IQcDataSeries AskLowPrices { get; }
        public IQcDataSeries AskClosePrices { get; }
        public IQcDataSeries AskVolumes { get; }
        public int TimeFrameSeconds { get; }
        public int Count => OpenTimes.Count;
        public string SymbolName => mSymbol;
        public bool IsNewBar { get; private set; }

        private TimeFrame mTimeFrame;
        private Robot mBot;
        private string mSymbol;
        private Bars mBars;
        #endregion

        public CtOrgBars(int barPeriodSeconds, string symbol, Robot robot)
        {
            TimeFrameSeconds = barPeriodSeconds;
            mSymbol = symbol;
            mBot = robot;
            mTimeFrame = AbstractRobot.Secs2Tf(barPeriodSeconds, out _);
            mBars = mBot.MarketData.GetBars(mTimeFrame, symbol);

            OpenTimes = new CtOrgTimeSeries(mBars.OpenTimes);
            BidOpenPrices = new CtOrgDataSeries(mBars.OpenPrices);
            BidHighPrices = new CtOrgDataSeries(mBars.HighPrices);
            BidLowPrices = new CtOrgDataSeries(mBars.LowPrices);
            BidClosePrices = new CtOrgDataSeries(mBars.ClosePrices);
            BidVolumes = new CtOrgDataSeries(mBars.TickVolumes, 2);
            AskOpenPrices = new CtOrgDataSeries(mBars.OpenPrices);
            AskHighPrices = new CtOrgDataSeries(mBars.HighPrices);
            AskLowPrices = new CtOrgDataSeries(mBars.LowPrices);
            AskClosePrices = new CtOrgDataSeries(mBars.ClosePrices);
            AskVolumes = new CtOrgDataSeries(mBars.TickVolumes, 34);
        }

        public void OnTick(DateTime fromTime, DateTime prevTime)
        {
            IsNewBar = CoFu.IsNewBar(TimeFrameSeconds, fromTime, prevTime);
        }

        public void OnStop()
        {
            // No specific stop actions needed for cTrader bars 
        }
    }
}
#endif
