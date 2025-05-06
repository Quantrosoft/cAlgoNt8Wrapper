//
// Copyright (c) 2035 Quantrosoft Pty. Ltd.
// All rights reserved.
//

using cAlgo.API;

namespace RobotLib
{
   public interface IRobotFactory 
   {
      IRobot CreateRobot();
      ILogger CreateLogger(Robot robot);
   }
}
