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
        private BusStopService _busStopService;

        public SelectBusStop()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            if(_busStopService == null)
            {
                _busStopService = new BusStopService();
                _busStopService.BusStopsRetrieved += OnBusStopsRetrieved;
            }
            var routeId = GetRouteId();
            var directionId = GetDirectionId();
            _busStopService.RetrieveBusStops(routeId, directionId);
        }

        private int GetRouteId()
        {
            string routeIdStr;
            NavigationContext.QueryString.TryGetValue("routeId", out routeIdStr);
            return int.Parse(routeIdStr);
        }

        private int GetDirectionId()
        {
            string directionIdStr;
            NavigationContext.QueryString.TryGetValue("directionId", out directionIdStr);
            return int.Parse(directionIdStr); 
        }

        private void OnBusStopsRetrieved(ICollection<BusStop> busStops)
        {
            Dispatcher.BeginInvoke(() => BusStopList.ItemsSource = busStops);
        }

        private void OnTextBlockTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var button = (TextBlock)sender;
            var busStop = (BusStop)button.DataContext;
            var routeId = GetRouteId();
            var directionId = GetDirectionId();

            using (var db = MadMetroDataContext.NewInstance())
            {
                var busStopRoute =
                    db.BusStopRoutes.Single(
                        bsr => bsr.RouteId == routeId && bsr.BusStopId == busStop.Id && bsr.DirectionId == directionId);

                if (!busStopRoute.IsTracking)
                {
                    var addRouteResult =
                        MessageBox.Show(
                            string.Format("Track times for route {0} at {1} (id #{2})?", routeId, busStop.Label,
                                          busStop.SignId), "Track this route?", MessageBoxButton.OKCancel);

                    if (addRouteResult == MessageBoxResult.OK)
                    {
                        busStopRoute.IsTracking = true;
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