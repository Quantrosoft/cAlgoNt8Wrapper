//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using NinjaTrader.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NinjaTrader.NinjaScript.Strategies;
using RobotLib;
using NinjaTrader.NinjaScript;

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
