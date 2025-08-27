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

using static TdsDefs;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class TimeFrame
    {
        private int mPeriodSeconds;

        public TimeFrame(int periodSeconds)
        {
            mPeriodSeconds = periodSeconds;
        }

        public override int GetHashCode()
        {
            return mPeriodSeconds / SEC_PER_MINUTE;
        }

        public int GetPeriodSeconds()
        {
            return mPeriodSeconds;
        }

        public static TimeFrame Minute => new TimeFrame(SEC_PER_MINUTE);
        public static TimeFrame Minute2 => new TimeFrame(2 * SEC_PER_MINUTE);
        public static TimeFrame Minute3 => new TimeFrame(3 * SEC_PER_MINUTE);
        public static TimeFrame Minute4 => new TimeFrame(4 * SEC_PER_MINUTE);
        public static TimeFrame Minute5 => new TimeFrame(5 * SEC_PER_MINUTE);
        public static TimeFrame Minute6 => new TimeFrame(6 * SEC_PER_MINUTE);
        public static TimeFrame Minute10 => new TimeFrame(10 * SEC_PER_MINUTE);
        public static TimeFrame Minute15 => new TimeFrame(15 * SEC_PER_MINUTE);
        public static TimeFrame Minute20 => new TimeFrame(20 * SEC_PER_MINUTE);
        public static TimeFrame Minute30 => new TimeFrame(30 * SEC_PER_MINUTE);
        public static TimeFrame Minute45 => new TimeFrame(45 * SEC_PER_MINUTE);

        public static TimeFrame Hour => new TimeFrame(SEC_PER_HOUR);
        public static TimeFrame Hour2 => new TimeFrame(2 * SEC_PER_HOUR);
        public static TimeFrame Hour3 => new TimeFrame(3 * SEC_PER_HOUR);
        public static TimeFrame Hour4 => new TimeFrame(4 * SEC_PER_HOUR);
        public static TimeFrame Hour6 => new TimeFrame(6 * SEC_PER_HOUR);
        public static TimeFrame Hour8 => new TimeFrame(8 * SEC_PER_HOUR);
        public static TimeFrame Hour12 => new TimeFrame(12 * SEC_PER_HOUR);

        public static TimeFrame Daily => new TimeFrame(SEC_PER_DAY);
        public static TimeFrame Day2 => new TimeFrame(2 * SEC_PER_DAY);
        public static TimeFrame Day3 => new TimeFrame(3 * SEC_PER_DAY);
        public static TimeFrame Weekly => new TimeFrame(SEC_PER_WEEK);
        public static TimeFrame Monthly => new TimeFrame(4 * SEC_PER_WEEK);
    }
}
