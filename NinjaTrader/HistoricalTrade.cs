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

namespace cAlgoNt8Wrapper
{
    //     Represents the historical trade interface.
    public class HistoricalTrade
    {
        //
        // Summary:
        //     Gets the unique closing deal identifier.
        public int ClosingDealId;

        //
        // Summary:
        //     Gets the position unique identifier.
        public int PositionId;

        //
        // Summary:
        //     Gets the symbol name.
        public string SymbolName;

        //
        // Summary:
        //     The TradeType of the Opening Deal.
        public TradeType TradeType;

        //
        // Summary:
        //     The Volume that was closed by the Closing Deal.
        public double VolumeInUnits;

        //
        // Summary:
        //     Time of the Opening Deal, or the time of the first Opening deal that was closed.
        public DateTime EntryTime;

        //
        // Summary:
        //     The VWAP (Volume Weighted Average Price) of the Opening Deals that are closed.
        public double EntryPrice;

        //
        // Summary:
        //     Time of the Closing Deal.
        public DateTime ClosingTime;

        //
        // Summary:
        //     The execution price of the Closing Deal.
        public double ClosingPrice;

        //
        // Summary:
        //     The label.
        public string Label;

        //
        // Summary:
        //     The comment
        public string Comment;

        //
        // Summary:
        //     Commission owed
        public double Commissions;

        //
        // Summary:
        //     Swap is the overnight interest rate if any, accrued on the position.
        public double Swap;

        //
        // Summary:
        //     Profit and loss including swaps and commissions
        public double NetProfit;

        //
        // Summary:
        //     Profit and loss before swaps and commission
        public double GrossProfit;

        //
        // Summary:
        //     Account balance after the Deal was filled
        public double Balance;

        //
        // Summary:
        //     Represents the winning or loosing pips
        public double Pips;

        //
        // Summary:
        //     The Quantity (in lots) that was closed by the Closing Deal
        public double Quantity;

        //
        // Summary:
        //     Gets the trade symbol.
        public Symbol Symbol;
    }
}
