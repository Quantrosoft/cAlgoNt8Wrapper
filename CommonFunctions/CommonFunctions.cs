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

#if CTRADER
using Newtonsoft.Json.Linq;
#endif
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace TdsCommons
{
    #region Enums
    /// <summary>
    /// Series array identifier used with ArrayCopySeries(), iHighest() and iLowest() functions.
    /// </summary>
    public enum SeriesMode
    {
        /// <summary>Open price.</summary>
        OPEN = 0,
        /// <summary>Low price.</summary>
        LOW = 1,
        /// <summary>High price.</summary>
        HIGH = 2,
        /// <summary>Close price.</summary>
        CLOSE = 3,
#if VOLUME
      /// <summary>Volume.</summary>
      VOLUME = 4,
#endif
        /// <summary>Bar open time.</summary>
        TIME = 5,
        /// <summary>Median Price in Time Series Array.</summary>
        MEDIAN = 6,
        /// <summary>Typical Price in Time Series Array.</summary>
        TYPICAL = 7,
        /// <summary>Weighted Price in Time Series Array.</summary>
        WEIGHTED = 8,
        /// <summary>Weighted Price in Time Series Array.</summary>
        HILO2 = 9,
        /// <summary>Do not normalie</summary>
        GENERAL = 10,
    }

    /// <summary>
    /// get/free a tsa or indicator array
    /// </summary>
    public enum ArrayMode
    {
        /// <summary>get the array instead of the value</summary>
        GET_ARRAY,
        /// <summary>free the array</summary>
        FREE_ARRAY
    }

    public enum ENUM_TIMEFRAMES
    {
        PERIOD_TICK = 0,
        PERIOD_M1 = 1 * 60,
        PERIOD_M5 = 5 * 60,
        PERIOD_M15 = 15 * 60,
        PERIOD_M30 = 30 * 60,
        PERIOD_H1 = 60 * 60,
        PERIOD_H4 = 240 * 60,
        PERIOD_D1 = 1440 * 60,
        PERIOD_W1 = 10080 * 60,
        PERIOD_MN1 = 43200 * 60,
    }

    public enum ENUM_DAY_OF_WEEK
    {
        INVALID_WEEK_DAY = -1,
        // week day definitions
        SUNDAY = 0,
        MONDAY = 1,
        TUESDAY = 2,
        WEDNESDAY = 3,
        THURSDAY = 4,
        FRIDAY = 5,
        SATURDAY = 6
    }

    /// <summary>
    /// Market information identifiers, used with MarketInfo() function.
    /// </summary>
    public enum InfoMode
    {
        /// <summary>
        /// Low day price.
        /// </summary>
        MODE_LOW = 1,
        /// <summary>
        /// High day price. 
        /// </summary>
        MODE_HIGH = 2,
        /// <summary>
        /// The last incoming tick time (last known mServer time). 
        /// </summary>
        MODE_TIME = 5,
        /// <summary>
        /// Last incoming bid price. For the current symbol, it is stored in the predefined variable Bid 
        /// </summary>
        MODE_BID = 9,
        /// <summary>
        /// Last incoming ask price. For the current symbol, it is stored in the predefined variable Ask 
        /// </summary>
        MODE_ASK = 10,
        /// <summary>
        /// _Point size in the quote currency. 
        /// </summary>
        MODE_POINT = 11,
        /// <summary>
        /// Count of digits after decimal point in the symbol prices. For the current symbol, it is stored in the predefined variable _Digits 
        /// </summary>
        MODE_DIGITS = 12,
        /// <summary>
        /// Spread value in points. 
        /// </summary>
        MODE_SPREAD = 13,
        /// <summary>
        /// Stop level in points. 
        /// </summary>
        MODE_STOPLEVEL = 14,
        /// <summary>
        /// Lot size in the base currency. 
        /// </summary>
        MODE_LOTSIZE = 15,
        /// <summary>
        /// DataRate value in the deposit currency. 
        /// </summary>
        MODE_TICKVALUE = 16,
        /// <summary>
        /// DataRate size in the quote currency. 
        /// </summary>
        MODE_TICKSIZE = 17,
        /// <summary>
        /// Swap of the long mPosition. 
        /// </summary>
        MODE_SWAPLONG = 18,
        /// <summary>
        /// Swap of the short mPosition. 
        /// </summary>
        MODE_SWAPSHORT = 19,
        /// <summary>
        ///  Market starting date (usually used for futures). 
        /// </summary>
        MODE_STARTING = 20,
        /// <summary>
        /// Market expiration date (usually used for futures). 
        /// </summary>
        MODE_EXPIRATION = 21,
        /// <summary>
        /// Trade is allowed for the symbol. 
        /// </summary>
        MODE_TRADEALLOWED = 22,
        /// <summary>
        /// Minimum permitted amount of a lot. 
        /// </summary>
        MODE_MINLOT = 23,
        /// <summary>
        /// Step for changing lots. 
        /// </summary>
        MODE_LOTSTEP = 24,
        /// <summary>
        /// Maximum permitted amount of a lot. 
        /// </summary>
        MODE_MAXLOT = 25,
        /// <summary>
        /// Swap calculation method. 0 - in points; 1 - in the symbol base currency; 2 - by interest; 3 - in the margin currency. 
        /// </summary>
        MODE_SWAPTYPE = 26,
        /// <summary>
        /// Profit calculation mode. 0 - Forex; 1 - CFD; 2 - Futures. 
        /// </summary>
        MODE_PROFITCALCMODE = 27,
        /// <summary>
        /// Margin calculation mode. 0 - Forex; 1 - CFD; 2 - Futures; 3 - CFD for indices. 
        /// </summary>
        MODE_MARGINCALCMODE = 28,
        /// <summary>
        /// Initial margin requirements for 1 lot. 
        /// </summary>
        MODE_MARGININIT = 29,
        /// <summary>
        /// Margin to maintain open positions calculated for 1 lot. 
        /// </summary>
        MODE_MARGINMAINTENANCE = 30,
        /// <summary>
        /// Hedged margin calculated for 1 lot. 
        /// </summary>
        MODE_MARGINHEDGED = 31,
        /// <summary>
        /// Free margin required to open 1 lot for buying. 
        /// </summary>
        MODE_MARGINREQUIRED = 32,
        /// <summary>
        /// Order freeze level in points. If the execution price lies within the range defined by the freeze level, the order cannot be modified, cancelled or closed. 
        /// </summary>
        MODE_FREEZELEVEL = 33,
        /// <summary>
        /// Allowed using OrderCloseBy() to close opposite orders on a specified symbol
        /// </summary>
        MODE_CLOSEBY_ALLOWED = 34,

        /// <summary></summary>
        TDS_BID_AT_TICK,
        /// <summary></summary>
        TDS_ASK_AT_TICK
    }

    public enum ENUM_SYMBOL_INFO_INTEGER
    {
        SYMBOL_SELECT = 0,
        SYMBOL_VISIBLE = 76,
        SYMBOL_SESSION_DEALS = 56,
        SYMBOL_SESSION_BUY_ORDERS = 60,
        SYMBOL_SESSION_SELL_ORDERS = 62,
        SYMBOL_VOLUME = 10,
        SYMBOL_VOLUMEHIGH = 11,
        SYMBOL_VOLUMELOW = 12,
        SYMBOL_TIME = 15,
        SYMBOL_DIGITS = 17,
        SYMBOL_SPREAD = 18,
        SYMBOL_SPREAD_FLOAT = 41,
        SYMBOL_TRADE_CALC_MODE = 29,
        SYMBOL_TRADE_MODE = 30,
        SYMBOL_START_TIME = 51,
        SYMBOL_EXPIRATION_TIME = 52,
        SYMBOL_TRADE_STOPS_LEVEL = 31,
        SYMBOL_TRADE_FREEZE_LEVEL = 32,
        SYMBOL_TRADE_EXEMODE = 33,
        SYMBOL_SWAP_MODE = 37,
        SYMBOL_SWAP_ROLLOVER3DAYS = 40,
        SYMBOL_EXPIRATION_MODE = 49,
        SYMBOL_FILLING_MODE = 50,
        SYMBOL_ORDER_MODE = 71
    }

    // https://docs.mql4.com/constants/environment_state/marketinfoconstants#enum_symbol_info_double
    public enum ENUM_SYMBOL_INFO_DOUBLE
    {
        SYMBOL_BID = 1, // Bid - best sell offer
        SYMBOL_BIDHIGH = 2, // Not supported
        SYMBOL_BIDLOW = 3, // Not supported
        SYMBOL_ASK = 4, // Ask - best buy offer
        SYMBOL_ASKHIGH = 5, // Not supported
        SYMBOL_ASKLOW = 6, // Not supported
        SYMBOL_LAST = 7, // Not supported
        SYMBOL_LASTHIGH = 8, // Not supported
        SYMBOL_LASTLOW = 9, // Not supported
        SYMBOL_POINT = 16, // SymbolName point value
        SYMBOL_TRADE_TICK_VALUE = 26, // Value of SYMBOL_TRADE_TICK_VALUE_PROFIT
        SYMBOL_TRADE_TICK_VALUE_PROFIT = 53, // Not supported
        SYMBOL_TRADE_TICK_VALUE_LOSS = 54, // Not supported
        SYMBOL_TRADE_TICK_SIZE = 27, // Minimal price change
        SYMBOL_TRADE_CONTRACT_SIZE = 28, // Trade contract size
        SYMBOL_VOLUME_MIN = 34, // Minimal volume for a deal
        SYMBOL_VOLUME_MAX = 35, // Maximal volume for a deal
        SYMBOL_VOLUME_STEP = 36, // Minimal volume change step for deal execution
        SYMBOL_VOLUME_LIMIT = 55, // Not supported
        SYMBOL_SWAP_LONG = 38, // Buy order swap value
        SYMBOL_SWAP_SHORT = 39, // Sell order swap value
        SYMBOL_MARGIN_INITIAL = 42, // Initial margin means the amount in the margin currency required for opening an order with the volume of one lot. It is used for checking a client's assets when he or she enters the market.
        SYMBOL_MARGIN_MAINTENANCE = 43, // The maintenance margin. If it is set, it sets the margin amount in the margin currency of the symbol, charged from one lot. It is used for checking a client's assets when his/her account state changes. If the maintenance margin is equal to 0, the initial margin is used.
        SYMBOL_MARGIN_LONG = 44, // Not supported
        SYMBOL_MARGIN_SHORT = 45, // Not supported
        SYMBOL_MARGIN_LIMIT = 46, // Not supported
        SYMBOL_MARGIN_STOP = 47, // Not supported
        SYMBOL_MARGIN_STOPLIMIT = 48, // Not supported
        SYMBOL_SESSION_VOLUME = 57, // Not supported
        SYMBOL_SESSION_TURNOVER = 58, // Not supported
        SYMBOL_SESSION_INTEREST = 59, // Not supported
        SYMBOL_SESSION_BUY_ORDERS_VOLUME = 61, // Not supported
        SYMBOL_SESSION_SELL_ORDERS_VOLUME = 63, // Not supported
        SYMBOL_SESSION_OPEN = 64, // Not supported
        SYMBOL_SESSION_CLOSE = 65, // Not supported
        SYMBOL_SESSION_AW = 66, // Not supported
        SYMBOL_SESSION_PRICE_SETTLEMENT = 67, // Not supported
        SYMBOL_SESSION_PRICE_LIMIT_MIN = 68, // Not supported
        SYMBOL_SESSION_PRICE_LIMIT_MAX = 69, // Not supported
    }

    public enum ENUM_SYMBOL_INFO_STRING
    {
        SYMBOL_CURRENCY_BASE = 22,
        SYMBOL_CURRENCY_QUOTE = 23,
        SYMBOL_CURRENCY_MARGIN = 24,
        SYMBOL_DESCRIPTION = 20,
        SYMBOL_PATH = 21
    }

    public enum ENUM_SYMBOL_TRADE_MODE
    {
        SYMBOL_TRADE_MODE_DISABLED = 0,
        SYMBOL_TRADE_MODE_LONGONLY = 1,
        SYMBOL_TRADE_MODE_SHORTONLY = 2,
        SYMBOL_TRADE_MODE_CLOSEONLY = 3,
        SYMBOL_TRADE_MODE_FULL = 4
    }

    public enum ENUM_SYMBOL_TRADE_EXECUTION
    {
        SYMBOL_TRADE_EXECUTION_REQUEST = 0,
        SYMBOL_TRADE_EXECUTION_INSTANT = 1,
        SYMBOL_TRADE_EXECUTION_MARKET = 2,
        SYMBOL_TRADE_EXECUTION_EXCHANGE = 3
    }

    public enum TIME
    {
        /// <summary>
        /// 
        /// </summary>
        TIME_DATE = 1,
        /// <summary>
        /// 
        /// </summary>
        TIME_MINUTES = 2,
        /// <summary>
        /// 
        /// </summary>
        TIME_SECONDS = 4,
    }

    public enum MT4Timeframe
    {
        M1 = 1,
        M5 = 5,
        M15 = 15,
        M30 = 30,
        H1 = 60,
        H4 = 240,
        D1 = 1440,
    }

    public enum ParameterFormat { Comment, ListBoxHeader, ObjectListView };

    public enum CollectTickMode { Pipe, FileTds, FileCtrader };

    public enum CorrectGap { Yes, No };

    public enum BarRegularHeikinashi { Regular, HeikinAshi }

    public enum BarCreationType { ConstantTimeframe, MovingBars, cTraderBars }

    public enum DataRateId { Undefined, Ticks, Minutes, Timeframe }

    public enum IndicatorSeries
    {
        Open,
        Low,
        High,
        Close,
        HiLo2,
    }

    public enum TradeStates
    {
        Long,    // must be same value as TradeType.Buy
        Short,   // must be same value as TradeType.Sell
        Flat,
    }

    public enum HighLow
    {
        High,
        Low
    }

    public enum TradeAction
    {
        Open,
        Close
    }

    public enum BidAsk
    {
        Bid,
        Ask
    };

    public enum ArithmeticOperators
    {
        Min,
        Max,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,
        Equal
    }

    public enum TradeDirectionsEnhanced
    {
        Long,
        Short,
        na,
        AllInOneBot,
        Neither,
        Mode1,
        Mode2,
        Mode3,
        Mode4
    };

    public enum OrderMode
    {
        Netting,
        Hedging
    }

    public enum PastFuture
    {
        Past,
        Future
    };

    public enum PriceMode
    {
        Value,
        Previous,
        Delta
    };

    public enum TimeFrameNdx
    {
        Minute = 0, // 0
        Minute5,    // 1
        Minute10,   // 2
        Minute15,   // 3
        Minute20,   // 4
        Minute30,   // 5
        Minute45,   // 6
        Hour,       // 7
        Hour2,      // 8
        Hour3,      // 9
        Hour4,      // 10
        Hour6,      // 11
        Hour8,      // 12
        Hour12,     // 13
        Daily,      // 14
        Day2,       // 15
        Day3,       // 16
        Weekly,     // 17
        FromConfFile// 18
    }

    public enum CommonTimeZones
    {
        NewZealandStandardTime_a12,
        SolomonIslandsTime_a11,
        AustralianEasternStandardTime_a10,
        JapanStandardTime_a9,
        ChinaStandardTime_a8,
        IndochinaTime_a7,
        BangladeshStandardTime_a6,
        IndiaStandardTime_a5_30,
        PakistanStandardTime_a5,
        GulfStandardTime_a4,
        MoscowStandardTime_a3,
        EasternEuropeanStandardTime_a2,
        CentralEuropeanStandardTime_a1,
        GreenwichMeanTime_a0,
        UTC_a0,
        CapeVerdeStandardTime_p1,
        AzoresStandardTime_p2,
        ArgentinaStandardTime_p3,
        AtlanticStandardTime_p4,
        EasternStandardTime_p5,
        CentralStandardTime_p6,
        MountainStandardTime_p7,
        PacificStandardTime_p8,
        AlaskaStandardTime_p9,
        HawaiiStandardTime_p10,
        SamoaStandardTime_p11,
        BakerIslandTime_p12,
        na
    }

    public enum LogModes
    {
        Off,
        File,
        Print,
        FileAndPrint
    }

    public enum LogFlags
    {
        HeaderAndSeveralLines = 0,
        NoHeader = 1,
        OneLine = 2,
        SelfMade = 4,
        NoCloseLog = 8,
        LogFile = 16,
        LogPrint = 32
    }

#if CTRADER
    public enum MarketDataType
    {
        Ask,
        Bid,
        Last,
        DailyHigh,
        DailyLow,
        DailyVolume,
        LastClose,
        Opening,
        OpenInterest,
        Settlement,
        Unknown
    }
#endif
#endregion

    public class CoFu
    {
        #region Constants
        static public readonly DateTime TimeInvalid = new DateTime(1970, 1, 1, 0, 0, 0, 0); // needed to convert DateTime and Mt4 datetime
        static public readonly long DateTime2EpocDiff = TimeInvalid.Ticks / TdsDefs.HECTONANOSEC_PER_SEC;
        public static readonly CultureInfo UsCulture = new CultureInfo("en-US");
        static public readonly TimeZoneInfo NytTzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        // ICM ==> Pepperstone symbol convert
        static public Dictionary<string, string> Icm2Pepper = new Dictionary<string, string>
        {
            {"STOXX50", "EUSTX50"},
            {"F40", "FRA40"},
            {"DE40", "GER40"},
            {"JP225", "JPN225"},
            {"ES35", "SPA35"},
            {"USTEC", "NAS100"},
            {"TecDE30", "GERTEC30"},
            {"XBRUSD", "SpotBrent"},
            {"XTIUSD", "SpotCrude"},
            {"XNGUSD", "NatGas"}
        };

        private static readonly Dictionary<CommonTimeZones, string> TimeZoneIdMap
            = new Dictionary<CommonTimeZones, string>
        {
            { CommonTimeZones.NewZealandStandardTime_a12, "New Zealand Standard Time" },
            { CommonTimeZones.SolomonIslandsTime_a11, "Solomon Islands Standard Time" },
            { CommonTimeZones.AustralianEasternStandardTime_a10, "AUS Eastern Standard Time" },
            { CommonTimeZones.JapanStandardTime_a9, "Tokyo Standard Time" },
            { CommonTimeZones.ChinaStandardTime_a8, "China Standard Time" },
            { CommonTimeZones.IndochinaTime_a7, "SE Asia Standard Time" },
            { CommonTimeZones.BangladeshStandardTime_a6, "Bangladesh Standard Time" },
            { CommonTimeZones.IndiaStandardTime_a5_30, "India Standard Time" },
            { CommonTimeZones.PakistanStandardTime_a5, "Pakistan Standard Time" },
            { CommonTimeZones.GulfStandardTime_a4, "Arabian Standard Time" },
            { CommonTimeZones.MoscowStandardTime_a3, "Russian Standard Time" },
            { CommonTimeZones.EasternEuropeanStandardTime_a2, "E. Europe Standard Time" },
            { CommonTimeZones.CentralEuropeanStandardTime_a1, "Central Europe Standard Time" },
            { CommonTimeZones.GreenwichMeanTime_a0, "GMT Standard Time" },
            { CommonTimeZones.UTC_a0, "UTC" },
            { CommonTimeZones.CapeVerdeStandardTime_p1, "Cape Verde Standard Time" },
            { CommonTimeZones.AzoresStandardTime_p2, "Azores Standard Time" },
            { CommonTimeZones.ArgentinaStandardTime_p3, "Argentina Standard Time" },
            { CommonTimeZones.AtlanticStandardTime_p4, "Atlantic Standard Time" },
            { CommonTimeZones.EasternStandardTime_p5, "Eastern Standard Time" },
            { CommonTimeZones.CentralStandardTime_p6, "Central Standard Time" },
            { CommonTimeZones.MountainStandardTime_p7, "Mountain Standard Time" },
            { CommonTimeZones.PacificStandardTime_p8, "Pacific Standard Time" },
            { CommonTimeZones.AlaskaStandardTime_p9, "Alaskan Standard Time" },
            { CommonTimeZones.HawaiiStandardTime_p10, "Hawaiian Standard Time" },
            { CommonTimeZones.SamoaStandardTime_p11, "Samoa Standard Time" },
            { CommonTimeZones.BakerIslandTime_p12, "Dateline Standard Time" }
        };

        public static TimeZoneInfo ToTimeZoneInfo(CommonTimeZones tzEnum)
        {
            if (TimeZoneIdMap.TryGetValue(tzEnum, out var timeZoneId))
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                }
                catch (TimeZoneNotFoundException)
                {
                    Console.WriteLine($"Time zone ID not found: {timeZoneId}");
                }
                catch (InvalidTimeZoneException)
                {
                    Console.WriteLine($"Invalid time zone ID: {timeZoneId}");
                }
            }
            return null;
        }
        #endregion

        #region Long/Short and other arithmetic
        // Summary:
        //     Least-Squares fitting the points (x,y) to a line y : x -> a+b*x, returning its
        //     best fitting parameters as (a, b) tuple, where a is the intercept and b the slope.
        //
        //
        // Parameters:
        //   x:
        //     Predictor (independent)
        //
        //   y:
        //     Response (dependent)
        public static (double A, double B) Fit(double[] x, double[] y)
        {
            if (x.Length != y.Length)
            {
                throw new ArgumentException($"All sample vectors must have the same length. However, vectors with disagreeing length {x.Length} and {y.Length} have been provided. A sample with index i is given by the value at index i of each provided vector.");
            }

            if (x.Length <= 1)
            {
                throw new ArgumentException($"A regression of the requested order requires at least {2} samples. Only {x.Length} samples have been provided.");
            }

            double num = 0.0;
            double num2 = 0.0;
            for (int i = 0; i < x.Length; i++)
            {
                num += x[i];
                num2 += y[i];
            }

            num /= x.Length;
            num2 /= y.Length;
            double num3 = 0.0;
            double num4 = 0.0;
            for (int j = 0; j < x.Length; j++)
            {
                double num5 = x[j] - num;
                num3 += num5 * (y[j] - num2);
                num4 += num5 * num5;
            }

            double num6 = num3 / num4;
            return (num2 - num6 * num, num6);
        }

        // Summary:
        //     Calculates r^2, the square of the sample correlation coefficient between the
        //     observed outcomes and the observed predictor values. Not to be confused with
        //     R^2, the coefficient of determination, see MathNet.Numerics.GoodnessOfFit.CoefficientOfDetermination(System.Collections.Generic.IEnumerable{System.Double},System.Collections.Generic.IEnumerable{System.Double}).
        //
        //
        // Parameters:
        //   modelledValues:
        //     The modelled/predicted values
        //
        //   observedValues:
        //     The observed/actual values
        //
        // Returns:
        //     Squared Person product-momentum correlation coefficient.
        public static double RSquared(IEnumerable<double> modelledValues, IEnumerable<double> observedValues)
        {
            double num = Pearson(modelledValues, observedValues);
            return num * num;
        }

        // Summary:
        //     Computes the Pearson Product-Moment Correlation coefficient.
        //
        // Parameters:
        //   dataA:
        //     Sample data A.
        //
        //   dataB:
        //     Sample data B.
        //
        // Returns:
        //     The Pearson product-moment correlation coefficient.
        public static double Pearson(IEnumerable<double> dataA, IEnumerable<double> dataB)
        {
            int num = 0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            double num5 = 0.0;
            double num6 = 0.0;
            using (IEnumerator<double> enumerator = dataA.GetEnumerator())
            {
                IEnumerator<double> enumerator2 = dataB.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (!enumerator2.MoveNext())
                    {
                        throw new ArgumentOutOfRangeException("dataB", "The array arguments must have the same length.");
                    }

                    double current = enumerator.Current;
                    double current2 = enumerator2.Current;
                    double num7 = current - num3;
                    double num8 = num7 / ++num;
                    double num9 = current2 - num4;
                    double num10 = num9 / num;
                    num3 += num8;
                    num4 += num10;
                    num5 += num8 * num7 * (num - 1);
                    num6 += num10 * num9 * (num - 1);
                    num2 += num7 * num9 * (num - 1) / num;
                }

                if (enumerator2.MoveNext())
                {
                    throw new ArgumentOutOfRangeException("dataA", "The array arguments must have the same length.");
                }
            }

            return num2 / Math.Sqrt(num5 * num6);
        }

        public static bool IsGreaterOrEqualLong(bool longNotShort, double val1, double val2)
        {
            if (longNotShort)
                return (val1 >= val2);
            else
                return (val1 <= val2);
        }

        public static bool IsLessOrEqualLong(bool longNotShort, double val1, double val2)
        {
            if (!longNotShort)
                return (val1 >= val2);
            else
                return (val1 <= val2);
        }

        public static bool IsGreaterLong(bool longNotShort, double val1, double val2)
        {
            if (longNotShort)
                return (val1 > val2);
            else
                return (val1 < val2);
        }

        public static bool IsLessLong(bool longNotShort, double val1, double val2)
        {
            if (!longNotShort)
                return (val1 > val2);
            else
                return (val1 < val2);
        }

        public static bool IsCrossing(bool longNotShort, int aCurrent, int aPrev, int bCurrent, int bPrev)
        {
            return (IsGreaterOrEqualLong(longNotShort, aCurrent, bCurrent) && IsLessOrEqualLong(longNotShort, aPrev, bPrev));
        }

        public static bool IsCrossing(bool longNotShort, double aCurrent, double aPrev, double bCurrent, double bPrev)
        {
            return (IsGreaterOrEqualLong(longNotShort, aCurrent, bCurrent) && IsLessOrEqualLong(longNotShort, aPrev, bPrev));
        }

        public static double AddLong(bool longNotShort, double val1, double val2)
        {
            if (longNotShort)
                return (val1 + val2);
            else
                return (val1 - val2);
        }

        public static int AddLong(bool longNotShort, int val1, int val2)
        {
            if (longNotShort)
                return (val1 + val2);
            else
                return (val1 - val2);
        }

        public static double SubLong(bool longNotShort, double val1, double val2)
        {
            if (!longNotShort)
                return (val1 + val2);
            else
                return (val1 - val2);
        }

        public static int SubLong(bool longNotShort, int val1, int val2)
        {
            if (!longNotShort)
                return (val1 + val2);
            else
                return (val1 - val2);
        }

        public static int DiffLong(bool longNotShort, int val1, int val2)
        {
            return longNotShort ? val1 - val2 : val2 - val1;
        }

        public static double DiffLong(bool longNotShort, double val1, double val2)
        {
            return longNotShort ? val1 - val2 : val2 - val1;
        }

        public static bool MaxLong(bool longNotShort, ref double val1, double val2)
        {
            if (longNotShort)
                return Max(ref val1, val2);
            else
                return Min(ref val1, val2);
        }

        public static bool MaxLong(bool longNotShort, ref int val1, int val2)
        {
            if (longNotShort)
                return Max(ref val1, val2);
            else
                return Min(ref val1, val2);
        }

        public static double MaxLong(bool longNotShort, double val1, double val2, double val3)
        {
            if (longNotShort)
                return (Math.Max(Math.Max(val1, val2), val3));
            else
                return (Math.Min(Math.Min(val1, val2), val3));
        }

        public static bool MinLong(bool longNotShort, ref double val1, double val2)
        {
            if (!longNotShort)
                return Max(ref val1, val2);
            else
                return Min(ref val1, val2);
        }

        public static bool MinLong(bool longNotShort, ref int val1, int val2)
        {
            if (!longNotShort)
                return Max(ref val1, val2);
            else
                return Min(ref val1, val2);
        }

        public static double MinLong(bool longNotShort, double val1, double val2, double val3)
        {
            if (!longNotShort)
                return (Math.Max(Math.Max(val1, val2), val3));
            else
                return (Math.Min(Math.Min(val1, val2), val3));
        }

        public static bool IsBetween(double val1, double middle, double val2)
        {
            return (val1 < middle && middle < val2) || (val1 > middle && middle > val2);
        }

        public static bool IsBetweenEqual(double val1, double middle, double val2)
        {
            return (val1 <= middle && middle <= val2) || (val1 >= middle && middle >= val2);
        }

        public static bool Max(ref long value, long compare)
        {
            if (compare > value)
            { value = compare; return true; }
            return false;
        }
        public static bool Max(ref int value, int compare)
        {
            if (compare > value)
            { value = compare; return true; }
            return false;
        }
        public static bool Max(ref double value, double compare)
        {
            if (compare > value)
            { value = compare; return true; }
            return false;
        }
        public static bool Max(ref float value, float compare)
        {
            if (compare > value)
            { value = compare; return true; }
            return false;
        }
        public static bool Min(ref long value, long compare)
        {
            if (compare < value)
            { value = compare; return true; }
            return false;
        }
        public static bool Min(ref int value, int compare)
        {
            if (compare < value)
            { value = compare; return true; }
            return false;
        }
        public static bool Min(ref double value, double compare)
        {
            if (compare < value)
            { value = compare; return true; }
            return false;
        }
        public static bool Min(ref float value, float compare)
        {
            if (compare < value)
            { value = compare; return true; }
            return false;
        }

        public static bool IsNewBar(int timeframe, DateTime current, DateTime prev)
        {
            return IsNewBar(timeframe, current.ToNativeSec(), prev.ToNativeSec());
        }

        public static bool IsNewBar(int timeframe, long current, long prev)
        {
            if (current <= CoFu.TimeInvalid.ToNativeSec() || prev <= CoFu.TimeInvalid.ToNativeSec())
                return false;
            return prev / timeframe != current / timeframe;
        }

        public static double SharpeSortino(bool isSortino, IEnumerable<double> vals)
        {
            if (vals.Count() < 2)
                return double.NaN;
            double average = vals.Average();
            double sd = Math.Sqrt(vals.Where(val => (!isSortino || val < average)).Select(val => (val - average) * (val - average)).Sum() / (vals.Count() - 1));
            return average / sd;
        }

        /// <summary>
        /// Convert a double price to signed integer
        /// </summary>
        public static int iPrice(double dPrice, double tickSize)
        {
            return Math.Sign(dPrice) * (int)(0.5 + Math.Abs(dPrice) / tickSize);
        }

        /// <summary>
        /// Convert an u integer price to double
        /// </summary>
        public static double dPrice(uint iPrice, double tickSize)
        {
            return tickSize * iPrice;
        }

        /// <summary>
        /// Convert an integer price to double
        /// </summary>
        public static double dPrice(int iPrice, double tickSize)
        {
            return tickSize * iPrice;
        }
        #endregion

        #region Get several different times
        //Get a NTP time from NIST
        //do not request a nist date more than once every 4 seconds, or the connection will be refused.
        //more servers at tf.nist.gov/tf-cgi/servers.cgi
        static public long GetNIST2dt(string tzid)
        {
            DateTime nistDateTime = DateTime.MinValue;
            try
            {
                using (var response = WebRequest.Create("http://www.google.com").GetResponse())
                    nistDateTime = DateTime.ParseExact(response.Headers["date"], "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                       CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AdjustToUniversal);
            }
            catch (WebException)
            {
                nistDateTime = DateTime.UtcNow;
            }

            nistDateTime = DateTime.SpecifyKind(nistDateTime, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(nistDateTime, TimeZoneInfo.FindSystemTimeZoneById(tzid)).ToNativeSec();
        }

        static public DateTime ConvertTimeFromUTC(DateTime utc, string tzid)
        {
            if ("" == tzid)
                return utc.ToLocalTime();

            return TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.FindSystemTimeZoneById(tzid));
        }

        static public long ConvertTimeFromUTC(long utc, string tzid)
        {
            return ConvertTimeFromUTC(utc.FromNativeSec(), tzid).ToNativeSec();
        }

        static public DateTime ConvertTimeToUTC(DateTime time, string tzid)
        {
            return TimeZoneInfo.ConvertTimeToUtc(time, TimeZoneInfo.FindSystemTimeZoneById(tzid));
        }

        static public long ConvertTimeToUTC(long time, string tzid)
        {
            return ConvertTimeToUTC(time.FromNativeSec(), tzid).ToNativeSec();
        }

        /// <summary>
        /// 17:00 New York Time is 00:00 "Normalized New York time"
        /// Using "Normalized New York time" has the benefit that daily/weekend gaps occure always around midnight
        /// independent of Daylight saving time active or not. However,as a consequence be aware that UTC
        /// is differnt in summer vs. winter. This time is used by some MT5 brokers (Pepperstone, ICMarket)
        /// as the mServer Time      
        /// </summary>
        /// <param name="time"></param>
        /// <param name="normalize"> It true, normalize NY time 17:00 to midnight</param>
        /// <returns>Normalized New York Time</returns>
        static public DateTime TimeUtc2Nyt(DateTime utc, bool normalize = false)
        {
            var nyTime = TimeZoneInfo.ConvertTimeFromUtc(utc, NytTzi);
            return normalize ? nyTime + TimeSpan.FromHours(7) : nyTime;
        }

        static public long TimeUtc2Nyt(long utc, bool normalize = false)
        {
            return TimeUtc2Nyt(utc.FromNativeSec(), normalize).ToNativeSec();
        }

        static public DateTime TimeNyt2Utc(DateTime nyt, bool normalize = false)
        {
            if (normalize)
                nyt -= TimeSpan.FromHours(7);
            return TimeZoneInfo.ConvertTimeToUtc(nyt, NytTzi);
        }

        static public long TimeNyt2Utc(long nyt, bool normalize = false)
        {
            return TimeNyt2Utc(nyt.FromNativeSec(), normalize).ToNativeSec();
        }

        /// <summary>
        /// Gets the time of the local computer also while backtesting.
        /// MT4's TimeLocal() returns TimeCurrent()
        /// </summary>
        /// <returns>Local time</returns>
        // cause c# does not include the return type into the signature, we use type as an argument to return type
        static public long GetLocalTime()
        {
            return DateTime.Now.ToNativeSec();
        }

        /// <summary>
        /// Parse time and date with the requested format
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        static public long ParseDate(string date, string format)
        {
            return DateTime.ParseExact(date, format, CultureInfo.InvariantCulture).ToNativeSec();
        }

        static public double ParseDouble(string value, string culture)
        {
            return double.Parse(
               value,
               NumberStyles.Any,
               CultureInfo.GetCultureInfo(culture));
        }

        static public bool TimeIsBetween(DateTime time, int hStart, int mStart, int hEnd, int mEnd)
        {
            long uNy = time.ToNativeSec() % TdsDefs.SEC_PER_DAY;    //MQ4_removeLine//
                                                                    //long lNy = time % TdsDefs::SEC_PER_DAY;  //MQ4_enable//
            int startSec = hStart * TdsDefs.SEC_PER_HOUR + mStart * TdsDefs.SEC_PER_MINUTE;
            int endSec = hEnd * TdsDefs.SEC_PER_HOUR + mEnd * TdsDefs.SEC_PER_MINUTE;
            return startSec <= uNy && uNy <= endSec;
        }
        #endregion

        #region Methods
        public static bool GetParameterFromConfigFile(string config,
            string objectName,
            string propertyName,
            out string value,
            bool isXml = false)
        {
            value = null;

            if (isXml)
            {
                try
                {
                    var doc = XDocument.Parse(config);

                    // Navigate to the nested structure: StrategyTemplate > Strategy > objectName
                    var targetElement = doc.Root?
                        .Element("Strategy")?
                        .Element(objectName)?
                        .Element(propertyName);

                    if (targetElement != null)
                    {
                        value = targetElement.Value;
                        return true;
                    }
                }
                catch
                {
                    // Handle or log malformed XML if needed
                }

                return false;
            }
#if CTRADER
            try
            {
                // JSON fallback
                JObject jObject = JObject.Parse(config);
                var parameters = jObject[objectName];

                if (parameters != null && parameters[propertyName] != null)
                {
                    var jToken = parameters[propertyName];

                    value = (jToken.Type == JTokenType.Float || jToken.Type == JTokenType.Integer)
                        ? string.Format(CultureInfo.InvariantCulture, "{0}", ((JValue)jToken).Value)
                        : jToken.ToString();

                    return true;
                }
            }
            catch
            {
                // Handle or log malformed JSON if needed
            }
#endif
            return false;
        }

        static public string ReadCtDayV1(
            string symbolFile,
            DateTime dayDate,
            ref int targetNdx,
            ref SerialArrays sa)
        {
            var fileName = Path.Combine(symbolFile,
                dayDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".zticks");

            if (!File.Exists(fileName))
                return "Tickdata file " + fileName + " not found";

            byte[] ba;
            using (GZipStream decompressor = new GZipStream(
                new FileStream(fileName, FileMode.Open, FileAccess.Read), CompressionMode.Decompress))
            {
                using MemoryStream to = new MemoryStream();
                decompressor.CopyTo(to);
                ba = to.ToArray();
            }

            int sourceNdx = 0;
            while (sourceNdx + 24 <= ba.Length)
            {
                if (targetNdx >= sa.Tick2NativeMs.Length)
                {
                    int size = sa.Tick2NativeMs.Length == 0 ? 10000 : sa.Tick2NativeMs.Length * 2;
                    Array.Resize(ref sa.Tick2NativeMs, size);
                    Array.Resize(ref sa.Tick2Bid, size);
                    Array.Resize(ref sa.Tick2Ask, size);
                }

                // 1. Read timestamp (little-endian Int64)
                sa.Tick2NativeMs[targetNdx] = BitConverter.ToInt64(ba, sourceNdx);
                var debugDt = sa.Tick2NativeMs[targetNdx].FromNativeMs();
                sourceNdx += 8;

                var bid = (double)BitConverter.ToUInt64(ba, sourceNdx) / 1e5;
                sourceNdx += 8;

                var ask = (double)BitConverter.ToUInt64(ba, sourceNdx) / 1e5;
                sourceNdx += 8;

                sa.Tick2Bid[targetNdx] = bid == 0 ? (targetNdx == 0 ? ask : sa.Tick2Bid[targetNdx - 1]) : bid;
                sa.Tick2Ask[targetNdx] = ask == 0 ? (targetNdx == 0 ? bid : sa.Tick2Ask[targetNdx - 1]) : ask;
                targetNdx++;
            }

            Array.Resize(ref sa.Tick2NativeMs, targetNdx);
            Array.Resize(ref sa.Tick2Bid, targetNdx);
            Array.Resize(ref sa.Tick2Ask, targetNdx);

            return "";
        }

        static public string WriteCtDayV1(
            string symbolFile,
            DateTime dayDate,
            in SerialArrays sa)
        {
            var fileName = Path.Combine(symbolFile,
                dayDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".zticks");

            try
            {
                using var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                using var gzipStream = new GZipStream(fileStream, CompressionMode.Compress);
                using var writer = new BinaryWriter(gzipStream);

                for (int i = 0; i < sa.Tick2NativeMs.Length; i++)
                {
                    // 1. Timestamp (little-endian Int64)
                    writer.Write(sa.Tick2NativeMs[i]);

                    // 2. Bid price as UInt32 (scaled by 1e5)
                    var bidInt = (ulong)Math.Round(sa.Tick2Bid[i] * 1e5);
                    writer.Write(bidInt);

                    // 3. Ask price as UInt32 (scaled by 1e5)
                    var askInt = (ulong)Math.Round(sa.Tick2Ask[i] * 1e5);
                    writer.Write(askInt);
                }

                return "";
            }
            catch (Exception ex)
            {
                return $"Error writing tickdata to file {fileName}: {ex.Message}";
            }
        }
#endregion
    }
}
