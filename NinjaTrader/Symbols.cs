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
using System.Collections.Generic;
using System.Linq;

namespace NinjaTrader.NinjaScript.Strategies.Internals
{
    //     Represents the list of all the symbols with the symbol names as string values.
    public class Symbols
    {
        public Dictionary<string, Symbol> SymbolDictionary = new Dictionary<string, Symbol>();
        private Strategy mRobot;

        public Symbols(Strategy robot)
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
