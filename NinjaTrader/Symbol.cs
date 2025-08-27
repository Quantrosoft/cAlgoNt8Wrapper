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
using System;

namespace NinjaTrader.NinjaScript.Strategies
{
    public class Symbol
    {
        private Strategy mRobot;
        private Instrument mNinjaInstrument;
        public int Digits;
        public int SymbolBarIndex;

        public Symbol(Strategy robot, Instrument ninjaInstrument)
        {
            mRobot = robot;
            mNinjaInstrument = ninjaInstrument;
            var tickSize = ninjaInstrument.MasterInstrument.TickSize;
            // Calc Digits also for not 0.xxx1 instruments like NQ with 0.25 tick size
            while (0 != (int)tickSize - tickSize)
            {
                tickSize *= 10;
                Digits++;
            }
        }

        public double Bid
        {
            get
            {
                if (mRobot.IsTickReplay)
                    return mRobot.MarketDataEventArgs.Bid;
                else
                    return mRobot.Opens[0][0];
            }
        }

        public double Ask
        {
            get
            {
                if (mRobot.IsTickReplay)
                    return mRobot.MarketDataEventArgs.Ask;
                //return mRobot.GetCurrentAsk();
                else
                    return mRobot.Opens[1][0];
            }
        }

        public double Spread => Ask - Bid;

        public long MarketDataVolume
        {
            get
            {
                return mRobot.MarketDataEventArgs.Volume;
            }
        }

        public double MarketDataPrice
        {
            get
            {
                return mRobot.MarketDataEventArgs.Price;
            }
        }

        public double MarketDataLast
        {
            get
            {
                return mRobot.MarketDataEventArgs.Last;
            }
        }

        public double TickSize => mNinjaInstrument.MasterInstrument.TickSize;

        public double TickValue => mNinjaInstrument.MasterInstrument.PointValue
            * mNinjaInstrument.MasterInstrument.TickSize;

        public double PipSize => mNinjaInstrument.MasterInstrument.TickSize;

        public double PipValue => mNinjaInstrument.MasterInstrument.TickSize
            * mNinjaInstrument.MasterInstrument.PointValue;

        public double LotSize
        {
            get
            {
                return 1;
            }
        }

        public string Name => mNinjaInstrument.FullName;

        public double VolumeInUnitsMin => 0.01;

        public double VolumeInUnitsMax => 100;

        public double VolumeInUnitsStep => VolumeInUnitsMin;

        //     Convert Volume in units of base currency to Quantity (in lots).
        //
        // Parameters:
        //   volume:
        //     The symbol volume units to convert to Quantity (in lots)
        public double VolumeInUnitsToQuantity(double volume)
        {
            // Forex: Convert volume (units) to lots (1 lot = 100,000 units)
            if (mNinjaInstrument.MasterInstrument.InstrumentType == InstrumentType.Forex)
            {
                double lotSize = 100000; // Standard lot size for Forex
                return volume / lotSize; // Convert units to lots
            }

            // Futures & Stocks: Quantity is the same as volume (1 contract = 1 unit)
            return volume;
        }

        //     Convert Quantity (in lots) to Volume in units of base currency.
        //
        // Parameters:
        //   quantity:
        //     Quantity (lots)
        //
        // Returns:
        //     Volume in units of base currency
        public double QuantityToVolumeInUnits(double quantity)
        {
            // Forex: Convert lots to volume (units)
            if (mNinjaInstrument.MasterInstrument.InstrumentType == InstrumentType.Forex)
            {
                double lotSize = 100000; // Standard lot size for Forex
                return quantity * lotSize; // Convert lots to units
            }
            // Futures & Stocks: Quantity is the same as volume (1 contract = 1 unit)
            return quantity;
        }

        public double NormalizeVolumeInUnits(double volume)
        {
            double normalizedVolume = volume;

            switch (mNinjaInstrument.MasterInstrument.InstrumentType)
            {
                case InstrumentType.Forex:
                double lotSize = 100000; // 1 standard lot = 100,000 units
                normalizedVolume = Math.Round(volume / lotSize, 2); // Convert units to nearest 0.01 lot
                break;

                case InstrumentType.CryptoCurrency:
                double contractSize = mNinjaInstrument.MasterInstrument.PointValue; // Contract size for crypto
                normalizedVolume = Math.Round(volume / contractSize, 8); // Round to 8 decimals (crypto standard)
                break;

                case InstrumentType.Future:
                case InstrumentType.Stock:
                default:
                normalizedVolume = Math.Round(volume); // Round to whole numbers (contracts/shares)
                break;
            }

            return normalizedVolume;
        }
    }
}
