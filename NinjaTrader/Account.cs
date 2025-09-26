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

using NinjaTrader.Cbi;
using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.NinjaScript.Strategies;
using System;

namespace cAlgoNt8Wrapper
{
    public class Account : IAccount
    {
        private StrategyRenderBase mStrategy;
        private Strategy mRobot;

        // Define the event to forward OrderUpdate
        public event Action<object, OrderEventArgs> OrderUpdate;

        public Account(Strategy strategy)
        {
            mStrategy = mRobot = strategy;
            Asset = new Asset(strategy);
            Balance = Equity = mStrategy.Account.Get(AccountItem.CashValue, mStrategy.Account.Denomination);

            // Subscribe to the NinjaTrader Account's OrderUpdate event
            if (mStrategy.Account != null)
            {
                mStrategy.Account.OrderUpdate += OnNinjaTraderOrderUpdate;
            }
        }

        // Handler for NinjaTrader's OrderUpdate event
        private void OnNinjaTraderOrderUpdate(object sender, OrderEventArgs e)
        {
            // Forward the event to subscribers of this class
            OrderUpdate?.Invoke(sender, e);
        }

        public Provider Provider => mStrategy.Account.Provider;

        //
        // Summary:
        //     Returns the balance of the current account.
        public double Balance { get; internal set; }

        //
        // Summary:
        //     Represents the equity of the current account (balance minus Unrealized Net Loss
        //     plus Unrealized Net Profit plus Bonus).
        public double Equity
        {
            get
            {
                // Since NinjaTrader does not provide a direct way to get the equity,
                // we calculate it by subtracting the gross profit of all open
                // positions from the current balance
                var profitSum = 0.0;
                foreach (var pos in mRobot.Positions)
                    profitSum += pos.GrossProfit;

                return Balance - profitSum;
            }
            internal set { }
        }

        //
        // Summary:
        //     Represents the margin of the current account.
        public double Margin
        {
            get
            {
                var BuyingPower = mStrategy.Account.Get(AccountItem.BuyingPower,
                    mStrategy.Account.Denomination);
                var CashValue = mStrategy.Account.Get(AccountItem.CashValue,
                    mStrategy.Account.Denomination);
                var Commission = mStrategy.Account.Get(AccountItem.Commission,
                    mStrategy.Account.Denomination);
                var ExcessIntradayMargin = mStrategy.Account.Get(AccountItem.ExcessIntradayMargin,
                    mStrategy.Account.Denomination);
                var ExcessInitialMargin = mStrategy.Account.Get(AccountItem.ExcessInitialMargin,
                    mStrategy.Account.Denomination);
                var ExcessMaintenanceMargin = mStrategy.Account.Get(AccountItem.ExcessMaintenanceMargin,
                    mStrategy.Account.Denomination);
                var ExcessPositionMargin = mStrategy.Account.Get(AccountItem.ExcessPositionMargin,
                    mStrategy.Account.Denomination);
                var Fee = mStrategy.Account.Get(AccountItem.Fee,
                    mStrategy.Account.Denomination);
                var GrossRealizedProfitLoss = mStrategy.Account.Get(AccountItem.GrossRealizedProfitLoss,
                    mStrategy.Account.Denomination);
                var InitialMargin = mStrategy.Account.Get(AccountItem.InitialMargin,
                    mStrategy.Account.Denomination);
                var IntradayMargin = mStrategy.Account.Get(AccountItem.IntradayMargin,
                    mStrategy.Account.Denomination);
                var LongOptionValue = mStrategy.Account.Get(AccountItem.LongOptionValue,
                    mStrategy.Account.Denomination);
                var LookAheadMaintenanceMargin = mStrategy.Account.Get(AccountItem.LookAheadMaintenanceMargin,
                    mStrategy.Account.Denomination);
                var LongStockValue = mStrategy.Account.Get(AccountItem.LongStockValue,
                    mStrategy.Account.Denomination);
                var MaintenanceMargin = mStrategy.Account.Get(AccountItem.MaintenanceMargin,
                    mStrategy.Account.Denomination);
                var NetLiquidation = mStrategy.Account.Get(AccountItem.NetLiquidation,
                    mStrategy.Account.Denomination);
                var NetLiquidationByCurrency = mStrategy.Account.Get(AccountItem.NetLiquidationByCurrency,
                    mStrategy.Account.Denomination);
                var PositionMargin = mStrategy.Account.Get(AccountItem.PositionMargin,
                    mStrategy.Account.Denomination);
                var RealizedProfitLoss = mStrategy.Account.Get(AccountItem.RealizedProfitLoss,
                    mStrategy.Account.Denomination);
                var ShortOptionValue = mStrategy.Account.Get(AccountItem.ShortOptionValue,
                    mStrategy.Account.Denomination);
                var ShortStockValue = mStrategy.Account.Get(AccountItem.ShortStockValue,
                    mStrategy.Account.Denomination);
                var SodCashValue = mStrategy.Account.Get(AccountItem.SodCashValue,
                    mStrategy.Account.Denomination);
                var SodLiquidatingValue = mStrategy.Account.Get(AccountItem.SodLiquidatingValue,
                    mStrategy.Account.Denomination);
                var UnrealizedProfitLoss = mStrategy.Account.Get(AccountItem.UnrealizedProfitLoss,
                    mStrategy.Account.Denomination);
                var TotalCashBalance = mStrategy.Account.Get(AccountItem.TotalCashBalance,
                    mStrategy.Account.Denomination);

                return mStrategy.Account.Get(AccountItem.PositionMargin,
                mStrategy.Account.Denomination);
            }
        }

        //
        // Summary:
        //     Represents the free margin of the current account.
        public double FreeMargin { get; }

        //
        // Summary:
        //     Represents the margin level of the current account. Margin Level (in %) is calculated
        //     using this formula: Equity / Margin * 100
        public double? MarginLevel { get; }

        //
        // Summary:
        //     Defines if the account is Live or Demo. True if the Account is Live, False if
        //     it is a Demo.
        public bool IsLive { get; }

        //
        // Summary:
        //     Returns the number of the current account, e.g. 123456.
        public int Number { get; }

        //
        // Summary:
        //     Returns the broker name of the current account.
        public string BrokerName => mStrategy.Account.Name;

        //
        // Summary:
        //     Gets the Unrealized Gross profit value.
        public double UnrealizedGrossProfit { get; }

        //
        // Summary:
        //     Gets the Unrealized Net profit value.
        public double UnrealizedNetProfit { get; }

        //
        // Summary:
        //     Gets the precise account leverage value.
        public double PreciseLeverage { get; }

        //
        // Summary:
        //     Stop Out level is a lowest allowed Margin Level for account. If Margin Level
        //     is less than Stop Out, position will be closed sequentially until Margin Level
        //     is greater than Stop Out.
        public double StopOutLevel { get; }

        //
        // Summary:
        //     Gets the user ID.
        public long UserId { get; }

        //
        // Summary:
        //     Gets the account deposit asset/currency
        public Asset Asset { get; }

        //
        // Summary:
        //     Type of total margin requirements per Symbol.
        //TotalMarginCalculationType TotalMarginCalculationType { get; }

        //
        // Summary:
        //     Gets the credit of the current account.
        public double Credit { get; }

    }
}

namespace cAlgoNt8Wrapper { }
