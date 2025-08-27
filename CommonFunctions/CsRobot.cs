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
using cAlgo.API;
#endif
using NinjaTrader.NinjaScript.Strategies;
using System;

namespace RobotLib.Cs
{
    public class CsRobot : AbstractRobot
    {
        #region Members
        static public readonly string[] TimeZoneArray = new string[]
        {
         // <summary>(GMT-12:00) International Date Line West</summary>
         //public const string DatelineStandardTime = 
         "Dateline Standard Time",  // Index 0
         // <summary>(GMT-11:00) Midway Island, Samoa</summary>
         //public const string SamoaStandardTime = 
         "Samoa Standard Time",     // Index 1
         // <summary>(GMT-10:00) Hawaii</summary>
         // public const string HawaiianStandardTime = 
         "Hawaiian Standard Time",  // Index 2
         // <summary>(GMT-09:00) Alaska</summary>
         // public const string AlaskanStandardTime = 
         "Alaskan Standard Time",   // Index 3
         // <summary>(GMT-08:00) Pacific Time (US and Canada), Tijuana</summary>
         // public const string PacificStandardTime = 
         "Pacific Standard Time",   // Index 4
         // <summary>(GMT-07:00) Mountain Time (US and Canada)</summary>
         // public const string MountainStandardTime = 
         "Mountain Standard Time",  // Index 5
         // <summary>(GMT-06:00) Central Time (US and Canada</summary>
         //// public const string CentralStandardTime = "Central Standard Time",
         // <summary>(GMT-06:00) Saskatchewan</summary>
         //// public const string CanadaCentralStandardTime = "Canada Central Standard Time",
         // <summary>(GMT-06:00) Central America</summary>
         // public const string CentralAmericaStandardTime = 
         "Central America Standard Time", // Index 6
         // <summary>(GMT-05:00) Eastern Time (US and Canada)</summary>
         // public const string EasternStandardTime = 
         "Eastern Standard Time",   // Index 7
         // <summary>(GMT-04:00) Atlantic Time (Canada)</summary>
         // public const string AtlanticStandardTime = 
         "Atlantic Standard Time",  // Index 8
         // <summary>(GMT-03:00) Brasilia</summary>
         // public const string ESouthAmericaStandardTime = 
         "E. South America Standard Time",   // Index 9
         // <summary>(GMT-03:00) Greenland</summary>
         //// public const string GreenlandStandardTime = "Greenland Standard Time",
         // <summary>(GMT-02:00) Mid-Atlantic</summary>
         // public const string MidAtlanticStandardTime = 
         "Mid-Atlantic Standard Time", // Index 10
         // <summary>(GMT-01:00) Azores</summary>
         //// public const string AzoresStandardTime = "Azores Standard Time",
         // <summary>(GMT-01:00) Cape Verde Islands</summary>
         // public const string CapeVerdeStandardTime = 
         "Cape Verde Standard Time",   // Index 11
         // <summary>Coordinated Universal Time</summary>
         // public const string UTC = 
         "UTC",   // Index 12
         // <summary>(GMT) Greenwich Mean Time: Dublin, Edinburgh, Lisbon, London</summary>
         //// public const string GMTStandardTime = "GMT Standard Time",
         // <summary>(GMT) Casablanca, Monrovia</summary>
         //// public const string GreenwichStandardTime = "Greenwich Standard Time",
         // <summary>(GMT+01:00) Belgrade, Bratislava, Budapest, Ljubljana, Prague</summary>
         //// public const string CentralEuropeStandardTime = "Central Europe Standard Time",
         // <summary>(GMT+01:00) Sarajevo, Skopje, Warsaw, Zagreb</summary>
         //// public const string CentralEuropeanStandardTime = "Central European Standard Time",
         // <summary>(GMT+01:00) Brussels, Copenhagen, Madrid, Paris</summary>
         //// public const string RomanceStandardTime = "Romance Standard Time",
         // <summary>(GMT+01:00) Amsterdam, Berlin, Bern, Rome, Stockholm, Vienna</summary>
         // public const string WEuropeStandardTime = 
         "W. Europe Standard Time", // Index 13
         // <summary>(GMT+01:00) West Central Africa</summary>
         //// public const string WCentralAfricaStandardTime = "W. Central Africa Standard Time",
         // <summary>(GMT+02:00) Bucharest</summary>
         // public const string EEuropeStandardTime = 
         "E. Europe Standard Time", // Index 14
         // <summary>(GMT+02:00) Cairo</summary>
         //// public const string EgyptStandardTime = "Egypt Standard Time",
         // <summary>(GMT+02:00) Helsinki, Kiev, Riga, Sofia, Tallinn, Vilnius</summary>
         //// public const string FLEStandardTime = "FLE Standard Time",
         // <summary>(GMT+02:00) Athens, Istanbul, Minsk</summary>
         //// public const string GTBStandardTime = "GTB Standard Time",
         // <summary>(GMT+02:00) Jerusalem</summary>
         //// public const string IsraelStandardTime = "Israel Standard Time",
         // <summary>(GMT+02:00) Harare, Pretoria</summary>
         // public const string SouthAfricaStandardTime = 
         //"South Africa Standard Time",
         // <summary>(GMT+03:00) Moscow, St. Petersburg, Volgograd</summary>
         //// public const string RussianStandardTime = "Russian Standard Time",
         // <summary>(GMT+03:00) Kuwait, Riyadh</summary>
         //// public const string ArabStandardTime = "Arab Standard Time",
         // <summary>(GMT+03:00) Nairobi</summary>
         // public const string EAfricaStandardTime = 
         "E. Africa Standard Time", // Index 15
         // <summary>(GMT+03:00) Baghdad</summary>
         //// public const string ArabicStandardTime = "Arabic Standard Time",
         // <summary>(GMT+03:30) Tehran</summary>
         //// public const string IranStandardTime = "Iran Standard Time",
         // <summary>(GMT+04:00) Abu Dhabi, Muscat</summary>
         // public const string ArabianStandardTime = 
         "Arabian Standard Time",   // Index 16
         // <summary>(GMT+04:00) Baku, Tbilisi, Yerevan</summary>
         //// public const string CaucasusStandardTime = "Caucasus Standard Time",
         // <summary>(GMT+05:00) Ekaterinburg</summary>
         //// public const string EkaterinburgStandardTime = "Ekaterinburg Standard Time",
         // <summary>(GMT+05:00) Islamabad, Karachi, Tashkent</summary>
         // public const string WestAsiaStandardTime = 
         "West Asia Standard Time", // Index 17
         // <summary>(GMT+05:30) Chennai, Kolkata, Mumbai, New Delhi</summary>
         //// public const string IndiaStandardTime = "India Standard Time",
         // <summary>(GMT+05:45) Kathmandu</summary>
         //// public const string NepalStandardTime = "Nepal Standard Time",
         // <summary>(GMT+06:00) Astana, Dhaka</summary>
         // public const string CentralAsiaStandardTime = 
         "Central Asia Standard Time", // Index 18
         // <summary>(GMT+06:00) Sri Jayawardenepura</summary>
         //// public const string SriLankaStandardTime = "Sri Lanka Standard Time",
         // <summary>(GMT+06:00) Almaty, Novosibirsk</summary>
         //// public const string NCentralAsiaStandardTime = "N. Central Asia Standard Time",
         // <summary>(GMT+06:30) Yangon Rangoon</summary>
         //// public const string MyanmarStandardTime = "Myanmar Standard Time",
         // <summary>(GMT+07:00) Krasnoyarsk</summary>
         // public const string NorthAsiaStandardTime = 
         "North Asia Standard Time",   // Index 19
         // <summary>(GMT+08:00) Beijing, Chongqing, Hong Kong SAR, Urumqi</summary>
         // public const string ChinaStandardTime = 
         "China Standard Time",        // Index 20
         // <summary>(GMT+08:00) Kuala Lumpur, Singapore</summary>
         //// public const string SingaporeStandardTime = "Singapore Standard Time",
         // <summary>(GMT+08:00) Taipei</summary>
         //// public const string TaipeiStandardTime = "Taipei Standard Time",
         // <summary>(GMT+08:00) Perth</summary>
         //// public const string WAustraliaStandardTime = "W. Australia Standard Time",
         // <summary>(GMT+08:00) Irkutsk, Ulaanbaatar</summary>
         //// public const string NorthAsiaEastStandardTime = "North Asia East Standard Time",
         // <summary>(GMT+09:00) Seoul</summary>
         //// public const string KoreaStandardTime = "Korea Standard Time",
         // <summary>(GMT+09:00) Osaka, Sapporo, Tokyo</summary>
         // public const string TokyoStandardTime = 
         "Tokyo Standard Time",        // Index 21
         // <summary>(GMT+09:00) Yakutsk</summary>
         //// public const string YakutskStandardTime = "Yakutsk Standard Time",
         // <summary>(GMT+09:30) Adelaide</summary>
         //// public const string CenAustraliaStandardTime = "Cen. Australia Standard Time",
         // <summary>(GMT+10:00) Brisbane</summary>
         // public const string EAustraliaStandardTime = 
         //"E. Australia Standard Time",
         // <summary>(GMT+10:00) Hobart</summary>
         //// public const string TasmaniaStandardTime = "Tasmania Standard Time",
         // <summary>(GMT+10:00) Vladivostok</summary>
         //// public const string VladivostokStandardTime = "Vladivostok Standard Time",
         // <summary>(GMT+10:00) Guam, Port Moresby</summary>
         // public const string WestPacificStandardTime = 
         "West Pacific Standard Time",    // Index 22
         // <summary>(GMT+11:00) Magadan, Solomon Islands, New Caledonia</summary>
         // public const string CentralPacificStandardTime = 
         "Central Pacific Standard Time", // Index 23
         // <summary>(GMT+12:00) Auckland, Wellington</summary>
         // public const string NewZealandStandardTime = 
         "New Zealand Standard Time",     // Index 24
         // <summary>(GMT+13:00) Nuku'alofa</summary>
         // public const string TongaStandardTime = 
         "Tonga Standard Time",           // // Index 25
        };

