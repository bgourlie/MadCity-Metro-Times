using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MadCityMetroTimes.Model;

namespace MadCityMetroTimes.ViewModels
{
    public class PivotViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private DepartureTimeService _departureTimeService;
        private BusStop _busStop;
        private ObservableCollection<RouteDirection> _routeDirections;
        private ObservableCollection<BusStopTime> _busStopTimes;

        public void RefreshTimes()
        {
            _departureTimeService.GetTimes(_routeDirections);
        }

        public ObservableCollection<RouteDirection>  RouteDirections
        {
            get
            {
                return _routeDirections;
            }

            set
            {
                if (_routeDirections == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("RouteDirections"));
                _routeDirections = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("RouteDirections"));
            }
        }

        public ObservableCollection<BusStopTime> BusStopTimes
        {
            get
            {
                return _busStopTimes;
            }

            set
            {
                if (_busStopTimes == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("BusStopTimes"));
                _busStopTimes = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BusStopTimes"));
            }
        }

        public BusStop BusStop
        {
            get
            {
                return _busStop;
            }

            set
            {
                if (_busStop == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("BusStop"));
                _busStop = value;
                if(_departureTimeService != null)
                {
                    //unregister handler from old service
                    _departureTimeService.TimesDetermined -= OnDepartureTimeRecieved;
                }
                _departureTimeService = new DepartureTimeService(_busStop);
                _departureTimeService.TimesDetermined += OnDepartureTimeRecieved;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BusStop"));
            }
        }

        private void OnDepartureTimeRecieved(ICollection<BusStopTime> busStopTimes)
        {
            var now = DateTime.Now;
            var routes = (from bst in busStopTimes select bst.Route).Distinct().ToArray();
            var oldTimes = BusStopTimes.Where(bst => bst.Time < now || routes.Contains(bst.Route)).ToArray();
            foreach(var oldTime in oldTimes)
            {
                BusStopTimes.Remove(oldTime);
            }

            foreach (var busStopTime in busStopTimes)
            {
                BusStopTimes.Add(busStopTime);
            }
        }
    }
}
