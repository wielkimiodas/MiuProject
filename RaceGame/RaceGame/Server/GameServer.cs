using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace RaceGame.Server
{
    class GameServer
    {
        private string address = "http://localhost:8001/MiuWebService";
        private ServiceHost HostProxy;


        public string LaunchServerInstance()
        {
            HostProxy = new ServiceHost(typeof(SteeringReceiver),new Uri(address));
            string response = "not even initialized";
            // Enable metadata publishing.
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            HostProxy.Description.Behaviors.Add(smb);

            // Open the ServiceHost to start listening for messages. Since
            // no endpoints are explicitly configured, the runtime will create
            // one endpoint per base address for each service contract implemented
            // by the service.
            try
            {
                HostProxy.Open();
                response="The service is ready at " + address;
            }
            catch (AddressAccessDeniedException)
            {
                response="You need to reserve the address for this service";
                HostProxy = null;
            }
            catch (AddressAlreadyInUseException)
            {
                response="Something else is already using this address";
                HostProxy = null;
            }
            catch (Exception ex)
            {
                response="Something bad happened on startup: " + ex.Message;
                HostProxy = null;
            }

            return response;
        }
    }
}
