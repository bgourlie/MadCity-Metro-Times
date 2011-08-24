using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MadMetroTimes.Model;
using Microsoft.Phone.Controls;

namespace MadMetroTimes
{
    public partial class SelectDirection : PhoneApplicationPage
    {
        private int _currentRoute;

        public SelectDirection()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            DirectionService.DirectionsRetrieved += Direction_DirectionsRetrieved;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string routeIdStr;
            NavigationContext.QueryString.TryGetValue("routeId", out routeIdStr);
            _currentRoute = int.Parse(routeIdStr);
            DirectionService.RetrieveDirections(_currentRoute);
        }

        void Direction_DirectionsRetrieved(ICollection<Direction> directions)
        {
            Dispatcher.BeginInvoke(() => DirectionList.ItemsSource = directions);
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var button = (TextBlock)sender;
            var direction = (Direction)button.DataContext;
            var uri =
                new Uri(string.Format("/SelectBusStop.xaml?routeId={0}&directionId={1}", _currentRoute,
                                      direction.Id), UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}