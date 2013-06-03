using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaceGame.Server
{
    public interface ISteeringReceiver
    {
        //[OperationalContract]
        bool DoSteer(InputState state);
    }
}
