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
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.Strategies;
using System;

namespace cAlgo.API
{
    public class Symbol
    {
        private Robot mRobot;
        private StrategyRenderBase mStrategy;
        private Instrument mNinjaInstrument;
        public int Digits;
        public int SymbolBarIndex;

        public Symbol(Robot robot, Instrument ninjaInstrument)
        {
            mStrategy = mRobot = robot;
            mNinjaInstrument = ninjaInstrument;
            var tickSize = mRobot.Instrument.MasterInstrument.TickSize;
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
                else
                    return mRobot.Opens[1][0];
            }
        }

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

        public double TickSize => mRobot.Instrument.MasterInstrument.TickSize;

        public double TickValue => mRobot.Instrument.MasterInstrument.TickSize
            * mRobot.Instrument.MasterInstrument.PointValue;

        public double LotSize
        {
            get
            {
                return mRobot.Instrument.MasterInstrument.InstrumentType == InstrumentType.Forex
                    ? 100000
                    : 1.0 / 25;
            }
        }

        public string Name => mRobot.Instrument.FullName;

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
            if (mRobot.Instrument.MasterInstrument.InstrumentType == InstrumentType.Forex)
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
            if (mRobot.Instrument.MasterInstrument.InstrumentType == InstrumentType.Forex)
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

            switch (mRobot.Instrument.MasterInstrument.InstrumentType)
            {
                case InstrumentType.Forex:
                double lotSize = 100000; // 1 standard lot = 100,000 units
                normalizedVolume = Math.Round(volume / lotSize, 2); // Convert units to nearest 0.01 lot
                break;

                case InstrumentType.CryptoCurrency:
                double contractSize = mRobot.Instrument.MasterInstrument.PointValue; // Contract size for crypto
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
