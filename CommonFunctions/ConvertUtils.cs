using System;
using System.Collections.Generic;
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
