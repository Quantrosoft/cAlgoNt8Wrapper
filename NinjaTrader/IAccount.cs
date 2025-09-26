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
    //     Contains the current account information.
    public interface IAccount
    {
        //
        // Summary:
        //     Returns the current account type.
        //AccountType AccountType { get; }

        //
        // Summary:
        //     Returns the balance of the current account.
        double Balance { get; }

        //
        // Summary:
        //     Represents the equity of the current account (balance minus Unrealized Net Loss
        //     plus Unrealized Net Profit plus Bonus).
        double Equity { get; }

        //
        // Summary:
        //     Represents the margin of the current account.
        double Margin { get; }

        //
        // Summary:
        //     Represents the free margin of the current account.
        double FreeMargin { get; }

        //
        // Summary:
        //     Represents the margin level of the current account. Margin Level (in %) is calculated
        //     using this formula: Equity / Margin * 100
        double? MarginLevel { get; }

        //
        // Summary:
        //     Defines if the account is Live or Demo. True if the Account is Live, False if
        //     it is a Demo.
        bool IsLive { get; }

        //
        // Summary:
        //     Returns the number of the current account, e.g. 123456.
        int Number { get; }

        //
        // Summary:
        //     Returns the broker name of the current account.
        string BrokerName { get; }

        //
        // Summary:
        //     Gets the Unrealized Gross profit value.
        double UnrealizedGrossProfit { get; }

        //
        // Summary:
        //     Gets the Unrealized Net profit value.
        double UnrealizedNetProfit { get; }

        //
        // Summary:
        //     Gets the precise account leverage value.
        double PreciseLeverage { get; }

        //
        // Summary:
        //     Stop Out level is a lowest allowed Margin Level for account. If Margin Level
        //     is less than Stop Out, position will be closed sequentially until Margin Level
        //     is greater than Stop Out.
        double StopOutLevel { get; }

        //
        // Summary:
        //     Gets the user ID.
        long UserId { get; }

        //
        // Summary:
        //     Gets the account deposit asset/currency
        Asset Asset { get; }

        //
        // Summary:
        //     Type of total margin requirements per Symbol.
        //TotalMarginCalculationType TotalMarginCalculationType { get; }

        //
        // Summary:
        //     Gets the credit of the current account.
        //double Credit { get; }

        //[Obsolete("Use Account.Asset.Name instead")]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //string Currency { get; }

        //[Obsolete("Use PreciseLeverage instead")]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //int Leverage { get; }

        //[Obsolete("Use Positions instead")]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //IReadonlyList<Position> Positions { get; }

        //[Obsolete("Use PendingOrders instead")]
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //IReadonlyList<PendingOrder> PendingOrders { get; }
    }
}
