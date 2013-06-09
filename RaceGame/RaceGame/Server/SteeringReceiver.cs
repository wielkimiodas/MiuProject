using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RaceGame.Server
{
    class SteeringReceiver : ISteeringReceiver
    {
        public bool DoSteer(InputState state)
        {

            Console.WriteLine("accel: " + state.acceleration + "\tbreak: " + state.breakVal + "\tsteer: " + state.steer);            
            return true;
        }
    }
}
