using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MadCityMetroTimes.Model
{
    [Table]
    public class Route : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
        private readonly EntitySet<BusStopRouteDirection> _busStopRoutes = new EntitySet<BusStopRouteDirection>();
        private readonly EntitySet<RouteDirection> _routeDirections = new EntitySet<RouteDirection>();
        
        [Association(Storage = "_routeDirections", OtherKey = "DirectionId")]
        public EntitySet<RouteDirection> RouteDirections
        {
            get { return _routeDirections; }
            set { _routeDirections.Assign(value); }
        }

        [Association(Storage = "_busStopRoutes", OtherKey = "RouteId")]
        public EntitySet<BusStopRouteDirection> ButStopRoutes
        {
            get { return _busStopRoutes; }
            set { _busStopRoutes.Assign(value); }
        }

        private int _id;
        private string _label;

        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public int Id {
            get
            {
                return _id;
            }
            set
            {
                if (_id == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("Id"));
                _id = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Id"));

            }
        }

        [Column(CanBeNull = false)]
        public string Label { 
            get
            {
                return _label;
            }
            set
            {
                if (_label == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("Label"));
                _label = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Label"));
            }
        }
    }

    public class RouteCollection : ObservableCollection<Route>{}
}