using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Devices.Sensors;
using RaceGamePhone.MiuWebService;
using System.ServiceModel;
using System.Diagnostics;

namespace RaceGamePhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        private string address = "http://192.168.0.15:8001/test";
        private bool isStared = false;
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            //InitAccelerometer();
        }

        Accelerometer accelSensor;
        SteeringReceiverClient client;
        InputState input;

        public void InitAccelerometer()
        {
            accelSensor = new Accelerometer();
            accelSensor.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(accelSensor_ReadingChanged);
            accelSensor.Start();
            accelSensor.TimeBetweenUpdates = new TimeSpan(0, 0, 0, 0, 45);
        }

        void accelSensor_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            
            if (isStared && client != null)
            {
                input = new InputState();
                input.acceleration = 1;
                input.breakVal = 0;
                input.steer = (float)e.Y;

                client.DoSteerAsync(input);
            }
            
        }




        private void btnBreak_MouseMove(object sender, MouseEventArgs e)
        {
            textBlock1.Text = "A=> x: " + e.GetPosition(btnBreak).X + Environment.NewLine + "y:" + e.GetPosition(btnBreak).Y;
        }

        private void btnAcceleration_MouseMove(object sender, MouseEventArgs e)
        {
            textBlock1.Text = "B=> x: " + e.GetPosition(btnAcceleration).X + Environment.NewLine + "y:" + e.GetPosition(btnAcceleration).Y;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (client == null)
            {
                string ip = tbAddress.Text;
                address = "http://" + ip + ":8001/MiuWebService";
                try
                {
                    
                    client = new SteeringReceiverClient(new BasicHttpBinding(), new EndpointAddress(address));
                    isStared = true;
                    tbStatus.Text = "Connected";
                    InitAccelerometer();

                }
                catch (Exception)
                {
                    isStared = false;
                    tbStatus.Text = "Connection could not be established";
                }

                client.DoSteerCompleted += new EventHandler<DoSteerCompletedEventArgs>(client_DoSteerCompleted);
            }
        }

        void client_DoSteerCompleted(object sender, DoSteerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Debug.WriteLine("The answer is {0}", e.Result);
            }
        }


    }
}