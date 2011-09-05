using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;
using MadCityMetroTimes.Model;
using MadCityMetroTimes.ViewModels;

namespace MadCityMetroTimes
{
    public partial class MainPage
    {
        private Dictionary<BusStop, DepartureTimeService> _services;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(_services == null) _services = new Dictionary<BusStop, DepartureTimeService>();
            var mainViewModel = new MainViewModel {PivotViewModels = new ObservableCollection<PivotViewModel>()};
            using(var db = MadMetroDataContext.NewInstance())
            {
                foreach(var busStopRoute in db.BusStopRoutes.Where(bsr => bsr.IsTracking))
                {
                    if(!_services.ContainsKey(busStopRoute.BusStop))
                    {
                        var service = new DepartureTimeService(busStopRoute.BusStop);
                        service.TimesDetermined += OnDepartureTimeRecieved;
                        _services.Add(busStopRoute.BusStop, service);
                        var pvm = new PivotViewModel
                        {
                            BusStop = busStopRoute.BusStop,
                            BusStopTimes = new ObservableCollection<BusStopTime>(),
                            RouteDirections = new ObservableCollection<RouteDirection>()
                        };

                        mainViewModel.PivotViewModels.Add(pvm);
                    }
                    
                    //get the viewModel for this bus stop so we 
                    //can add all tracked RouteDirections
                    var viewModel = mainViewModel.PivotViewModels.Single(pvm => pvm.BusStop.Equals(busStopRoute.BusStop));
                    var routeDirection = new RouteDirection
                                             {
                                                 Direction = busStopRoute.Direction, 
                                                 Route = busStopRoute.Route
                                             };
                    viewModel.RouteDirections.Add(routeDirection);
                }
            }
            DataContext = mainViewModel;
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/SelectRoute.xaml", UriKind.Relative));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivotViewModel = (PivotViewModel) e.AddedItems[0];
            _services[pivotViewModel.BusStop].GetTimes(pivotViewModel.RouteDirections);
        }

        private void OnDepartureTimeRecieved(BusStop busStop, ICollection<BusStopTime> busStopTimes)
        {
            Dispatcher.BeginInvoke(() =>
                                       {
                                           var now = DateTime.Now;
                                           var mainViewModel = (MainViewModel)DataContext;
                                           var pivotViewModel = mainViewModel.PivotViewModels.Single(pvm => pvm.BusStop.Equals(busStop));
                                           var routes = (from bst in busStopTimes select bst.Route).Distinct().ToArray();
                                           var oldTimes = pivotViewModel.BusStopTimes.Where(bst => bst.Time < now || routes.Contains(bst.Route)).ToArray();
                                           foreach (var oldTime in oldTimes)
                                           {
                                               pivotViewModel.BusStopTimes.Remove(oldTime);
                                           }

                                           foreach (var busStopTime in busStopTimes)
                                           {
                                               pivotViewModel.BusStopTimes.Add(busStopTime);
                                           }
                                       });
        }
    }
}