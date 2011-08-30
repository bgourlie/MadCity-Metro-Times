using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using MadCityMetroTimes.Model;
using Microsoft.Phone.Controls;

namespace MadCityMetroTimes
{
    public partial class SelectDirection : PhoneApplicationPage
    {
        private DirectionService _directionService;

        public SelectDirection()
        {
            InitializeComponent();
        }
        private int GetRouteId()
        {
            string routeIdStr;
            NavigationContext.QueryString.TryGetValue("routeId", out routeIdStr);
            return int.Parse(routeIdStr);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if(_directionService == null)
            {
                _directionService = new DirectionService();
                _directionService.DirectionsRetrieved += OnDirectionsRetrieved;
            }
            var routeId = GetRouteId();
            _directionService.Execute(routeId);
        }

        void OnDirectionsRetrieved(ICollection<Direction> directions)
        {
            Dispatcher.BeginInvoke(() => DirectionList.ItemsSource = directions);
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var button = (TextBlock)sender;
            var direction = (Direction)button.DataContext;
            var routeId = GetRouteId();
            var uri =
                new Uri(string.Format("/SelectBusStop.xaml?routeId={0}&directionId={1}", routeId,
                                      direction.Id), UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}