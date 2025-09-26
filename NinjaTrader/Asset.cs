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

using NinjaTrader.Gui.NinjaScript;
using NinjaTrader.NinjaScript.Strategies;

namespace cAlgoNt8Wrapper
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
