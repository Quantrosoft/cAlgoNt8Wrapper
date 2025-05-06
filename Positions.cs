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
    public class Positions : IEnumerable<Position>
    {
        public Positions(Robot robot)
        {
            mAlgoBot = robot;
        }

        private List<Position> mPositions = new List<Position>();
        private Robot mAlgoBot;

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
            // Only the class that declares the event can do this
            Opened?.Invoke(args);
        }

        public void RaiseClosed(PositionClosedEventArgs args)
        {
            // Only the class that declares the event can do this
            Closed?.Invoke(args);
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
        //public event Action<PositionModifiedEventArgs> Modified;

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
        //Position Find(string label);

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
        //Position Find(string label, string symbolName);

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
        //Position Find(string label, string symbolName, TradeType tradeType);

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
        //Position[] FindAll(string label, string symbolName);

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
        //Position[] FindAll(string label, string symbolName, TradeType tradeType);
    }
}
