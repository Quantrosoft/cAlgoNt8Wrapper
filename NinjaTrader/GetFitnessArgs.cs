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
#else
using NinjaTrader.NinjaScript.Strategies;
#endif

namespace cAlgoNt8Wrapper
{
    //
    // Summary:
    //     Represents the custom fitness calculation interface.
    public class GetFitnessArgs
    {
        //
        // Summary:
        //     Gets all the historical trades.
        public History History { get; }

        //
        // Summary:
        //     Gets all open positions.
        public Positions Positions { get; }

        //
        // Summary:
        //     Gets all pending orders.
        public PendingOrders PendingOrders { get; }

        //
        // Summary:
        //     Gets the equity of the account (balance plus unrealized profit and loss).
        public double Equity { get; }

        //
        // Summary:
        //     Gets the net profit of all trades in account deposit currency.
        public double NetProfit { get; }

        //
        // Summary:
        //     Gets the maximum amount of balance drawdown in percentage (ex: 40%). It can return
        //     a positive value between 0-100.
        public double MaxBalanceDrawdownPercentages { get; }

        //
        // Summary:
        //     Gets the maximum amount of equity drawdown in percentage (ex: 40%). It can return
        //     a positive value between 0-100.
        public double MaxEquityDrawdownPercentages { get; }

        //
        // Summary:
        //     Gets the maximum amount of balance drawdown in account deposit currency.
        public double MaxBalanceDrawdown { get; }

        //
        // Summary:
        //     Gets the maximum amount of equity drawdown in account deposit currency.
        public double MaxEquityDrawdown { get; }

        //
        // Summary:
        //     Gets the total number of winning trades.
        public double WinningTrades { get; }

        //
        // Summary:
        //     Gets total number of losing trades.
        public double LosingTrades { get; }

        //
        // Summary:
        //     Gets the total number of trades taken.
        public double TotalTrades { get; }

        //
        // Summary:
        //     Gets the average profit for all trades in account deposit currency.
        public double AverageTrade { get; }

        //
        // Summary:
        //     Gets the Profit Factor - the ratio of Total Net Profit divided by the Total Net
        //     Loss.
        public double ProfitFactor { get; }

        //
        // Summary:
        //     Gets the swaps of all trades in account deposit currency.
        public double Swaps { get; }

        //
        // Summary:
        //     Gets the commissions of all trades in account deposit currency.
        public double Commissions { get; }
    }
}