        static public readonly TimeFrame[] TimeFrameArray = new TimeFrame[]
        {
            TimeFrame.Minute,
            TimeFrame.Minute5,
            TimeFrame.Minute10,
            TimeFrame.Minute15,
            TimeFrame.Minute20,
            TimeFrame.Minute30,
            TimeFrame.Minute45,
            TimeFrame.Hour,
            TimeFrame.Hour2,
            TimeFrame.Hour3,
            TimeFrame.Hour4,
            TimeFrame.Hour6,
            TimeFrame.Hour8,
            TimeFrame.Hour12,
            TimeFrame.Daily,
            TimeFrame.Day2,
            TimeFrame.Day3,
            TimeFrame.Weekly,
            null
        };
        #endregion

        public override string ConfigInit(Strategy robot, string timeZoneId = null)
        {
            string retVal = base.ConfigInit(robot, timeZoneId);
            if ("" == retVal)
            {
                mRobot.Positions.Opened += (PositionOpenedEventArgs args) =>
                {
                    DrawOnOpenedPosition(
                        args.Position.SymbolName,
                        args.Position.TradeType,
                        args.Position.EntryTime, args.Position.EntryPrice,
                        args.Position.Label);
                };

                mRobot.Positions.Closed += (PositionClosedEventArgs args) =>
                {
                    DrawOnClosedPosition(
                        args.Position.SymbolName,
                        args.Position.TradeType,
                        args.Position.EntryTime, args.Position.EntryPrice,
                        mRobot.Time,
                        args.Position.CurrentPrice,
                        args.Position.GrossProfit >= 0,
                        args.Position.Label);
                };
            }

            return retVal;
        }

        public override string GetSymbolTrail(string cTsymbol)
        {
            return cTsymbol;  // nothing to do here on cTrader
        }

        public override string GetSymbolPlain(string cTsymbol)
        {
            return cTsymbol;  // nothing to do here on cTrader
        }

        public override string DecodeStringFromLong(long lSymbol)
        {
            byte[] buffer = new byte[8];
            int i = 0;
            for (; i < buffer.Length; i++)
            {
                if (0 == (byte)lSymbol)
                    break;
                buffer[i] = (byte)lSymbol;
                lSymbol = lSymbol >> 8;
            }
            Array.Resize(ref buffer, i);
            return System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length).Trim();
        }
    }
}
