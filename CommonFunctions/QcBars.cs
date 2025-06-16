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
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;

// quantConnect bar class:
// Time, OHLCV for Bid and Ask

namespace TdsCommons
{
    public class QcBars
    {
        #region Members
        internal const int QcBarsSize = 1000;
        private ZipArchive mZipArchive;
        private StreamReader mStreamReader;
        private string mHistoricalDataPath;
        private string mSymbolPair;
        private int mBarPeriodSeconds;
        private DateTime mDay;
        private QcBar mSecondBar;
        private bool mIsNewDay = true;

        public QcTimeSeries OpenTimes;
        public QcDataSeries BidOpenPrices;
        public QcDataSeries BidHighPrices;
        public QcDataSeries BidLowPrices;
        public QcDataSeries BidClosePrices;
        public QcDataSeries BidVolumes;
        public QcDataSeries AskOpenPrices;
        public QcDataSeries AskHighPrices;
        public QcDataSeries AskLowPrices;
        public QcDataSeries AskClosePrices;
        public QcDataSeries AskVolumes;
        #endregion

        public QcBars(string folderPath, string symbolPair, int barPeriodSeconds)
        {
            mHistoricalDataPath = Environment.ExpandEnvironmentVariables(folderPath);
            mSymbolPair = symbolPair;
            mBarPeriodSeconds = barPeriodSeconds;

            OpenTimes = new QcTimeSeries();
            BidOpenPrices = new QcDataSeries();
            BidHighPrices = new QcDataSeries();
            BidLowPrices = new QcDataSeries();
            BidClosePrices = new QcDataSeries();
            BidVolumes = new QcDataSeries();
            AskOpenPrices = new QcDataSeries();
            AskHighPrices = new QcDataSeries();
            AskLowPrices = new QcDataSeries();
            AskClosePrices = new QcDataSeries();
            AskVolumes = new QcDataSeries();

            // Add an empty slot to start with
            // Prices and volumes must inized with 0
            OpenTimes.Add(CoFu.TimeInvalid);
            BidOpenPrices.Add(0);
            BidHighPrices.Add(0);
            BidLowPrices.Add(0);
            BidClosePrices.Add(0);
            BidVolumes.Add(0);
            AskOpenPrices.Add(0);
            AskHighPrices.Add(0);
            AskLowPrices.Add(0);
            AskClosePrices.Add(0);
            AskVolumes.Add(0);
        }

        public void OnTick(DateTime from, DateTime prevTime)
        {
            var fromNative = from.ToNativeSec();

            if (null == mStreamReader)
                ReadNextSecondBar(from);

            do
            {
                if (mIsNewDay || CoFu.IsNewBar(mBarPeriodSeconds, from, prevTime))
                {
                    // init open stuff
                    long periodTicks = mBarPeriodSeconds * TimeSpan.TicksPerSecond;
                    OpenTimes.Add(new DateTime(mSecondBar.TimeOpen.Ticks
                        - mSecondBar.TimeOpen.Ticks % periodTicks));

                    BidOpenPrices.Add(mSecondBar.BidOpen);
                    AskOpenPrices.Add(mSecondBar.AskOpen);

                    BidHighPrices.Add(mSecondBar.BidHigh);
                    AskHighPrices.Add(mSecondBar.AskHigh);
                    BidLowPrices.Add(mSecondBar.BidLow);
                    AskLowPrices.Add(mSecondBar.AskLow);

                    mIsNewDay = false;
                }

                BidHighPrices.Swap(Math.Max(BidHighPrices.LastValue, mSecondBar.BidHigh));
                BidLowPrices.Swap(Math.Min(BidLowPrices.LastValue, mSecondBar.BidLow));
                AskHighPrices.Swap(Math.Max(AskHighPrices.LastValue, mSecondBar.AskHigh));
                AskLowPrices.Swap(Math.Min(AskLowPrices.LastValue, mSecondBar.AskLow));

                BidClosePrices.Swap(mSecondBar.BidClose);
                AskClosePrices.Swap(mSecondBar.AskClose);

                BidVolumes.Swap(BidVolumes.LastValue + mSecondBar.BidVolume);
                AskVolumes.Swap(AskVolumes.LastValue + mSecondBar.AskVolume);

                ReadNextSecondBar(from);    // get second bar for next loop
                if (null == mStreamReader)
                    OnTick(from, prevTime);

            } while (mSecondBar.TimeOpen.ToNativeSec() < fromNative);
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

            var folderPath = Path.Combine(mHistoricalDataPath, mSymbolPair.Split('=')[0], "QuantConnect Seconds");

            var fileName = $"{dateStr}_quote.zip";
            var zipFilePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
                throw new FileNotFoundException(folderPath + " not found");

            if (!File.Exists(zipFilePath))
                throw new FileNotFoundException(fileName + " not found in " + mHistoricalDataPath);

            mZipArchive = ZipFile.OpenRead(zipFilePath);
            var csvEntry = mZipArchive.Entries.FirstOrDefault();

            if (csvEntry == null)
                throw new FileNotFoundException("CSV file not found inside ZIP archive.");

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