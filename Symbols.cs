//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using NinjaTrader.Cbi;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.API.Internals
{
    //     Represents the list of all the symbols with the symbol names as string values.
    public class Symbols
    {
        public Dictionary<string, Symbol> SymbolDictionary = new Dictionary<string, Symbol>();
        private Robot mRobot;

        public Symbols(Robot robot)
        {
            mRobot = robot;
        }

        //
        // Summary:
        //     Gets the desired symbol.
        //
        // Parameters:
        //   index:
        string this[int index] => SymbolDictionary.ElementAt(index).Key;

        //
        // Summary:
        //     Gets the total number of the symbols in the list.
        public int Count => SymbolDictionary.Count;

        //
        // Summary:
        //     Gets the specific symbol.
        //
        // Parameters:
        //   symbolName:
        //     Name of the symbol you want to get
        public Symbol GetSymbol(string symbolName)
        {
            if (SymbolDictionary.ContainsKey(symbolName))
                return SymbolDictionary[symbolName];
            else
            {
                var retVal = new Symbol(mRobot, Instrument.GetInstrument(symbolName));
                SymbolDictionary.Add(symbolName, retVal);
                return retVal;
            }
        }

        //
        // Summary:
        //     Get multiple symbols at once.
        //
        // Parameters:
        //   symbolNames:
        //     Names of the symbols you want to get
        public Symbol[] GetSymbols(params string[] symbolNames)
            => symbolNames.Select(GetSymbol).ToArray();

        //
        // Summary:
        //     Defines if the specific symbol name exists in the list.
        //
        // Parameters:
        //   symbolName:
        //     Name of the symbol you want to check
        public bool Exists(string symbolName)
        {
            return SymbolDictionary.ContainsKey(symbolName);
        }
    }
}
