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
        private InputState input = new InputState();
        private Accelerometer accelSensor;
        public ConnectionManager connectionMgr;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            connectionMgr = new ConnectionManager(this);
            
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

        public void InitAccelerometer()
        {
            accelSensor = new Accelerometer();
            accelSensor.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(accelSensor_ReadingChanged);
            accelSensor.Start();
            accelSensor.TimeBetweenUpdates = new TimeSpan(0, 0, 0, 0, 45);
        }

        void accelSensor_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            
            if (connectionMgr.IsStarted)
            {                                
                input.steer = -(float)e.Y;
                connectionMgr.SendContent(input);                
            }            
        }

        private void StartGame()
        {
            if (connectionMgr.IsStarted)
            {
                InitAccelerometer();
                Touch.FrameReported += new TouchFrameEventHandler(Touch_FrameReported);
            }
            else
            {
                MessageBox.Show("Connect to server");
            }

        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            (App.Current as App).MainClassPointer = this;
            NavigationService.Navigate(new Uri("/ConfigurationPage.xaml", UriKind.Relative));                       
        }

        private void btnAcceleration_MouseLeave(object sender, MouseEventArgs e)
        {
            input.acceleration = 0;
            tbAccelInfo.Text = "zero";
        }

        private void btnBreak_MouseLeave(object sender, MouseEventArgs e)
        {
            input.breakVal= 0;
            tbBrakeInfo.Text = "zero";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

    }
}