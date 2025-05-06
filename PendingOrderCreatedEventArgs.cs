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
    //     Provides data for the pending order creation events.
    public class PendingOrderCreatedEventArgs
    {
        //
        // Summary:
        //     Gets the pending order that was created.
        public PendingOrder PendingOrder { get; }

        internal PendingOrderCreatedEventArgs(PendingOrder pendingOrder)
        {
            PendingOrder = pendingOrder;
        }
    }
}