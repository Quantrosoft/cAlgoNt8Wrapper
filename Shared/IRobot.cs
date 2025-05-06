/* MIT License
Copyright (c) 2035 Quantrosoft Pty. Ltd.

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

using cAlgo.API;
using cAlgo.API.Internals;
using System;
using TdsCommons;
using static TdsDefs;

namespace RobotLib
{
    public interface IRobot
    {
        bool Is1stTick { get; }
        double InitialAccountBalance { get; }
        DateTime InitialTime { get; }
        DateTime PrevTime { get; }
        DataRateId DataRateId { get; }
        bool IsVisualMode { get; }
        bool IsRealtime { get; }
        bool IsVisible { get; }
        bool IsValidateTickData { get; set; }
        string CommentTab { get; }

        double LotPoint(Symbol symbol);
        double MinLot(Symbol symbol);
        double MaxLot(Symbol symbol);
        double StepLot(Symbol symbol);
        double CommissionPerLot(Symbol symbol);
        double SwapPerLot(Symbol symbol, bool isLongNotShort);
        int LotDigits(Symbol symbol);
        int SpreadInPoints(Symbol symbol);
        string ConfigInit(Robot robot, string timeZoneId = "");
        void DataLoadedInit(
            Action<PositionOpenedEventArgs> onPositionOpened = null,
            Action<PositionClosedEventArgs> onPositionClosed = null);
        void PreTick();
        void PostTick();
        void OpenLogfile(ILogger logger, string filename, LogFlags mode = LogFlags.HeaderAndSeveralLines, string header = "");
        GetCurrentTime GetCurrentTimeDelegate();
        double GetBidAskPrice(Symbol symbol, BidAsk tradeType);
        int iPrice(double price, double tickSize);

        string CalcProfitMode2Lots(
           Symbol symbol,
           ProfitMode profitMode,
           double value,
           int tpPts,
           int riskPoints,
           out double desiMon,
           out double lotSize);

        string CalcProfitMode2Volume(
           Symbol symbol,
           ProfitMode profitMode,
           double value,
           int tpPts,
           int riskPoints,
           out double desiMon,
           out double volume);

        double CalcPointsAndLot2Money(Symbol symbol, int points, double lot);

        double CalcPointsAndVolume2Money(Symbol symbol, int points, double volume);

        int CalcMoneyAndLot2Points(Symbol symbol, double money, double lot);

        int CalcMoneyAndVolume2Points(Symbol symbol, double money, double volume);

        double CalcMoneyAndPoints2Lots(Symbol symbol, double money, int points, double xProLot);

        void PrintComment(string version, string comment, int avgSpreadPts = -1, bool normNyTime = false);

        string MakeLogComment(Symbol symbol, string firstPart);

        void LoggerWriteHeader(string header = "");

        void LoggerAddText(string s);

        void LoggerClosingTrade(LogParams logParams);
        
        void LoggerFlush();
        
        void LoggerClose(string preText = "");

        string GetSymbolTrail(string cTsymbol);

        string GetSymbolPlain(string cTsymbol);

        string DecodeStringFromLong(long lSymbol);

        void DrawOnOpenedPosition(
           string symName,
           TradeType tt,
           DateTime entryTime, double entryPrice,
           string label);

        void DrawOnClosedPosition(
           string symName,
           TradeType tt,
           DateTime entryTime, double entryPrice,
           DateTime closeTime, double closePrice,
           bool profitLoss,
           string label);

        //bool IsNewBar(int timeframe, DateTime current, DateTime prev);

        Position OpenTrade(Symbol symbol,
           TradeType tradeType,
           TradeDirections allowedDirection,
           string commentVersion,
           string label,
           double volume);

        bool CloseTrade(Position pos);

        //ChartStaticText TextXy(Robot mBot, string text, int x, int y, Color color);

        // cTrader API
        void Print(string message, params object[] parameters);

        void Stop();

        Symbols Symbols { get; }
        MarketData MarketData { get; }
        DateTime Time { get; }
        Chart Chart { get; }
        Bars Bars { get; }
        IAccount Account { get; }

    }
}
// end of file
