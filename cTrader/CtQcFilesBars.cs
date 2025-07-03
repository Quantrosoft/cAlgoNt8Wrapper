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

using RobotLib;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using TdsCommons;

namespace cAlgo.API
{
    public class CtQcFilesBars : IQcBars
    {
        #region Members
        public const int QcBarsSize = 1000;
        public IQcTimeSeries OpenTimes { get; internal set; }
        public IQcDataSeries BidOpenPrices { get; internal set; }
        public IQcDataSeries BidHighPrices { get; internal set; }
        public IQcDataSeries BidLowPrices { get; internal set; }
        public IQcDataSeries BidClosePrices { get; internal set; }
        public IQcDataSeries BidVolumes { get; internal set; }
        public IQcDataSeries AskOpenPrices { get; internal set; }
        public IQcDataSeries AskHighPrices { get; internal set; }
        public IQcDataSeries AskLowPrices { get; internal set; }
        public IQcDataSeries AskClosePrices { get; internal set; }
        public IQcDataSeries AskVolumes { get; internal set; }
        public int TimeFrameSeconds { get; internal set; }
        public TimeFrame TimeFrame { get; internal set; }
        public int Count => OpenTimes.Count;
        public string SymbolName => mSymbolPair;
        public bool IsNewBar => CoFu.IsNewBar(TimeFrameSeconds, mFromTime, mPrevTime);

        private ZipArchive mZipArchive;
        private StreamReader mStreamReader;
        private string mHistoricalDataPath;
        private string mSymbolPair;
        private DateTime mDay;
        private QcBar mSecondBar;
        private bool mIsNewDay = true;
        private DateTime mSecondBarPrevTime;
        private DateTime mFromTime;
        private DateTime mPrevTime;
        #endregion

        public CtQcFilesBars(string folderPath,
            string symbolPair,
            int barPeriodSeconds,
            DateTime from)
        {
            mHistoricalDataPath = Environment.ExpandEnvironmentVariables(folderPath);
            mSymbolPair = symbolPair;
            TimeFrameSeconds = barPeriodSeconds;
            TimeFrame = AbstractRobot.Secs2Tf(barPeriodSeconds, out _);

            OpenTimes = new CtQcFilesTimeSeries();
            BidOpenPrices = new CtQcFilesDataSeries();
            BidHighPrices = new CtQcFilesDataSeries();
            BidLowPrices = new CtQcFilesDataSeries();
            BidClosePrices = new CtQcFilesDataSeries();
            BidVolumes = new CtQcFilesDataSeries();
            AskOpenPrices = new CtQcFilesDataSeries();
            AskHighPrices = new CtQcFilesDataSeries();
            AskLowPrices = new CtQcFilesDataSeries();
            AskClosePrices = new CtQcFilesDataSeries();
            AskVolumes = new CtQcFilesDataSeries();

            OnTick(from, from);
        }

        public void OnTick(DateTime fromTime, DateTime prevTime)
        {
            mFromTime = fromTime;
            mPrevTime = prevTime;

            var fromNative = fromTime.ToNativeSec();
            if (prevTime <= CoFu.TimeInvalid || mIsNewDay || CoFu.IsNewBar(1, fromTime, prevTime))
                do
                {
                    if (mIsNewDay || fromNative >= mSecondBar.TimeOpen.ToNativeSec())
                    {
                        if (!mIsNewDay)
                        {
                            BidHighPrices.Swap(Math.Max(BidHighPrices.LastValue, mSecondBar.BidHigh));
                            BidLowPrices.Swap(Math.Min(BidLowPrices.LastValue, mSecondBar.BidLow));
                            AskHighPrices.Swap(Math.Max(AskHighPrices.LastValue, mSecondBar.AskHigh));
                            AskLowPrices.Swap(Math.Min(AskLowPrices.LastValue, mSecondBar.AskLow));

                            BidClosePrices.Swap(mSecondBar.BidClose);
                            AskClosePrices.Swap(mSecondBar.AskClose);

                            BidVolumes.Swap(BidVolumes.LastValue + mSecondBar.BidVolume);
                            AskVolumes.Swap(AskVolumes.LastValue + mSecondBar.AskVolume);

                            mSecondBarPrevTime = mSecondBar.TimeOpen;
                        }

                        ReadNextSecondBar(fromTime);
                        if (mSecondBar.TimeOpen >= CoFu.TimeInvalid)    // Invalid TimeOpen means end of file
                        {
                            if (mIsNewDay || CoFu.IsNewBar(TimeFrameSeconds, mSecondBar.TimeOpen, mSecondBarPrevTime))
                                InitOpenBar();
                            mIsNewDay = false;
                        }
                    }
                } while (mSecondBar.TimeOpen >= CoFu.TimeInvalid && mSecondBar.TimeOpen.ToNativeSec() < fromNative);
        }

