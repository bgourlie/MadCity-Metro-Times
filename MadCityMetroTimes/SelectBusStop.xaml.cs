using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MadCityMetroTimes.Model;
using Microsoft.Phone.Controls;

namespace MadCityMetroTimes
{
    public partial class SelectBusStop : PhoneApplicationPage
    {
        private int _routeId;

        public SelectBusStop()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            string routeIdStr;
            string directionIdStr;
            NavigationContext.QueryString.TryGetValue("routeId", out routeIdStr);
            NavigationContext.QueryString.TryGetValue("directionId", out directionIdStr);
            _routeId = int.Parse(routeIdStr);
            int directionId = int.Parse(directionIdStr);
            BusStopService.RetrieveBusStops(_routeId, directionId);
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            BusStopService.BusStopsRetrieved += BusStop_BusStopsRetrieved;
        }

        private void BusStop_BusStopsRetrieved(ICollection<BusStop> busStops)
        {
            Dispatcher.BeginInvoke(() => BusStopList.ItemsSource = busStops);
        }

        private void TextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var button = (TextBlock)sender;
            var busStop = (BusStop)button.DataContext;

            var busStopRoute = new BusStopRoute { RouteId = _routeId, BusStopId = busStop.Id };

            using (var db = MadMetroDataContext.NewInstance())
            {
                var alreadyExists =
                    db.BusStopRoutes.Any(
                        bsr => bsr.RouteId == busStopRoute.RouteId && bsr.BusStopId == busStopRoute.BusStopId);

                if (!alreadyExists)
                {
                    var addRouteResult =
                        MessageBox.Show(
                            string.Format("Track times for route {0} at {1} (id #{2})?", _routeId, busStop.Label,
                                          busStop.SignId), "Track this route?", MessageBoxButton.OKCancel);

                    if (addRouteResult == MessageBoxResult.OK)
                    {
                        db.BusStopRoutes.InsertOnSubmit(busStopRoute);
                        db.SubmitChanges();
                    }
                }else
                {
                    MessageBox.Show("You are already tracking this route.");
                }
            }
        }
    }
}