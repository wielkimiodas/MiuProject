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
    public partial class ConfigurationPage : PhoneApplicationPage
    {
        
        public ConfigurationPage()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            var b = frame.Content as PhoneApplicationPage;
            var last = NavigationService.BackStack.FirstOrDefault();
            var mainPage = (App.Current as App).MainClassPointer;

            var connMgr = mainPage.connectionMgr;
            bool isSuccess = connMgr.Connect(tbAddress.Text);

            if (isSuccess) tbStatus.Text = "Connected";
            else tbStatus.Text = "Connection could not be established";
        }
    }
}