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
        private InputState input = new InputState();
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Touch.FrameReported += new TouchFrameEventHandler(Touch_FrameReported);

            //InitAccelerometer();
        }

        private double Normalize(double ptX, double ptY, double size)
        {
            var r = size / 2;
            ptX = Math.Abs(ptX - r);
            ptY = Math.Abs(ptY - r);
            var d = Math.Sqrt(ptX * ptX + ptY * ptY);

            return d / r;
        }

        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            TouchPoint primaryTouchPoint = e.GetPrimaryTouchPoint(null);

            TouchPointCollection touchPoints = e.GetTouchPoints(null);
            
            foreach (TouchPoint tp in touchPoints)
            {
                if (tp.Action == TouchAction.Move)
                {
                    var t = TransformToVisual(ContentPanel);
                    var absposition = t.Transform(new Point(tp.Position.X, tp.Position.Y));

                    if (absposition.X > 0
                        && absposition.X < btnBreak.ActualHeight
                        && absposition.Y > 0
                        && absposition.Y < btnBreak.ActualWidth)
                    {
                        //btn BREAK
                        tbBrakeInfo.Text = "b-> x: " + absposition.X + " y:" + absposition.Y;
                        double val = Normalize(absposition.X, absposition.Y, btnBreak.ActualHeight);
                        tbBrakeInfo.Text += " " + val;

                        if (val > 1) btnBreak_MouseLeave(null, null);

                        input.breakVal = (float)val;
                    }
                    
                    if (absposition.X < btnAcceleration.ActualHeight
                        && absposition.X > 0
                        && absposition.Y < ContentPanel.ActualWidth
                        && absposition.Y > ContentPanel.ActualWidth - btnAcceleration.ActualWidth)
                    {
                        //btn ACCEL
                        var yp = absposition.Y - (ContentPanel.ActualWidth - btnAcceleration.ActualWidth);
                        tbAccelInfo.Text = "a-> x: " + absposition.X + " y:" + yp;

                        double val = Normalize(absposition.X, yp, btnAcceleration.ActualHeight);
                        tbAccelInfo.Text += " " + val;
                        if (val > 1) btnAcceleration_MouseLeave(null, null);
                        input.acceleration = (float)val;
                    }
                }

            }
        }

        Accelerometer accelSensor;
        SteeringReceiverClient client;        

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
                input.steer = -(float)e.Y;

                client.DoSteerAsync(input);
            }
            
        }


        private void btnBreak_MouseMove(object sender, MouseEventArgs e)
        {
            //tbBrakeInfo.Text = "A=> x: " + e.GetPosition(btnBreak).X + Environment.NewLine + "y:" + e.GetPosition(btnBreak).Y;
        }

        private void btnAcceleration_MouseMove(object sender, MouseEventArgs e)
        {
            //tbAccelInfo.Text = "B=> x: " + e.GetPosition(btnAcceleration).X + Environment.NewLine + "y:" + e.GetPosition(btnAcceleration).Y;
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {

            //NavigationService.Navigate(new Uri("/ConfigurationPage.xaml", UriKind.Relative));

            if (client == null)
            {
                string ip = "192.168.0.15";//tbAddress.Text;
                address = "http://" + ip + ":8001/MiuWebService";
                try
                {

                    client = new SteeringReceiverClient(new BasicHttpBinding(), new EndpointAddress(address));
                    isStared = true;
                    //tbStatus.Text = "Connected";
                    InitAccelerometer();

                }
                catch (Exception)
                {
                    isStared = false;
                    //tbStatus.Text = "Connection could not be established";
                }

                client.DoSteerCompleted += new EventHandler<DoSteerCompletedEventArgs>(client_DoSteerCompleted);
            }
        }

        void client_DoSteerCompleted(object sender, DoSteerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                //Debug.WriteLine("The answer is {0}", e.Result);
            }
        }

        private void btnAcceleration_MouseLeave(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("acc mouse leave");
            input.acceleration = 0;
            tbAccelInfo.Text = "zero";
        }

        private void btnBreak_MouseLeave(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("break mouse leave");
            input.breakVal= 0;
            tbBrakeInfo.Text = "zero";
        }

    }
}