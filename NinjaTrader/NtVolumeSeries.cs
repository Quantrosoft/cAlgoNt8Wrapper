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

using NinjaTrader.NinjaScript;
using TdsCommons;

namespace cAlgo.API
{
    public class NtVolumeSeries : ISeries<double>, IQcDataSeries
    {
        private NinjaTraderQcBars mBars;
        // With TickReplay we use our own Ringbuffer
        private Ringbuffer<long> mTickReplayData;
        // Without TickReplay we redirect directly to Ninja series
        private NinjaTrader.NinjaScript.VolumeSeries mNinjaVolumeSeries;
        private long mAskVolume;
        private long mBidVolume;

        public NtVolumeSeries(NinjaTraderQcBars bars, NinjaTrader.NinjaScript.VolumeSeries ninjaVolumeSeries)
        {
            mBars = bars;
            mNinjaVolumeSeries = ninjaVolumeSeries;
            mTickReplayData = new Ringbuffer<long>(NinjaTraderQcBars.TickReplaySize);
        }

        public void OnMarketData()
        {
            var args = mBars.Robot.MarketDataEventArgs;
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

        //     An indexer used to access the NtVolumeSeries array
        //
        // Parameters:
        //   barsAgo:
        //     An int representing from the current bar the number of historical bars to reference.
        //
        // The philosophie of cTrader is to use array indexing
        // So [0] is the very 1st element while [Count-1] is the last element
        // To get the most recent value, use Last(0)
        public double this[int index] => Last(Count - 1 - index);

        //     Indicates the number total number of values in the NtVolumeSeries array.
        public int Count => mNinjaVolumeSeries.Count;

        //     Gets the last value of this time series.
        public double LastValue => Last(0);

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
                    return (long)mNinjaVolumeSeries[index] << 34
                        | (long)mNinjaVolumeSeries[index] << 2;
            }
        }

        //     Returns the underlying NtVolumeSeries value at a specified bar index value.
        //
        // Parameters:
        //   barIndex:
        //     An int representing an absolute bar index value
        public double GetValueAt(int barIndex)
        {
            return 0;   // NinjaTraderQcBars.GetVolume(barIndex);
        }

        //     Indicates if the specified input is set at a barsAgo value relative to the current
        //     bar.
        //
        // Parameters:
        //   barsAgo:
        //     An int representing from the current bar the number of historical bars to reference.
        public bool IsValidDataPoint(int barsAgo)
        {
            return false;   // NinjaTraderQcBars.IsValidDataPoint(barsAgo);
        }

        //     Indicates if the specified input is set at a specified bar index value
        //
        // Parameters:
        //   barIndex:
        //     An int representing an absolute bar index value
        public bool IsValidDataPointAt(int barIndex)
        {
            return false;// NinjaTraderQcBars.IsValidDataPointAt(barIndex);
        }

        public void Add(double value) { }
        public void Bump() { }
        public void Swap(double value) { }
    }
}
