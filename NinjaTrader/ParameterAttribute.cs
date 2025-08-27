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

using System;

namespace NinjaTrader.NinjaScript.Strategies
{
    //
    // Summary:
    //     The Parameter Attribute class.
    //
    // Remarks:
    //     Marks a property as input parameter.
    public class ParameterAttribute : Attribute
    {
        private readonly string _name;

        //
        // Summary:
        //     The input parameter name.
        public string Name => _name;

        //
        // Summary:
        //     Gets or sets the default value of this Parameter property.
        public object DefaultValue { get; set; }

        //
        // Summary:
        //     Gets or sets the minimum value of this Parameter property. It is used for validating
        //     user input.
        public object MinValue { get; set; }

        //
        // Summary:
        //     Gets or sets the maximum value of this Parameter property. It is used for validating
        //     user input.
        public object MaxValue { get; set; }

        //
        // Summary:
        //     Gets or sets the step of this Parameter. Step is used in NumericUpDown controls
        //     in parameter editors.
        public double Step { get; set; }

        //
        // Summary:
        //     Groups parameters in UI.
        public string Group { get; set; }

        //
        // Summary:
        //     Initializes a new ParameterAttribute instance and sets the name.
        //
        // Parameters:
        //   name:
        //     The name of the parameter.
        //
        // Remarks:
        //     Represents an input parameter to the program. To make it effective type in enclosed
        //     in square brackets, e.g. [Parameter], before the property declaration. Parameters
        //     are listed in the instance button of the robot/indicator.
        public ParameterAttribute(string name)
        {
            _name = name;
        }

        //
        // Summary:
        //     Initializes a new instance of the ParameterAttribute class.
        //
        // Remarks:
        //     In this case the parameter name is the same as the property name.
        public ParameterAttribute()
        {
        }
    }
}