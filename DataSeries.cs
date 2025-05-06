//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using NinjaTrader.CQG.ProtoBuf;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TdsCommons;

namespace cAlgo.API
{
    public class DataSeries
    {
        //
        // Summary:
        //     Represents a read only list of values, typically used to represent market price
        //     series. The values are accessed with an array-like [] operator.
        private Bars mBars;
        private Symbol mSymbol;
        // With TickReplay we use our own Ringbuffer
        private Ringbuffer<double> mTickReplayData;
        // Without TickReplay we redirect directly to Ninja series
        private NinjaTrader.NinjaScript.PriceSeries[] mNinjaDataSeriesArray;

        public DataSeries(Bars bars, Symbol symbol,
            NinjaTrader.NinjaScript.PriceSeries[] ninjaDataSeriesArray)
        {
            mBars = bars;
            mSymbol = symbol;
            mNinjaDataSeriesArray = ninjaDataSeriesArray;
            mTickReplayData = new Ringbuffer<double>(Bars.TickReplaySize);
        }

        public void OnMarketData(MarketDataEventArgs args)
        {
            if (mBars.IsNewBar || 0 == mTickReplayData.Count)
                mTickReplayData.Add(args.Bid);

            switch (mNinjaDataSeriesArray[0].PriceType) // all series have the same PriceType
            {
                case PriceType.High:
                mTickReplayData.Swap(Math.Max(mTickReplayData[0], args.Bid));
                break;
                case PriceType.Low:
                mTickReplayData.Swap(Math.Min(mTickReplayData[0], args.Bid));
                break;
                case PriceType.Close:
                mTickReplayData.Swap(args.Bid);
                break;
            }
        }

        public double this[int index] => Last(Count - 1 - index);

        //
        // Summary:
        //     Gets the value in the dataseries at the specified position.
        //
        // The philosophie of cTrader is to use array indexing
        // So [0] is the very 1st element while [Count-1] is the last element
        // To get the most recent value, use Last(0)
        //public double this[int index] => mNinjaDataSeries[Count - 1 - index];

        //
        // Summary:
        //     Gets the last value of this DataSeries.
        //
        // Remarks:
        //     The last value may represent one of the values of the last bar of the market
        //     series, e.g. Open, High, Low and Close. Therefore, take into consideration that
        //     on each tick, except the Open price, the rest of the values will most probably
        //     change.
        public double LastValue => Last(0);

        //
        // Summary:
        //     Gets the total number of elements contained in the DataSeries.
        public int Count => mNinjaDataSeriesArray[mBars.BidBarsIndex].Count;

        //
        // Summary:
        //     Access a value in the dataseries certain bars ago
        //
        // Parameters:
        //   index:
        //     Number of bars ago
        public double Last(int index)
        {
            if (mBars.Robot.IsTickReplay)
                return mTickReplayData[index];
            else
            {
                // Do not return end volume of current bar 0; would be future data
                if (0 == index)
                    return mSymbol.Bid;
                else
                    return mNinjaDataSeriesArray[mBars.BidBarsIndex][index];
            }
        }
    }
}
