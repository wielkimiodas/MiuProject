using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using RaceGamePhone.MiuWebService;
using System.ServiceModel;

namespace RaceGamePhone
{
    public class ConnectionManager
    {
        private string address;// = "http://192.168.0.15:8001/test";
        public bool IsStarted { get; set; }
        SteeringReceiverClient client;
        public MainPage mainPage;

        public ConnectionManager(MainPage mainPage)
        {
            IsStarted = false;
            this.mainPage = mainPage;
        }

        public bool Connect(string ip)
        {
            if (client == null)
            {                
                address = "http://" + ip + ":8001/MiuWebService";
                try
                {
                    client = new SteeringReceiverClient(new BasicHttpBinding(), new EndpointAddress(address));
                    IsStarted = true;
                }
                catch (Exception)
                {
                    IsStarted= false;
                    return false;
                }

                client.DoSteerCompleted += new EventHandler<DoSteerCompletedEventArgs>(client_DoSteerCompleted);                
            }
            return true;
        }

        public void SendContent(InputState input)
        {
            client.DoSteerAsync(input);
        }

        void client_DoSteerCompleted(object sender, DoSteerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //Debug.WriteLine("The answer is {0}", e.Result);
            }
        }
    }
}
