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
    //     Provides data for the pending order fill event.
    public class PendingOrderFilledEventArgs
    {
        //
        // Summary:
        //     Gets the pending order that was filled.
        public PendingOrder PendingOrder { get; }

        //
        // Summary:
        //     Gets the position that was filled from the pending order.
        public Position Position { get; }

        internal PendingOrderFilledEventArgs(PendingOrder pendingOrder, Position position)
        {
            PendingOrder = pendingOrder;
            Position = position;
        }
    }
}