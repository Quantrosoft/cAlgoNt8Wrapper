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

using System.Globalization;

namespace TdsCommons
{
   public class ConvertUtils
   {
      private static readonly CultureInfo sUSCulture = new CultureInfo("en-US");

      public static string DoubleToString(double value, int digits)
      {
         if (double.MaxValue == value || double.NaN == value) return "NaN";
         string format = "F" + digits.ToString();
         return value.ToString(format, sUSCulture);
      }

      public static string IntegerToString(int n)
      {
         return n.ToString();
      }

      public static double StringToDouble(string s)
      {
         try { return double.Parse(s, sUSCulture); }
         catch { return 0; }
      }

      public static int StringToInteger(string s)
      {
         try { return int.Parse(s); }
         catch { return 0; }
      }
   }
}
