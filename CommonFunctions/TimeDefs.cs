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

public class TdsDefs
{
    // 1 sec = 1000 millisec = 1,000,000 microsec = 1,000,000,000 nanosec = 10,000,000 100-nanosec  
    // 1 microsec = 1000 nanosec = 10 100-nanosec 
    // 100 nanosec = 0.1 microsec
    /// <summary>
    /// Milliseconds per second.
    /// </summary>
    public const int MILLISEC_PER_SEC = (1000);

    /// <summary>
    /// Microseconds per second.
    /// </summary>
    public const int MICROSEC_PER_SEC = (1000000);

    /// <summary>
    /// Nanoseconds per second.
    /// </summary>
    public const int NANOSEC_PER_SEC = (1000000000);

    /// <summary>
    /// Hectonanoseconds per second.
    /// </summary>
    public const int HECTONANOSEC_PER_SEC = (10000000);

    /// <summary>
    /// Nanoseconds per millisecond.
    /// </summary>
    public const int NANOSEC_PER_MILLISEC = (1000000);

    /// <summary>
    /// Nanoseconds per microsecond.
    /// </summary>
    public const int NANOSEC_PER_MICROSEC = (1000);

    /// <summary>
    /// Hours per day.
    /// </summary>
    public const int HOUR_PER_DAY = (24);

    /// <summary>
    /// Minutes per hour.
    /// </summary>
    public const int MIN_PER_HOUR = (60);

    /// <summary>
    /// Minutes per day.
    /// </summary>
    public const int MIN_PER_DAY = (MIN_PER_HOUR * HOUR_PER_DAY);

    /// <summary>
    /// Seconds per minute.
    /// </summary>
    public const int SEC_PER_MINUTE = (60);

    /// <summary>
    /// Seconds per hour.
    /// </summary>
    public const int SEC_PER_HOUR = (MIN_PER_HOUR * SEC_PER_MINUTE);

    /// <summary>
    /// Seconds per day.
    /// </summary>
    public const int SEC_PER_DAY = (SEC_PER_HOUR * 24);

    /// <summary>
    /// Seconds per day.
    /// </summary>
    public const int SEC_PER_WEEK = (SEC_PER_DAY * 7);

    /// <summary>
    /// MT4 Epoc datetime starts on 1.1.1970 what was a Thursday (WeekOfDay == 4). 
    /// Add this offset to get Sunday
    /// </summary>
    public const int EPOC_WEEKDAY_SEC_OFFSET = (3 * SEC_PER_DAY);
}
