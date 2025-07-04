using System;

namespace TdsCommons
{
    // quantConnect bar class:
    // Time, OHLCV for Bid and Ask
    public class QcBar
    {
        public DateTime TimeOpen { get; set; }
        public double BidOpen { get; set; }
        public double BidHigh { get; set; }
        public double BidLow { get; set; }
        public double BidClose { get; set; }
        public long BidVolume { get; set; }
        public double AskOpen { get; set; }
        public double AskHigh { get; set; }
        public double AskLow { get; set; }
        public double AskClose { get; set; }
        public long AskVolume { get; set; }
    }
}
