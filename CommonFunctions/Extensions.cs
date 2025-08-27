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

using System;
using System.Collections.Generic;
using System.Linq;

namespace TdsCommons
{
    public static class Extensions
    {
        /// <summary>
        /// Round to full and clean seconds
        /// </summary>
        /*public static DateTime Time(this DateTime time)
        {
           return new DateTime((time.ToNativeSec() + CoFu.DateTime2EpocDiff) * (long)HECTONANOSEC_PER_SEC);
        }*/

        /// <summary>
        /// Get seconds since 1.1.1970 as in MQL
        /// </summary>
        /// <returns>Seconds since 1.1.1970 as in MT4</returns>
        public static long ToNativeSec(this DateTime time)
        {
            return time.Ticks / TdsDefs.HECTONANOSEC_PER_SEC - CoFu.DateTime2EpocDiff;
        }

        /// <summary>
        /// Get seconds since 1.1.1970 as in MQL
        /// </summary>
        /// <returns>Seconds since 1.1.1970 as in MT4</returns>
        public static long ToNativeMs(this DateTime time)
        {
            return (time.Ticks - CoFu.TimeInvalid.Ticks) / (TdsDefs.HECTONANOSEC_PER_SEC / 1000);
        }

        /// <summary>
        /// Convert MQL time to .Net DateTime
        /// </summary>
        public static DateTime FromNativeSec(this long time)
        {
            var ret = CoFu.TimeInvalid + TimeSpan.FromSeconds(time);
            return ret;
        }

        /// <summary>
        /// Convert MQL time in ms to .Net DateTime
        /// </summary>
        public static DateTime FromNativeMs(this long time)
        {
            var ret = CoFu.TimeInvalid + TimeSpan.FromMilliseconds(time);
            return ret;
        }

        public static DateTime ManageTimeZones(this DateTime dt, string timezoneId, bool init)
        {
            // nothing to do here for cTrader, only for MQL
            return dt;
        }

        /// <summary>
        /// Calculates the Standard-Deviation
        /// </summary>
        public static double StdDev(this IEnumerable<double> values, bool isSample = true)
        {
            int n = values.Count();
            if (n <= 1)
                return 0;

            // Calc the Average
            double mean = values.Average();

            // Perform the Sum of (value-avg)^2
            double sum = values.Sum(x => (x - mean) * (x - mean));

            // Put it all together
            return isSample
               ? Math.Sqrt(sum / (n - 1))
               : Math.Sqrt(sum / n);
        }

        /// <summary>
        /// Calculates the Downside-Deviation
        /// </summary>
        public static double DownDev(this IEnumerable<double> values, bool isSample = true)
        {
            int n = values.Count();
            if (n <= 1)
                return 0;

            // Calc the Average
            double mean = values.Average();

            // Perform the Sum of (value-avg)^2
            double sum = values.Where(x => x < 0).Sum(x => (x - mean) * (x - mean));

            // Put it all together
            return isSample
               ? Math.Sqrt(sum / (n - 1))
               : Math.Sqrt(sum / n);
        }
    }
}
// end of file
