using TdsCommons;

namespace RobotLib
{
    public interface ILogger
    {
        bool IsOpen { get; }

        LogFlags Mode { get; set; }

        string LogOpen(string pathName, string filename, bool append, LogFlags mode);

        string MakeLogPath();

        void AddText(string text);

        void Flush();

        void Close(string preText);
    }
}
