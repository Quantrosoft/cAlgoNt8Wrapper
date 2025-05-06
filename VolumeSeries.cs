//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using SevenZip.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TdsCommons;

namespace cAlgo.API
{
    public class VolumeSeries : ISeries<double>
    {
        private Bars mBars;
        // With TickReplay we use our own Ringbuffer
        private Ringbuffer<long> mTickReplayData;
        // Without TickReplay we redirect directly to Ninja series
        private NinjaTrader.NinjaScript.VolumeSeries[] mNinjaVolumeSeriesArray;
        private long mAskVolume;
        private long mBidVolume;

        public VolumeSeries(Bars bars, NinjaTrader.NinjaScript.VolumeSeries[] ninjaVolumeSeriesArray)
        {
            mBars = bars;
            mNinjaVolumeSeriesArray = ninjaVolumeSeriesArray;
            mTickReplayData = new Ringbuffer<long>(Bars.TickReplaySize);
        }

        public void OnMarketData(MarketDataEventArgs args)
        {
            if (mBars.IsNewBar)
            {
                mTickReplayData.Add(0);
                mAskVolume = mBidVolume = 0; // reset volumes at the start of a new bar
            }

            // Accumulate volumes
            if (args.Ask != args.Bid)   // add no volume if prices are equal
                if (args.Price >= args.Ask)
                    mAskVolume += args.Volume;
                else if (args.Price <= args.Bid)
                    mBidVolume += args.Volume;

            mTickReplayData.Swap(mAskVolume << 34 | mBidVolume << 2);
        }

        //
        // Summary:
        //     An indexer used to access the VolumeSeries array
        //
        // Parameters:
        //   barsAgo:
        //     An int representing from the current bar the number of historical bars to reference.
        //
        // The philosophie of cTrader is to use array indexing
        // So [0] is the very 1st element while [Count-1] is the last element
        // To get the most recent value, use Last(0)
        public double this[int index] => Last(Count - 1 - index);

        //
        // Summary:
        //     Indicates the number total number of values in the VolumeSeries array.
        public int Count => mNinjaVolumeSeriesArray[mBars.BidBarsIndex].Count;

        //
        // Summary:
        //     Gets the last value of this time series.
        public double LastValue => Last(0);

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
                // Do not return end volume of current bar 0; would be future volume
                if (0 == index)
                    return 0;
                else
                    return (double)((long)mNinjaVolumeSeriesArray[mBars.AskBarsIndex][index] << 34
                        | (long)mNinjaVolumeSeriesArray[mBars.BidBarsIndex][index] << 2);
            }
        }

        //
        // Summary:
        //     Returns the underlying VolumeSeries value at a specified bar index value.
        //
        // Parameters:
        //   barIndex:
        //     An int representing an absolute bar index value
        public double GetValueAt(int barIndex)
        {
            return 0;   // Bars.GetVolume(barIndex);
        }

        //
        // Summary:
        //     Indicates if the specified input is set at a barsAgo value relative to the current
        //     bar.
        //
        // Parameters:
        //   barsAgo:
        //     An int representing from the current bar the number of historical bars to reference.
        public bool IsValidDataPoint(int barsAgo)
        {
            return false;   // Bars.IsValidDataPoint(barsAgo);
        }

        //
        // Summary:
        //     Indicates if the specified input is set at a specified bar index value
        //
        // Parameters:
        //   barIndex:
        //     An int representing an absolute bar index value
        public bool IsValidDataPointAt(int barIndex)
        {
            return false;// Bars.IsValidDataPointAt(barIndex);
        }
    }
}
