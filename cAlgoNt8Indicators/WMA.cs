using NinjaTrader.Cbi;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.Data;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;

// This namespace holds indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
    /// <summary>
    /// The WMAct (Weighted Moving Average) is a Moving Average indicator that shows the average
    /// value of a security's price over a period of time with special emphasis on the more recent
    /// portions of the time period under analysis as opposed to the earlier.
    /// </summary>
    public class WMAct : Indicator
    {
        public WMAct()
        {
            Result = new Result(this);
        }

        private int myPeriod;
        private double priorSum;
        private double priorWsum;
        private double sum;
        private double wsum;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {

                Description = "cAlgoNt8Wma";
                Name = "cAlgoNt8Wma";
                IsOverlay = true;
                IsSuspendedWhileInactive = true;
                Period = 14;

                AddPlot(Brushes.Goldenrod, "cAlgoNt8Wma");
            }
            else if (State == State.Configure)
            {
                priorSum = 0;
                priorWsum = 0;
                sum = 0;
                wsum = 0;
            }
        }

        protected override void OnBarUpdate()
        {
            if (BarsArray[0].BarsType.IsRemoveLastBarSupported)
            {
                if (CurrentBar == 0)
                    Value[0] = Input[0];
                else
                {
                    int back = Math.Min(Period - 1, CurrentBar);
                    double val = 0;
                    int weight = 0;
                    for (int idx = back; idx >= 0; idx--)
                    {
                        val += (idx + 1) * Input[back - idx];
                        weight += (idx + 1);
                    }
                    Value[0] = val / weight;
                }
            }
            else
            {
                if (IsFirstTickOfBar)
                {
                    priorWsum = wsum;
                    priorSum = sum;
                    myPeriod = Math.Min(CurrentBar + 1, Period);
                }

                wsum = priorWsum - (CurrentBar >= Period ? priorSum : 0) + myPeriod * Input[0];
                sum = priorSum + Input[0] - (CurrentBar >= Period ? Input[Period] : 0);
                Value[0] = wsum / (0.5 * myPeriod * (myPeriod + 1));
            }
        }

        #region Properties
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(Name = "Period", GroupName = "NinjaScriptParameters", Order = 0)]
        public int Period { get; set; }

        public Result Result { get; private set; }
        #endregion
    }
}
