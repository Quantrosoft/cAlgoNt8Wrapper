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
using System;
using System.Collections;
using System.Collections.Generic;

namespace cAlgoNt8Wrapper
{
    public class PendingOrders : IEnumerable<PendingOrder>, IEnumerable
    {
        private List<PendingOrder> mPendingOrders = new();
        //private Robot mAlgoBot;

        public PendingOrders(Strategy robot)
        {
            //mAlgoBot = robot;
        }

        // Implementing IEnumerable<Position>
        public IEnumerator<PendingOrder> GetEnumerator()
        {
            return mPendingOrders.GetEnumerator();
        }

        // Non-generic IEnumerator for compatibility
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>Find a pending order by index</summary>
        /// <param name="index">The position of the order in the collection</param>
        /// <example>
        /// <code>
        /// if(PendingOrders.Count > 0)
        ///     Print(PendingOrders[0].Id);
        /// </code>
        /// </example>
        public PendingOrder this[int index]
        {
            get
            {
                return mPendingOrders[index];
            }
        }

        /// <summary>Total number of pending orders</summary>
        /// <example>
        /// <code>
        /// var totalOrders = PendingOrders.Count;
        /// </code>
        /// </example>
        public int Count => mPendingOrders.Count;

        public void Add(PendingOrder pendingOrder)
        {
            mPendingOrders.Add(pendingOrder);
        }

        public void Remove(PendingOrder pendingOrder)
        {
            mPendingOrders.Remove(pendingOrder);
        }

        public void RaiseCreated(PendingOrderCreatedEventArgs args)
        {
            Created?.Invoke(args);
        }

        public void RaiseModified(PendingOrderModifiedEventArgs args)
        {
            Modified?.Invoke(args);
        }

        public void RaiseCancelled(PendingOrderCancelledEventArgs args)
        {
            Cancelled?.Invoke(args);
        }

        public void RaiseFilled(PendingOrderFilledEventArgs args)
        {
            Filled?.Invoke(args);
        }

        /// <summary>Occurs when pending order is created</summary>
        /// <example>
        /// <code>
        /// protected override void OnStart()
        /// {
        ///     PendingOrders.Created += PendingOrdersOnCreated;
        ///     PlaceStopOrder(TradeType.Buy, SymbolName, 10000, SymbolName.Ask + 10 * SymbolName.PipSize);
        /// }
        /// private void PendingOrdersOnCreated(PendingOrderCreatedEventArgs args)
        /// {
        ///     Print("Pending order with id {0} was created", args.PendingOrder.Id);
        /// }
        /// </code>
        /// </example>
        public event Action<PendingOrderCreatedEventArgs> Created;

        /// <summary>Occurs when pending order is modified</summary>
        /// <example>
        /// <code>
        /// protected override void OnStart()
        /// {
        ///     PendingOrders.Modified += PendingOrdersOnModified;
        ///     var result = PlaceStopOrder(TradeType.Buy, SymbolName, 10000, SymbolName.Ask + 10 * SymbolName.PipSize);
        ///     ModifyPendingOrder(result.PendingOrder, SymbolName.Ask + 20 * SymbolName.PipSize ,null, null, null);
        /// }
        /// private void PendingOrdersOnModified(PendingOrderModifiedEventArgs args)
        /// {
        ///     Print("Pending order with id {0} was modifed", args.PendingOrder.Id);
        /// }
        /// </code>
        /// </example>
        public event Action<PendingOrderModifiedEventArgs> Modified;

        /// <summary>Occurs when pending order is cancelled</summary>
        /// <example>
        /// <code>
        /// protected override void OnStart()
        /// {
        ///     PendingOrders.Cancelled += PendingOrdersOnCancelled;
        ///     var result = PlaceStopOrder(TradeType.Buy, SymbolName, 10000, SymbolName.Ask + 10 * SymbolName.PipSize);
        ///     CancelPendingOrder(result.PendingOrder);
        /// }
        /// private void PendingOrdersOnCancelled(PendingOrderCancelledEventArgs args)
        /// {
        ///    Print("Pending order with id {0} was cancelled. Reason: {1}", args.PendingOrder.Id, args.Reason);
        /// }
        /// </code>
        /// </example>
        public event Action<PendingOrderCancelledEventArgs> Cancelled;

        /// <summary>Occurs when pending order is filled</summary>
        /// <example>
        /// <code>
        /// protected override void OnStart()
        /// {
        ///     PendingOrders.Filled += PendingOrdersOnFilled;
        ///     PlaceStopOrder(TradeType.Buy, SymbolName, 10000, SymbolName.Ask);
        /// }
        /// private void PendingOrdersOnFilled(PendingOrderFilledEventArgs args)
        /// {
        ///     Print("Pending order with id {0} was filled, position id  is {1}", args.PendingOrder.Id, args.Position.Id);
        /// }
        /// </code>
        /// </example>
        public event Action<PendingOrderFilledEventArgs> Filled;
    }
}
