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
    public class CsLogger : ILogger
    {
        private StreamWriter mStreamWriter;
        private string mLogFile;
        private string mLine = "";
#if CTRADER
        cAlgo.API.
#endif
        Strategy mRobot;

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
            try
            {
                mStreamWriter = new StreamWriter(mLogFile, append);
            }
            catch (Exception)
            {
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
            if (IsOpen)
            {
                mStreamWriter.Close();

                if (preText.Length > 10)
                {
                    // Read the existing content of the file
                    string fileContent = File.ReadAllText(mLogFile);

                    fileContent = fileContent.Replace("sep=,\n", "");

                    // Concatenate the new text at the beginning with the original content
                    string updatedContent = preText + fileContent;

                    // Write the updated content back to the file
                    File.WriteAllText(mLogFile, updatedContent);
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
    }
}
