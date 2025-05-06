//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAlgo.API
{
    //     Provides access to methods of the historical trades collection
    public class History : IEnumerable<HistoricalTrade>, IEnumerable
    {
        private List<HistoricalTrade> mHistoricalTrades = new List<HistoricalTrade>();
        private Robot mAlgoBot;

        public History(Robot robot)
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
