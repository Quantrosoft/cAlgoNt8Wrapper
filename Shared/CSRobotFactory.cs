using cAlgo.API;

namespace RobotLib.Cs
{
    public class CSRobotFactory : IRobotFactory
    {
        public ILogger CreateLogger(Robot robot)
        {
            return new CsLogger(robot);
        }

        public IRobot CreateRobot()
        {
            return new CsRobot();
        }
    }
}
