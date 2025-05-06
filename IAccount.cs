//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cAlgo.API
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
        //Asset Asset { get; }

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
