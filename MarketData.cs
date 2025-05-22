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

using System.Collections.Generic;

namespace cAlgo.API
{
    //     Provide access to charts data (bars and ticks) and to the Depth of Market data.
    public class MarketData
    {
        private Robot mRobot;
        public Dictionary<(int, string), Bars> BarsDictionary = new Dictionary<(int, string), Bars>();

        public MarketData(Robot robot)
        {
            mRobot = robot;
        }

        //
        // Summary:
        //     Gets the bars for the specific timeframe.
        //
        // Parameters:
        //   timeFrame:
        //     The bars time frame
        //public Bars GetBars(TimeFrame timeFrame);


        //
        // Summary:
        //     Gets the chart bars for the specific timeframe for the specific symbol.
        //
        // Parameters:
        //   timeFrame:
        //     The bars time frame
        //
        //   symbolName:
        //     The bars symbol name
        public Bars GetBars(TimeFrame timeFrame, string symbolName)
        {
            var barsSeconds = timeFrame.GetPeriodSeconds();
            if (!BarsDictionary.ContainsKey((barsSeconds, symbolName)))
            {
                var bars = new Bars(mRobot, timeFrame, symbolName);
                BarsDictionary.Add((barsSeconds, symbolName), bars);
                return bars;
            }
            else
                return BarsDictionary[(barsSeconds, symbolName)];
        }

        //
        // Summary:
        //     The asynchronous method to get the bars for the specific timeframe for the specific
        //     symbol.
        //
        // Parameters:
        //   timeFrame:
        //     The bars time frame
        //
        //   callback:
        //     The callback that will be called after getting the bars
        //public void GetBarsAsync(TimeFrame timeFrame, Action<Bars> callback);

        //
        // Summary:
        //     The asynchronous method to get the specific bars for the specific timeframe for
        //     the specific symbol.
        //
        // Parameters:
        //   timeFrame:
        //     The bars time frame
        //
        //   symbolName:
        //     The bars symbol name
        //
        //   callback:
        //     The callback that will be called after getting the bars
        //public void GetBarsAsync(TimeFrame timeFrame, string symbolName, Action<Bars> callback);

        //
        // Summary:
        //     Gets the Tick data.
        //public Ticks GetTicks();

        //
        // Summary:
        //     Gets the Tick data for the specific symbol.
        //
        // Parameters:
        //   symbolName:
        //     The ticks symbol name
        //public Ticks GetTicks(string symbolName);

        //
        // Summary:
        //     The asynchronous method to get tick data.
        //
        // Parameters:
        //   callback:
        //     The callback that will be called after getting the ticks
        //public void GetTicksAsync(Action<Ticks> callback);

        //
        // Summary:
        //     The asynchronous method to get tick data for the specific symbol.
        //
        // Parameters:
        //   symbolName:
        //     The ticks symbol name
        //
        //   callback:
        //     The callback that will be called after getting the ticks
        //public void GetTicksAsync(string symbolName, Action<Ticks> callback);

        //
        // Summary:
        //     Gets the Depth of Market for the specific symbol.
        //
        // Parameters:
        //   symbolName:
        //     The symbol name e.g. "EURUSD"
        //
        // Returns:
        //     Depth of Market
        //MarketDepth GetMarketDepth(string symbolName);
    }
}