        public void OnStop()
        {
            Close();
        }

        private void InitOpenBar()
        {
            // init open stuff
            long periodTicks = TimeFrameSeconds * TimeSpan.TicksPerSecond;
            OpenTimes.Add(new DateTime(mSecondBar.TimeOpen.Ticks
                - mSecondBar.TimeOpen.Ticks % periodTicks));

            BidOpenPrices.Add(mSecondBar.BidOpen);
            AskOpenPrices.Add(mSecondBar.AskOpen);

            BidHighPrices.Add(mSecondBar.BidHigh);
            AskHighPrices.Add(mSecondBar.AskHigh);
            BidLowPrices.Add(mSecondBar.BidLow);
            AskLowPrices.Add(mSecondBar.AskLow);

            BidClosePrices.Add(mSecondBar.BidClose);
            AskClosePrices.Add(mSecondBar.AskClose);

            BidVolumes.Add(0);
            AskVolumes.Add(0);
        }

        private void ReadNextSecondBar(DateTime time)
        {
            mSecondBar = new QcBar();
            if (null == mStreamReader)
                Open(time);

            if (mStreamReader.EndOfStream)
            {
                Close();
                mIsNewDay = true;
                return; // end of file
            }

            string line = mStreamReader.ReadLine();
            string[] parts = line.Split(',');
            if (parts.Length != 11)
                throw new FormatException("Invalid CSV format. Expected 11 columns.");

            long millisecondsSinceStartOfDay = long.Parse(parts[0], CultureInfo.InvariantCulture);
            DateTime timeOpen = mDay.Date.AddMilliseconds(millisecondsSinceStartOfDay);

            mSecondBar.TimeOpen = timeOpen;
            mSecondBar.BidOpen = double.Parse(parts[1], CultureInfo.InvariantCulture);
            mSecondBar.BidHigh = double.Parse(parts[2], CultureInfo.InvariantCulture);
            mSecondBar.BidLow = double.Parse(parts[3], CultureInfo.InvariantCulture);
            mSecondBar.BidClose = double.Parse(parts[4], CultureInfo.InvariantCulture);
            mSecondBar.BidVolume = int.Parse(parts[5], CultureInfo.InvariantCulture);
            mSecondBar.AskOpen = double.Parse(parts[6], CultureInfo.InvariantCulture);
            mSecondBar.AskHigh = double.Parse(parts[7], CultureInfo.InvariantCulture);
            mSecondBar.AskLow = double.Parse(parts[8], CultureInfo.InvariantCulture);
            mSecondBar.AskClose = double.Parse(parts[9], CultureInfo.InvariantCulture);
            mSecondBar.AskVolume = int.Parse(parts[10], CultureInfo.InvariantCulture);
        }

        private void Open(DateTime day)
        {
            mDay = day.Date;
            string dateStr = day.ToString("yyyyMMdd");

            var folderPath = Path.Combine(mHistoricalDataPath, mSymbolPair.Split('=')[0],
                "QuantConnect Seconds");

            var fileName = $"{dateStr}_quote.zip";
            var zipFilePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
                throw new FileNotFoundException(folderPath + " not found");

            if (!File.Exists(zipFilePath))
                throw new FileNotFoundException(fileName + " not found in " + mHistoricalDataPath
                    + ". Choose a later start date");

            mZipArchive = ZipFile.OpenRead(zipFilePath);
            var csvEntry = mZipArchive.Entries.FirstOrDefault();

            mStreamReader = new StreamReader(csvEntry.Open());
        }

        private void Close()
        {
            if (mStreamReader != null)
            {
                mStreamReader?.Close();
                mZipArchive?.Dispose();
                mStreamReader = null;
                mZipArchive = null;
            }
        }
    }
}