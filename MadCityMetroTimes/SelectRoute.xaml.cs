using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MadCityMetroTimes.Model;
using Microsoft.Phone.Controls;

namespace MadCityMetroTimes
{
    public partial class SelectRoute : PhoneApplicationPage
    {
        private RouteService _routeService;

        public SelectRoute()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(_routeService == null)
            {
                _routeService = new RouteService();
                _routeService.RoutesRetrieved += OnRoutesRetrieved;
            }
            _routeService.RetrieveRoutes();
        }

        void OnRoutesRetrieved(ICollection<Route> routes)
        {
            Dispatcher.BeginInvoke(() => RouteList.ItemsSource = routes);
        }

        private void OnTextBlockTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var button = (TextBlock)sender;
            var route = (Route)button.DataContext;
            var uri = new Uri(string.Format("/SelectDirection.xaml?routeId={0}", route.Id), UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}