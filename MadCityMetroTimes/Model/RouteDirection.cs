using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MadCityMetroTimes.Model
{

    [Table]
    public class RouteDirection : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private int _routeId;
        private int _directionId;
        private EntityRef<Route> _route;
        private EntityRef<Direction> _direction;

        [Association(Storage = "_route", ThisKey = "RouteId", IsForeignKey = true)]
        public Route Route {
            get
            {
                return _route.Entity;
            }

            set
            {
                if (_route.Entity == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("Route"));
                _route.Entity = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Route"));
                RouteId = _route.Entity == null ? 0 : _route.Entity.Id;
            }
        }

        [Association(Storage = "_direction", ThisKey = "DirectionId", IsForeignKey = true)]
        public Direction Direction
        {
            get
            {
                return _direction.Entity;
            }

            set
            {
                if (_direction.Entity == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("Direction"));
                _direction.Entity = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Direction"));
                DirectionId = _direction.Entity == null ? 0 : _direction.Entity.Id;
            }
        }

        [Column(IsPrimaryKey= true, CanBeNull = false)]
        public int RouteId
        {
            get
            {
                return _routeId;
            }

            set
            {
                if (_routeId == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("RouteId"));
                _routeId = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("RouteId"));
            }
        }

        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public int DirectionId
        {
            get
            {
                return _directionId;
            }

            set
            {
                if (_directionId == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("DirectionId"));
                _directionId = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("DirectionId"));
            }
        }

        
    }
}
