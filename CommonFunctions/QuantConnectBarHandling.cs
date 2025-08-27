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

namespace TdsCommons
{
    public class QcBarHandling
    {
        private ZipArchive mZipArchive;
        private StreamReader mStreamReader;
        private string mFolderPath;
        private string mInstrumentName;
        private int mBarPeriodSeconds;
        private DateTime mDay;
        private QcBar mLastSecondBar;

        public QcBarHandling(string folderPath, string instrumentName, int barPeriodSeconds)
        {
            mFolderPath = folderPath;
            mInstrumentName = instrumentName;
            mBarPeriodSeconds = barPeriodSeconds;
        }

        public string OpenFile(DateTime day)
        {
            mDay = day.Date;
            string dateStr = day.ToString("yyyyMMdd");

            var fileName = $"{dateStr}_quote.zip";
            var zipFilePath = Path.Combine(mFolderPath, fileName);
            var csvFileName = $"{dateStr}_{mInstrumentName}_second_quote.csv";

            if (!Directory.Exists(mFolderPath))
                return mFolderPath + " not found";

            if (!File.Exists(zipFilePath))
                return fileName + " not found in " + mFolderPath;

            mZipArchive = ZipFile.OpenRead(zipFilePath);
            var csvEntry = mZipArchive.GetEntry(csvFileName);

            if (csvEntry == null)
                throw new FileNotFoundException("CSV file not found inside ZIP archive.", csvFileName);

            mStreamReader = new StreamReader(csvEntry.Open());
            return "";
        }

        public string GetQcBar(DateTime from, DateTime to, out QcBar qcBar)
        {
            qcBar = new QcBar();
            string error = "";
            while (true)
            {
                if (0 != qcBar.BidOpen          // not first loop
                    || null == mLastSecondBar   // No last bar yet
                    || !(from.ToNativeSec() <= mLastSecondBar.TimeOpen.ToNativeSec() // mSecondBar bar is not in from-to range 
                            && mLastSecondBar.TimeOpen.ToNativeSec() < to.ToNativeSec())
                   )
                {
                    error = ReadNextBar(from, out mLastSecondBar);
                    if ("" != error)
                    {
                        return error;
                    }
                }

                if (null != mLastSecondBar)
                {
                    var fromNative = mLastSecondBar.TimeOpen.ToNativeSec();
                    if (fromNative >= to.ToNativeSec())
                        return ""; // no more data in range

                    // fast forward to from time
                    if (fromNative < from.ToNativeSec())
                        continue;
                }

                if (0 == qcBar.BidOpen)
                {
                    // init open stuff
                    long periodTicks = mBarPeriodSeconds * TimeSpan.TicksPerSecond;
                    qcBar.TimeOpen = new DateTime(mLastSecondBar.TimeOpen.Ticks - mLastSecondBar.TimeOpen.Ticks % periodTicks);

                    qcBar.BidOpen = mLastSecondBar.BidOpen;
                    qcBar.AskOpen = mLastSecondBar.AskOpen;

                    qcBar.BidHigh = mLastSecondBar.BidHigh;
                    qcBar.AskHigh = mLastSecondBar.AskHigh;
                    qcBar.BidLow = mLastSecondBar.BidLow;
                    qcBar.AskLow = mLastSecondBar.AskLow;
                }

                qcBar.BidHigh = Math.Max(qcBar.BidHigh, mLastSecondBar.BidHigh);
                qcBar.BidLow = Math.Min(qcBar.BidLow, mLastSecondBar.BidLow);
                qcBar.AskHigh = Math.Max(qcBar.AskHigh, mLastSecondBar.AskHigh);
                qcBar.AskLow = Math.Min(qcBar.AskLow, mLastSecondBar.AskLow);

                qcBar.BidClose = mLastSecondBar.BidClose;
                qcBar.AskClose = mLastSecondBar.AskClose;

                qcBar.BidVolume += mLastSecondBar.BidVolume;
                qcBar.AskVolume += mLastSecondBar.AskVolume;
            }
        }

        private string ReadNextBar(DateTime time, out QcBar qcBar)
        {
            qcBar = null;
            if (null == mStreamReader)
            {
                var error = OpenFile(time);
                if ("" != error)
                    return error;
            }

            if (mStreamReader.EndOfStream)
            {
                mZipArchive?.Dispose();
                mStreamReader?.Close();
                mStreamReader = null;
                return "No more data";
            }

            string line = mStreamReader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                return "Empty line";

            string[] parts = line.Split(',');
            if (parts.Length != 11)
                throw new FormatException("Invalid CSV format. Expected 11 columns.");

            long millisecondsSinceStartOfDay = long.Parse(parts[0], CultureInfo.InvariantCulture);
            DateTime timeOpen = mDay.Date.AddMilliseconds(millisecondsSinceStartOfDay);

            qcBar = new QcBar
            {
                TimeOpen = timeOpen,
                BidOpen = double.Parse(parts[1], CultureInfo.InvariantCulture),
                BidHigh = double.Parse(parts[2], CultureInfo.InvariantCulture),
                BidLow = double.Parse(parts[3], CultureInfo.InvariantCulture),
                BidClose = double.Parse(parts[4], CultureInfo.InvariantCulture),
                BidVolume = int.Parse(parts[5], CultureInfo.InvariantCulture),
                AskOpen = double.Parse(parts[6], CultureInfo.InvariantCulture),
                AskHigh = double.Parse(parts[7], CultureInfo.InvariantCulture),
                AskLow = double.Parse(parts[8], CultureInfo.InvariantCulture),
                AskClose = double.Parse(parts[9], CultureInfo.InvariantCulture),
                AskVolume = int.Parse(parts[10], CultureInfo.InvariantCulture),
            };

            return "";
        }

        public void Close()
        {
            if (mStreamReader != null)
                mStreamReader?.Close();
            mZipArchive?.Dispose();
        }
    }
}