using cAlgo.API;

namespace RobotLib
{
   public interface IRobotFactory 
   {
      IRobot CreateRobot();
      ILogger CreateLogger(Robot robot);
   }
}
