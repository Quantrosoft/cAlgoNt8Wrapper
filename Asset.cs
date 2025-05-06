//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using NinjaTrader.Gui.NinjaScript;

namespace cAlgo.API
{
    //     The asset represents a currency.
    public class Asset
    {
        private StrategyRenderBase mStrategy;

        public Asset(StrategyRenderBase strategy)
        {
            mStrategy = strategy;
        }

        //
        // Summary:
        //     The asset Name
        public string Name => mStrategy.Account.Denomination.ToString();
        //Account.Get(AccountItem.NetLiquidationByCurrency).ToString();

        //
        // Summary:
        //     Number of asset digits
        public int Digits => 2;

        //
        // Summary:
        //     Converts value to another asset.
        //
        // Parameters:
        //   to:
        //     Target asset
        //
        //   value:
        //     The value you want to convert from current Asset
        //
        // Returns:
        //     ProfitModeValue in to / target asset
        //public double Convert(Asset to, double value);

        //
        // Summary:
        //     Converts value to another asset.
        //
        // Parameters:
        //   to:
        //     Target asset
        //
        //   value:
        //     The value you want to convert from current Asset
        //
        // Returns:
        //     ProfitModeValue in to / target asset
        //public double Convert(string to, double value);

        /// <summary>
        /// Returns a string representation of the asset
        /// </summary>
        public override string ToString() => Name;
    }
}
