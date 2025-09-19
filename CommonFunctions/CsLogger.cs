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

using NinjaTrader.NinjaScript.Strategies;
using System;
using System.IO;
using TdsCommons;

namespace RobotLib.Cs
{
    public class CsLogger : ILogger, IDisposable
    {
        private StreamWriter mStreamWriter;
        private string mLogFile;
        private string mLine = "";
#if CTRADER
    cAlgo.API.
#endif
        Strategy mRobot;
        private bool disposed = false;

        public bool IsOpen
        {
            get { return mStreamWriter != null; }
        }

        public LogFlags Mode { get; set; }

        public bool WriteHeader { get; private set; }

        public CsLogger(
#if CTRADER
    cAlgo.API.
#endif
            Strategy robot)
        {
            mRobot = robot;
        }

        public string LogOpen(string pathName, string filename, bool append, LogFlags mode)
        {
            // Close any existing stream first
            if (IsOpen)
            {
                Close("");
            }

            Mode = mode;

            var dir = Path.Combine(Path.GetDirectoryName(pathName), Path.GetDirectoryName(filename));
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            mLogFile = Path.Combine(dir, Path.GetFileName(filename)
               + ("" == filename
                  ? Path.GetFileName(pathName)
                  : Path.GetExtension(pathName)));

            if (!append)
                mLogFile = MakeUniqueLogfileName(mLogFile);

            var retVal = File.Exists(mLogFile) ? mLogFile : "";

            // Add retry logic with delay
            int retryCount = 0;
            const int maxRetries = 5;
            const int delayMs = 100;

            while (retryCount < maxRetries)
            {
                try
                {
                    mStreamWriter = new StreamWriter(mLogFile, append);
                    break; // Success, exit retry loop
                }
                catch (IOException ex) when (ex.Message.Contains("being used by another process"))
                {
                    retryCount++;
                    if (retryCount >= maxRetries)
                    {
                        // Log the error or handle it appropriately
                        mRobot?.Print($"Log | Failed to open log file after {maxRetries} attempts: {ex.Message}");
                        return "";
                    }

                    // Wait before retrying
                    System.Threading.Thread.Sleep(delayMs * retryCount);
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    mRobot?.Print($"Log | Error opening log file: {ex.Message}");
                    return "";
                }
            }

            return retVal;
        }

        public string MakeLogPath()
        {
            string terminalCommondataPath = Environment.GetEnvironmentVariable("USERPROFILE")
               + @"\Documents\cAlgo\Sources\..\Logfiles";
#if TDS
        return terminalCommondataPath + "\\Tds.csv";
#else
            return terminalCommondataPath + "\\Algo.csv";
#endif
        }

        public void AddText(string text)
        {
            if (0 != (Mode & LogFlags.LogFile))
            {
                if (!IsOpen)
                    return;

                mStreamWriter.Write(text);
            }

            if (0 != (Mode & LogFlags.LogPrint))
            {
                mLine += text.Replace("sep=,\n", "");
                if (mLine.Contains("\n"))
                {
                    mLine = mLine.Replace("\r", "");
                    var splitLine = mLine.Split('\n');
                    var lastNdx = splitLine.Length - 1;
                    for (int i = 0; i < lastNdx; i++)
                    {
                        var printText = splitLine[i].Replace(": ,", ": ");
                        mRobot.Print("Log | " + printText);
                    }
                    mLine = splitLine[lastNdx];
                }
            }
        }

        public void Flush()
        {
            if (IsOpen)
            {
                mStreamWriter.Write("\n");
                mStreamWriter.Flush();
            }

            if (0 != (Mode & LogFlags.LogPrint))
            {
                mRobot.Print("Log | " + mLine);
                mLine = "";
            }
        }

        public void Close(string preText)
        {
            if (!IsOpen)
                return;

            try
            {
                // Flush and close the stream
                mStreamWriter.Flush();
                mStreamWriter.Close();
                mStreamWriter.Dispose();
            }
            catch (Exception ex)
            {
                mRobot?.Print($"Log | Error closing stream: {ex.Message}");
            }
            finally
            {
                mStreamWriter = null; // Important: nullify the reference
            }

            // Add delay before file operations to ensure handle is released
            if (preText.Length > 10)
            {
                System.Threading.Thread.Sleep(50); // Small delay

                try
                {
                    // Read the existing content of the file
                    string fileContent = File.ReadAllText(mLogFile);
                    fileContent = fileContent.Replace("sep=,\n", "");

                    // Concatenate the new text at the beginning with the original content
                    string updatedContent = preText + fileContent;

                    // Write the updated content back to the file
                    File.WriteAllText(mLogFile, updatedContent);
                }
                catch (Exception ex)
                {
                    mRobot?.Print($"Log | Error updating file with preText: {ex.Message}");
                }
            }
        }

        private string MakeUniqueLogfileName(string pathName)
        {
            while (File.Exists(pathName))
            {
                var fnEx = pathName.Split('.');

                int splitExSize = fnEx.Length;
                if (splitExSize < 2)
                    return "";

                string name = fnEx[0];
                for (int i = 1; i < splitExSize - 1; i++)
                    name += "." + fnEx[i];

                string ext = fnEx[splitExSize - 1];

                var fnNum = name.Split('_');
                pathName = (1 == fnNum.Length)
                    ? name + "_1." + ext
                    : fnNum[0] + "_" + ConvertUtils.IntegerToString(
                      ConvertUtils.StringToInteger(fnNum[1]) + 1) + "." + ext;
            }

            return pathName;
        }

        // IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (IsOpen)
                    {
                        Close("");
                    }
                }
                disposed = true;
            }
        }

        ~CsLogger()
        {
            Dispose(false);
        }
    }
}
