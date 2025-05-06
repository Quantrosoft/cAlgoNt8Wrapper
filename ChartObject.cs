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
    //     Represents the chart object.
    public class ChartObject
    {
        //
        // Summary:
        //     Gets the chart object name - the unique identifier for the object in the current
        //     chart area.
        public string Name { get; }

        //
        // Summary:
        //     Gets or sets the comment for the chart object.
        //
        // ProfitModeValue:
        //     The comment.
        public string Comment { get; set; }

        //
        // Summary:
        //     Gets the chart object type.
        //public ChartObjectType ObjectType { get; }

        //
        // Summary:
        //     Defines whether the instance is interactive. The non-interactive chart objects
        //     cannot be selected, have no hover effect and cannot be searched. Available only
        //     for the current cBot or Indicator and will be removed when the cBot/Indicator
        //     stops.
        public bool IsInteractive { get; set; }

        //
        // Summary:
        //     Gets or sets if the object is loacked or not, user can't move locked objects
        //     unless unlocked
        public bool IsLocked { get; set; }

        //
        // Summary:
        //     Gets or sets if object is hidden and not visible to user or not
        public bool IsHidden { get; set; }

        //
        // Summary:
        //     Gets or sets the location of a chart object on the Z-axis in respect to the other
        //     chart objects.
        public int ZIndex { get; set; }

        //
        // Summary:
        //     Defines if the chart object still exists on the chart.
        public bool IsAlive { get; }
    }
}
