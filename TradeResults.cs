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
    public class TradeResult
    {
        //
        // Summary:
        //     True if the trade is successful, false if there is an error.
        public bool IsSuccessful;

        ////
        //// Summary:
        ////     The error code of an unsuccessful trade.
        //public ErrorCode? Error { get; private set; }

        ////
        //// Summary:
        ////     The resulting position of a trade request.
        public Position Position;

        ////
        //// Summary:
        ////     The resulting pending order of a trade request.
        //public PendingOrder PendingOrder { get; private set; }
    }
}
