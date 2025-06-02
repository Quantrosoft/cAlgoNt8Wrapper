using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaTrader.NinjaScript.Indicators
{
    public class Result
    {
        private Indicator mParent;

        public Result(Indicator parent)
        {
            mParent = parent;
        }

        public double LastValue => mParent.Value[0];
        public double Last(int index) => mParent.Value[index];
    }

}
