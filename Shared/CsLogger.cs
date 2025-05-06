using cAlgo.API;
using System;
using System.IO;
using TdsCommons;
using static System.Net.Mime.MediaTypeNames;

namespace RobotLib.Cs
{
    public class CsLogger : ILogger
    {
        private StreamWriter mStreamWriter;
        private string mLogFile;
        private string mLine = "";
        private Robot mRobot;

        public bool IsOpen
        {
            get { return mStreamWriter != null; }
        }
        public LogFlags Mode { get; set; }
        public bool WriteHeader { get; private set; }

        public CsLogger(Robot robot)
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
                    var splitLine = mLine.Split('\n');
                    var lastNdx = splitLine.Length - 1;
                    for (int i = 0; i < lastNdx; i++)
                        mRobot.Print("Log | " + splitLine[i]);

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
