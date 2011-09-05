using System.Collections.ObjectModel;
using System.ComponentModel;
using MadCityMetroTimes.Model;

namespace MadCityMetroTimes.ViewModels
{
    public class PivotViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private BusStop _busStop;
        private ObservableCollection<RouteDirection> _routeDirections;
        private ObservableCollection<BusStopTime> _busStopTimes;

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
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BusStop"));
            }
        }
    }
}
