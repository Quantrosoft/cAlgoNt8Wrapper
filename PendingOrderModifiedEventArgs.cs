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
    //     Provides data for the pending order modification event.
    public class PendingOrderModifiedEventArgs
    {
        //
        // Summary:
        //     Gets the pending order that was modified.
        public PendingOrder PendingOrder { get; }

        internal PendingOrderModifiedEventArgs(PendingOrder pendingOrder)
        {
            PendingOrder = pendingOrder;
        }
    }
}