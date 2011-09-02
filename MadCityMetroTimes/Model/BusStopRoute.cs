using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MadCityMetroTimes.Model
{
    [Table]
    public class BusStopRoute : INotifyPropertyChanged, INotifyPropertyChanging, IEquatable<BusStopRoute>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private int _routeId;
        private int _busStopId;
        private int _directionId;
        private bool _isTracking;

        private EntityRef<Direction> _direction;
        private EntityRef<Route> _route;
        private EntityRef<BusStop> _busStop;

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

        [Column(IsPrimaryKey = true, IsDbGenerated = false, CanBeNull = false)]
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

        [Association(Storage = "_route", ThisKey = "RouteId", IsForeignKey = true)]
        public Route Route
        {
            get { return _route.Entity; }

            set
            {
                if (_route.Entity == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("Route"));
                _route.Entity = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Route"));
                RouteId = _route.Entity == null ? 0 : _route.Entity.Id;
            }
        }

        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public int BusStopId
        {
            get
            {
                return _busStopId;
            }

            set
            {
                if (_busStopId == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("BusStopId"));
                _busStopId = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BusStopId"));
            }
        }

        [Association(Storage = "_busStop", ThisKey = "BusStopId", IsForeignKey = true)]
        public BusStop BusStop
        {
            get { return _busStop.Entity; }

            set
            {
                if (_busStop.Entity == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("BusStop"));
                _busStop.Entity = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BusStop"));
                BusStopId = _busStop.Entity == null ? 0 : _busStop.Entity.Id;
            }
        }

        [Column(CanBeNull = false)]
        public bool IsTracking
        {
            get
            {
                return _isTracking;
            }
            set
            {
                if (_isTracking == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("IsTracking"));
                _isTracking = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsTracking"));
                
            }
        }

        #region Equality Members

        public bool Equals(BusStopRoute other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._routeId == _routeId && other._directionId == _directionId && other._busStopId == _busStopId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BusStopRoute)) return false;
            return Equals((BusStopRoute) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _routeId;
                result = (result*397) ^ _directionId;
                result = (result*397) ^ _busStopId;
                return result;
            }
        }

        #endregion
    }
}
