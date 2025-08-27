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

using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using System;
using System.Collections.Generic;
using TdsCommons;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class NtQcVolumeSeries : ISeries<double>, IQcVolumeSeries
    {
        private NtQcBars mBars;
        private Symbol mSymbol;
        private bool mIsAsk;
        private bool mIsBid;
        private double mPriceLevelSize;
        private Ringbuffer<long> mTickReplayData;
        private double mDigitsSize;
        private SortedDictionary<int, long> mPriceLevels = new();
        private long mVolume;

        public VolumeSeries PlatformVolumeSeries { get; }

        public NtQcVolumeSeries(NtQcBars bars,
            Symbol symbol,
            BidAsk bidAsk,
            NinjaTrader.NinjaScript.VolumeSeries ninjaVolumeSeries,
            double cashPriceLevelSize)
        {
            mBars = bars;
            mSymbol = symbol;
            mIsAsk = bidAsk == BidAsk.Ask;
            mIsBid = bidAsk == BidAsk.Bid;
            PlatformVolumeSeries = ninjaVolumeSeries;
            mTickReplayData = new Ringbuffer<long>(NtQcBars.TickReplaySize);
            mDigitsSize = 1.0 / Math.Pow(10, mSymbol.Digits);
            mPriceLevelSize = cashPriceLevelSize;
        }

        public void OnMarketData()
        {
            var args = mBars.Robot.MarketDataEventArgs;
            if (mBars.IsNewBar)
            {
                mTickReplayData.Add(0);
                mVolume = 0; // reset volumes at the start of a new bar
                mPriceLevels.Clear();
            }

            // Accumulate volumes
            if (args.Ask != args.Bid)   // do not add volume if prices are equal
            {
                var priceLevel = CoFu.iPrice(args.Price, mDigitsSize);

                if (mIsAsk && args.Price >= args.Ask)
                {
                    mVolume += args.Volume;
                    mPriceLevels[priceLevel] = (mPriceLevels.TryGetValue(priceLevel, out var v) ? v : 0) 
                        + args.Volume;
                }

                if (mIsBid && args.Price <= args.Bid)
                {
                    mVolume += args.Volume;
                    mPriceLevels[priceLevel] = (mPriceLevels.TryGetValue(priceLevel, out var v) ? v : 0)
                        + args.Volume;
                }
            }

            mTickReplayData.Swap(mVolume);
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
        public int Count => PlatformVolumeSeries.Count;

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
                    return (long)PlatformVolumeSeries[index] << 34
                        | (long)PlatformVolumeSeries[index] << 2;
            }
        }

        //     Returns the underlying NtVolumeSeries value at a specified bar index value.
        //
        // Parameters:
        //   barIndex:
        //     An int representing an absolute bar index value
        public double GetValueAt(int barIndex)
        {
            return 0;   // NtQcBars.GetVolume(barIndex);
        }

        //     Indicates if the specified input is set at a barsAgo value relative to the current
        //     bar.
        //
        // Parameters:
        //   barsAgo:
        //     An int representing from the current bar the number of historical bars to reference.
        public bool IsValidDataPoint(int barsAgo)
        {
            return false;   // NtQcBars.IsValidDataPoint(barsAgo);
        }

        //     Indicates if the specified input is set at a specified bar index value
        //
        // Parameters:
        //   barIndex:
        //     An int representing an absolute bar index value
        public bool IsValidDataPointAt(int barIndex)
        {
            return false;// NtQcBars.IsValidDataPointAt(barIndex);
        }

        public void Add(double value) { }
        public void Bump() { }
        public void Swap(double value) { }
    }
}
