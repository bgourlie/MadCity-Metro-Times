using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using MadMetroTimes.Model;
using Microsoft.Phone.Controls;

namespace MadMetroTimes
{
    public partial class SelectRoute : PhoneApplicationPage
    {
        public SelectRoute()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            RouteService.RoutesRetrieved += Route_RoutesRetrieved;
            RouteService.RetrieveRoutes();
        }

        void Route_RoutesRetrieved(ICollection<Route> routes)
        {
            Dispatcher.BeginInvoke(() => RouteList.ItemsSource = routes);
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var button = (TextBlock)sender;
            var route = (Route)button.DataContext;
            var uri = new Uri(string.Format("/SelectDirection.xaml?routeId={0}", route.Id), UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}