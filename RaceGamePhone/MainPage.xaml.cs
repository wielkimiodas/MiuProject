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

namespace RaceGamePhone
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            Console.WriteLine("loaded");
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void image1_MouseMove(object sender, MouseEventArgs e)
        {
            Console.WriteLine("move");
        }

        private void image1_MouseEnter(object sender, MouseEventArgs e)
        {
            Console.WriteLine("enter");
        }

        private void button1_MouseEnter(object sender, MouseEventArgs e)
        {
            Console.WriteLine("wchodze");
        }

        private void button1_MouseLeave(object sender, MouseEventArgs e)
        {
            Console.WriteLine("wychodze");
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            textBlock1.Text = "x: "+e.GetPosition(button1).X + Environment.NewLine+ "y:" + e.GetPosition(button1).Y;
        }


    }
}