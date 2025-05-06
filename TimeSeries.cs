//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using NinjaTrader.Data;
using System;
using TdsCommons;

namespace cAlgo.API
{
    //     A series of values that represent time like Bars.OpenPricesTime
    public class TimeSeries
    {
        private Bars mBars;
        // With TickReplay we use our own Ringbuffer
        private Ringbuffer<DateTime> mTickReplayData;
        // Without TickReplay we redirect directly to Ninja series
        private NinjaTrader.NinjaScript.TimeSeries[] mNinjaTimeSeries;

        public TimeSeries(Bars bars, NinjaTrader.NinjaScript.TimeSeries[] timeSeriesArray)
        {
            mBars = bars;
            mNinjaTimeSeries = timeSeriesArray;
            mTickReplayData = new Ringbuffer<DateTime>(Bars.TickReplaySize);
        }

        public void OnMarketData(MarketDataEventArgs args)
        {
            if (mBars.IsNewBar || 0 == mTickReplayData.Count)
                mTickReplayData.Add(args.Time);
        }

        //
        // Summary:
        //     Returns the DateTime value at the specified index.
        //
        // Parameters:
        //   index:
        //     The index of the returned value within the series.
        //
        // The philosophie of cTrader is to use array indexing
        // So [0] is the very 1st element while [Count-1] is the last element
        // To get the most recent value, use Last(0)
        public DateTime this[int index] => Last(Count - 1 - index);

        //
        // Summary:
        //     Gets the last value of this time series.
        public DateTime LastValue => Last(0);

        //
        // Summary:
        //     Gets the number of elements contained in the series.
        public int Count => mNinjaTimeSeries[0].Count;

        //
        // Summary:
        //     Access a value in the data series certain number of bars ago.
        //
        // Parameters:
        //   index:
        //     Number of bars ago
        public DateTime Last(int index)
        {
            if (mBars.Robot.IsTickReplay)
            {
                var nativeTime = mTickReplayData[index].ToNativeSec();
                return (nativeTime - (nativeTime % mBars.BarsSeconds)).FromNativeSec();
            }
            else
                return mNinjaTimeSeries[mBars.BidBarsIndex][index];
        }
    }
}
