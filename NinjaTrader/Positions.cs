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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class Positions : IEnumerable<Position>, IEnumerable
    {
        private List<Position> mPositions = new();
        //private Robot mAlgoBot;

        public Positions(Strategy robot)
        {
            //mAlgoBot = robot;
        }

        // Implementing IEnumerable<Position>
        public IEnumerator<Position> GetEnumerator()
        {
            return mPositions.GetEnumerator();
        }

        // Non-generic IEnumerator for compatibility
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //
        // Summary:
        //     Finds a position by index.
        //
        // Parameters:
        //   index:
        //     the index in the list
        public Position this[int index]
        {
            get
            {
                return mPositions[index];
            }
        }

        //
        // Summary:
        //     The total number of open positions.
        public int Count => mPositions.Count;

        public void Add(Position position)
        {
            mPositions.Add(position);
        }

        public void Remove(Position position)
        {
            mPositions.Remove(position);
        }

        public void RaiseOpened(PositionOpenedEventArgs args)
        {
            Opened?.Invoke(args);   // Only invoke if not null
        }

        public void RaiseClosed(PositionClosedEventArgs args)
        {
            Closed?.Invoke(args);   // Only invoke if not null
        }

        public void RaiseModified(PositionModifiedEventArgs args)
        {
            Modified?.Invoke(args); // Only invoke if not null
        }

        //
        // Summary:
        //     Occurs each time a position is closed.
        public event Action<PositionClosedEventArgs> Closed;

        //
        // Summary:
        //     Occurs each time a position is opened.
        public event Action<PositionOpenedEventArgs> Opened;

        //
        // Summary:
        //     Occurs each time a position is modified.
        public event Action<PositionModifiedEventArgs> Modified;

        //
        // Summary:
        //     Find a position by its label.
        //
        // Parameters:
        //   label:
        //     Label to search by
        //
        // Returns:
        //     Position if it exists, null otherwise
        public Position Find(string label) => mPositions.Where(p => p.Label == label).FirstOrDefault();

        //
        // Summary:
        //     Find a position by its label and symbol name.
        //
        // Parameters:
        //   label:
        //     Label to search by
        //
        //   symbolName:
        //     Symbol name to search by
        //
        // Returns:
        //     Position if it exists, null otherwise
        public Position Find(string label, string symbolName) => mPositions.Where(p => p.Label == label
            && p.SymbolName == symbolName).FirstOrDefault();

        //
        // Summary:
        //     Find a position by its label, symbol name and trade type
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
        //
        // Returns:
        //     Position if it exists, null otherwise
        public Position Find(string label, string symbolName, TradeType tradeType) => mPositions.Where(p => p.Label == label
            && p.SymbolName == symbolName
            && p.TradeType == tradeType).FirstOrDefault();

        //
        // Summary:
        //     Find all positions with this label.
        //
        // Parameters:
        //   label:
        //     Label to search by
        //
        // Returns:
        //     Array of Positions
        //Position[] FindAll(string label);

        //
        // Summary:
        //     Find all positions with this label and symbol name.
        //
        // Parameters:
        //   label:
        //     Label to search by
        //
        //   symbolName:
        //     Symbol name to search by
        //
        // Returns:
        //     Array of Positions
        public Position[] FindAll(string label, string symbolName) => mPositions.Where(p => p.Label == label
            && p.SymbolName == symbolName).ToArray();

        //
        // Summary:
        //     Finds all the positions of this label, symbol name and trade type.
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
        //
        // Returns:
        //     Array of Positions
        public Position[] FindAll(string label, string symbolName, TradeType tradeType) => mPositions.Where(p => p.Label == label
            && p.SymbolName == symbolName
            && p.TradeType == tradeType).ToArray();
    }
}
