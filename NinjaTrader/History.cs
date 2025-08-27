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

using NinjaTrader.NinjaScript.Strategies;
using System.Collections;
using System.Collections.Generic;

namespace NinjaTrader.NinjaScript.Strategies
{
    //     Provides access to methods of the historical trades collection
    public class History : IEnumerable<HistoricalTrade>, IEnumerable
    {
        private List<HistoricalTrade> mHistoricalTrades = new();
        private Strategy mAlgoBot;

        public History(Strategy robot)
        {
            mAlgoBot = robot;
        }

        //
        // Summary:
        //     Find a historical trade by index
        //
        // Parameters:
        //   index:
        //     the index in the list
        public HistoricalTrade this[int index]
        {
            get
            {
                return mHistoricalTrades[index];
            }
        }

        //
        // Summary:
        //     Total number of historical trades
        public int Count => mHistoricalTrades.Count;

        // Required by IEnumerable<HistoricalTrade>
        public IEnumerator<HistoricalTrade> GetEnumerator()
        {
            foreach (var trade in mHistoricalTrades)
                yield return trade;
        }

        // Required by non-generic IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(HistoricalTrade position)
        {
            mHistoricalTrades.Add(position);
        }

        //
        // Summary:
        //     Find last historical trade by its label
        //
        // Parameters:
        //   label:
        //     Label to search by
        //public HistoricalTrade FindLast(string label);

        //
        // Summary:
        //     Find last historical trade by its label, symbol name
        //
        // Parameters:
        //   label:
        //     Label to search by
        //
        //   symbolName:
        //     Symbol name to search by
        //public HistoricalTrade FindLast(string label, string symbolName);

        //
        // Summary:
        //     Find last historical trade by its label, symbol name and trade type
        //
        // Parameters:
        //   label:
        //     Label to search by
        //
        //   symbolName:
        //     Symbol name to search by
        //
        //   tradeType:
        //     Trade type to search by
        //public HistoricalTrade FindLast(string label, string symbolName, TradeType tradeType);

        //
        // Summary:
        //     Find all historical trades by the label
        //
        // Parameters:
        //   label:
        //     Label to search by
        //public HistoricalTrade[] FindAll(string label);

        //
        // Summary:
        //     Find all historical trades by label, symbol name
        //
        // Parameters:
        //   label:
        //     Label to search by
        //
        //   symbolName:
        //     Symbol name to search by
        //public HistoricalTrade[] FindAll(string label, string symbolName);

        //
        // Summary:
        //     Find all historical trades by label, symbol name and trade type
        //
        // Parameters:
        //   label:
        //     Label to search by
        //
        //   symbolName:
        //     Symbol name to search by
        //
        //   tradeType:
        //     Trade type to search by
        //public HistoricalTrade[] FindAll(string label, string symbolName, TradeType tradeType);
    }
}
