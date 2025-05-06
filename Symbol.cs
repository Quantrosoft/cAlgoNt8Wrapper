//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using NinjaTrader.Cbi;
using System;

namespace cAlgo.API
{
    public class Symbol
    {
        private Robot mRobot;
        private Instrument mNinjaInstrument;
        public int Digits;

        public Symbol(Robot robot, Instrument ninjaInstrument)
        {
            mRobot = robot;
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
                    return mRobot.mMarketDataEventArgs.Bid;
                else
                    return mRobot.Opens[0][0];
            }
        }

        public double Ask
        {
            get
            {
                if (mRobot.IsTickReplay)
                    return mRobot.mMarketDataEventArgs.Ask;
                else
                    return mRobot.Opens[1][0];
            }
        }

        public long MarketDataVolume
        {
            get
            {
                return mRobot.mMarketDataEventArgs.Volume;
            }
        }

        public double MarketDataPrice
        {
            get
            {
                return mRobot.mMarketDataEventArgs.Price;
            }
        }

        public double MarketDataLast
        {
            get
            {
                return mRobot.mMarketDataEventArgs.Last;
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

        public double VolumeInUnitsMax => 1000000000;

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
