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
    //     Provides data for the position opening event.
    public class PositionOpenedEventArgs
    {
        //
        // Summary:
        //     Gets or sets the position being opened.
        public Position Position { get; private set; }

        internal PositionOpenedEventArgs(Position position)
        {
            Position = position;
        }
    }
}
