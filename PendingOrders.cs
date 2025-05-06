//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAlgo.API
{
    public interface PendingOrders : IEnumerable<PendingOrder>//, IEnumerable
    {
        /// <summary>Find a pending order by index</summary>
        /// <param name="index">The position of the order in the collection</param>
        /// <example>
        /// <code>
        /// if(PendingOrders.Count > 0)
        ///     Print(PendingOrders[0].Id);
        /// </code>
        /// </example>
        PendingOrder this[int index] { get; }

        /// <summary>Total number of pending orders</summary>
        /// <example>
        /// <code>
        /// var totalOrders = PendingOrders.Count;
        /// </code>
        /// </example>
        int Count { get; }

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
        event Action<PendingOrderCreatedEventArgs> Created;

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
        event Action<PendingOrderModifiedEventArgs> Modified;

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
        event Action<PendingOrderCancelledEventArgs> Cancelled;

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
        event Action<PendingOrderFilledEventArgs> Filled;
    }
}
