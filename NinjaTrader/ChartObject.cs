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

namespace cAlgoNt8Wrapper
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
