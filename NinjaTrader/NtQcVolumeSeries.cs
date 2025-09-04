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
using NinjaTrader.NinjaScript.DrawingTools;
using System;
using System.Collections.Generic;
using TdsCommons;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class NtQcVolumeSeries : ISeries<double>, IQcVolumeSeries
    {
        private NtQcBars mBars;
        private Symbol mSymbol;
        private bool mIsBidNotAsk;

        private Ringbuffer<long> mTickReplayVolumes;
        private Ringbuffer<Dictionary<int, long>> mLevelsBuf;

        // NEW: ringbuffers for min/max keys per bar
        private Ringbuffer<int> mMinKeyBuf;
        private Ringbuffer<int> mMaxKeyBuf;

        private Dictionary<int, long> mPriceLevels;
        private VolumeSeries mPlatformVolumeSeries;
        private long mVolume;

        // NEW: current-bar min/max trackers
        private int mMinKey;
        private int mMaxKey;

        private static readonly Dictionary<int, long> s_emptyLevels = new();

        public double DigitsSize { get; }

        public NtQcVolumeSeries(
            NtQcBars bars,
            Symbol symbol,
            BidAsk bidAsk,
            NinjaTrader.NinjaScript.VolumeSeries ninjaVolumeSeries)
        {
            mBars = bars;
            mSymbol = symbol;
            mIsBidNotAsk = bidAsk == BidAsk.Bid;
            mPlatformVolumeSeries = ninjaVolumeSeries;

            mTickReplayVolumes = new Ringbuffer<long>(NtQcBars.TickReplaySize);

            DigitsSize = 1.0 / Math.Pow(10, mSymbol.Digits);

            if (mBars.PriceLevelSize > 0)
            {
                mLevelsBuf = new Ringbuffer<Dictionary<int, long>>(NtQcBars.TickReplaySize);
                mMinKeyBuf = new Ringbuffer<int>(NtQcBars.TickReplaySize);
                mMaxKeyBuf = new Ringbuffer<int>(NtQcBars.TickReplaySize);
                mPriceLevels = new Dictionary<int, long>();
                mLevelsBuf.Add(mPriceLevels);

                mMinKey = int.MaxValue;
                mMaxKey = int.MinValue;
                mMinKeyBuf.Add(mMinKey);
                mMaxKeyBuf.Add(mMaxKey);
            }
        }

        public void OnMarketData()
        {
            var args = mBars.Robot.MarketDataEventArgs;

            if (mBars.IsNewInternalBar)
            {
                if (mBars.PriceLevelSize > 0)
                {
                    // Push previous bar’s snapshot into the ring buffers
                    mLevelsBuf.Add(mPriceLevels);
                    mMinKeyBuf.Add(mMinKey);
                    mMaxKeyBuf.Add(mMaxKey);

                    // Start new bar with fresh dictionary and reset min/max
                    mPriceLevels = new Dictionary<int, long>();
                    mMinKey = int.MaxValue;
                    mMaxKey = int.MinValue;
                }

                // Open a new slot in the volume buffer
                mTickReplayVolumes.Add(0);
                mVolume = 0;
            }

            // Accumulate volumes (skip crossed/locked check where ask==bid)
            if (args.Ask != args.Bid)
            {
                bool isAsk = !mIsBidNotAsk && args.Price >= args.Ask;
                bool isBid = mIsBidNotAsk && args.Price <= args.Bid;

                if (isAsk || isBid)
                {
                    mVolume += args.Volume;

                    if (mBars.PriceLevelSize > 0)
                    {
                        var priceLevel = CoFu.iPrice(args.Price, DigitsSize)
                                         / CoFu.iPrice(mBars.PriceLevelSize, DigitsSize);

                        if (mPriceLevels.TryGetValue(priceLevel, out var v))
                        {
                            mPriceLevels[priceLevel] = v + args.Volume;
                        }
                        else
                        {
                            mPriceLevels[priceLevel] = args.Volume;

                            // NEW: update min/max on FIRST insertion of this key
                            if (priceLevel < mMinKey) mMinKey = priceLevel;
                            if (priceLevel > mMaxKey) mMaxKey = priceLevel;
                        }
                    }
                }
            }

            // Keep head of buffers reflecting the current bar
            mTickReplayVolumes.Swap(mVolume);
            if (mBars.PriceLevelSize > 0)
            {
                mLevelsBuf.Swap(mPriceLevels);
                mMinKeyBuf.Swap(mMinKey);
                mMaxKeyBuf.Swap(mMaxKey);
            }
        }

        // ISeries<double> plumbing (unchanged semantics)
        public double this[int index] => Last(Count - 1 - index);
        public int Count => mPlatformVolumeSeries.Count;
        public double LastValue => Last(0);

        public double Last(int index)
        {
            if (mBars.Robot.IsTickReplay)
                return mTickReplayVolumes[index];
            else
            {
                // Do not return end volume of current bar 0; would be future volume
                if (index == 0) return 0;
                // TODO: replace placeholder with your true packed/historical logic
                return (long)mPlatformVolumeSeries[index] << 34
                     | (long)mPlatformVolumeSeries[index] << 2;
            }
        }

        public double GetValueAt(int barIndex) => 0;
        public bool IsValidDataPoint(int barsAgo) => false;
        public bool IsValidDataPointAt(int barIndex) => false;
        public void Add(double value) { }
        public void Bump() { }
        public void Swap(double value) { }

        // CHANGED: now returns dictionary + minKey + maxKey
        // minKey > maxKey indicates "empty"
        public (Dictionary<int, long> Levels, int MinKey, int MaxKey) GetPriceLevelVolumes(int index)
        {
            if (mBars.PriceLevelSize > 0)
            {

                if (mBars.Robot.IsTickReplay)
                    return (mLevelsBuf[index], mMinKeyBuf[index], mMaxKeyBuf[index]);

                // If not in TickReplay, typically only current bar has data.
                if (index == 0)
                    return (mLevelsBuf[index], mMinKeyBuf[index], mMaxKeyBuf[index]);
            }

            // Return an explicit "empty" snapshot for history
            return (s_emptyLevels, int.MaxValue, int.MinValue);
        }
    }
}
