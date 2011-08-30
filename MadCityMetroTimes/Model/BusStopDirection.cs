using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MadCityMetroTimes.Model
{
    [Table]
    public class BusStopDirection : INotifyPropertyChanged, INotifyPropertyChanging, IEquatable<BusStopDirection>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
        
        private EntityRef<BusStop> _busStop;
        private EntityRef<Direction> _direction;

        private int _busStopId;
        private int _directionId;

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

        [Association(Storage = "_busStop", ThisKey = "BusStopId", IsForeignKey = true)]
        public BusStop BusStop
        {
            get
            {
                return _busStop.Entity;
            }

            set
            {
                if (_busStop.Entity == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("BusStop"));
                _busStop.Entity = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("BusStop"));
                BusStopId = _busStop.Entity == null ? 0 : _busStop.Entity.Id;
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

        #region Equality Members

        public bool Equals(BusStopDirection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._busStopId == _busStopId && other._directionId == _directionId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BusStopDirection)) return false;
            return Equals((BusStopDirection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_busStopId*397) ^ _directionId;
            }
        }

        #endregion

    }
}
