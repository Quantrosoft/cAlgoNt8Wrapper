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

using TdsCommons;

namespace cAlgo.API
{
    public class CtQcDataSeries : IQcDataSeries
    {
        private Ringbuffer<double> mQcData;

        public CtQcDataSeries()
        {
            mQcData = new Ringbuffer<double>(CtQcBars.QcBarsSize);
        }

        public double this[int index] => Last(Count - 1 - index);

        //     Gets the value in the dataseries at the specified position.
        //
        // The philosophie of cTrader is to use array indexing
        // So [0] is the very 1st element while [Count-1] is the last element
        // To get the most recent value, use Last(0)
        //public double this[int index] => mNinjaDataSeries[Count - 1 - index];

        //
        // Summary:
        //     Gets the last value of this NtQcDataSeries.
        //
        // Remarks:
        //     The last value may represent one of the values of the last bar of the market
        //     series, e.g. Open, High, Low and Close. Therefore, take into consideration that
        //     on each tick, except the Open price, the rest of the values will most probably
        //     change.
        public double LastValue => Last(0);

        //     Gets the total number of elements contained in the NtQcDataSeries.
        public int Count => mQcData.AddCount;

        public void OnMarketData() { }

        //     Access a value in the dataseries certain bars ago
        //
        // Parameters:
        //   index:
        //     Number of bars ago
        public double Last(int index)
        {
            return (long)mQcData[index];
        }

        public void Add(double value)
        {
            mQcData.Add(value);
        }

        public void Bump()
        {
            mQcData.Bump();
        }

        public void Swap(double value)
        {
            mQcData.Swap(value);
        }
    }
}
