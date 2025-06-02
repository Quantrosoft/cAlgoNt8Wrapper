//
// Copyright (C) 2024, NinjaTrader LLC <www.ninjatrader.com>.
// NinjaTrader reserves the right to modify or overwrite this NinjaScript component with each release.
//
#region Using declarations
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
#endregion

// This namespace holds indicators in this folder and is required. Do not change it.
namespace NinjaTrader.NinjaScript.Indicators
{
    /// <summary>
    /// The SMAct (Simple Moving Average) is an indicator that shows the average value of a security's price over a period of time.
    /// </summary>
    public class SMAct : Indicator
    {
        public SMAct()
        {
            Result = new Result(this);
        }

        private double priorSum;
        private double sum;

        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = "cAlgoNt8Sma";
                Name = "cAlgoNt8Sma";
                IsOverlay = true;
                IsSuspendedWhileInactive = true;
                Period = 14;

                AddPlot(Brushes.Goldenrod, "cAlgoNt8Sma");
            }
            else if (State == State.Configure)
            {
                priorSum = 0;
                sum = 0;
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
                    double last = Value[1] * Math.Min(CurrentBar, Period);

                    if (CurrentBar >= Period)
                        Value[0] = (last + Input[0] - Input[Period]) / Math.Min(CurrentBar, Period);
                    else
                        Value[0] = ((last + Input[0]) / (Math.Min(CurrentBar, Period) + 1));
                }
            }
            else
            {
                if (IsFirstTickOfBar)
                    priorSum = sum;

                sum = priorSum + Input[0] - (CurrentBar >= Period ? Input[Period] : 0);
                Value[0] = sum / (CurrentBar < Period ? CurrentBar + 1 : Period);
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
