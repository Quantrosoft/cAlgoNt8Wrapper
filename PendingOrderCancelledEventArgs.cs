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
    //
    // Summary:
    //     Provides data for the pending order cancellation event.
    public class PendingOrderCancelledEventArgs
    {
        //
        // Summary:
        //     Gets the pending order that was cancelled.
        public PendingOrder PendingOrder { get; }

        //
        // Summary:
        //     Gets the reason for the pending order cancellation.
        public PendingOrderCancellationReason Reason { get; }

        internal PendingOrderCancelledEventArgs(PendingOrder pendingOrder, PendingOrderCancellationReason reason)
        {
            PendingOrder = pendingOrder;
            Reason = reason;
        }
    }
}