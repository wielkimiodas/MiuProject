using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace RaceGame.Server
{
    [ServiceContract]
    interface ISteeringReceiver
    {
        [OperationContract]
        bool DoSteer(InputState state);
    }
}
