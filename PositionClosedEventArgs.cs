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
    //     Provides data for the position closing event.
    public class PositionClosedEventArgs
    {
        //
        // Summary:
        //     Gets the position being closed.
        public Position Position { get; }

        //
        // Summary:
        //     Gets the reason of the position being closed.
        public PositionCloseReason Reason { get; }

        //
        // Summary:
        //     Provides data for the closing positions event.
        //
        // Parameters:
        //   position:
        //
        //   reason:
        protected internal PositionClosedEventArgs(Position position, PositionCloseReason reason)
        {
            Reason = reason;
            Position = position;
        }
    }
}
