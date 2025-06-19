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
using System;

namespace TdsCommons
{
    public class CtOrgTimeSeries : IQcTimeSeries
    {
        private TimeSeries mTimeSeries;

        public CtOrgTimeSeries(TimeSeries timeSeries)
        {
            mTimeSeries = timeSeries;
        }

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

        //     Gets the last value of this time series.
        public DateTime LastValue => Last(0);

        //     Access a value in the data series certain number of bars ago.
        //
        // Parameters:
        //   index:
        //     Number of bars ago
        public DateTime Last(int index)
        {
            return mTimeSeries.Last(index);
        }

        //     Gets the total number of elements contained in the NtQcTimeSeries.
        public int Count => mTimeSeries.Count;

        public void OnMarketData() { }
        public void Add(DateTime value) { }
        public void Bump() { }
        public void Swap(DateTime value) { }
    }
}
#endif