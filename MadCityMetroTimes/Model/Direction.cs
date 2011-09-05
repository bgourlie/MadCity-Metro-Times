using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MadCityMetroTimes.Model
{
    [Table]
    public class Direction : INotifyPropertyChanged, INotifyPropertyChanging, IEquatable<Direction>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private readonly EntitySet<RouteDirection> _routeDirections = new EntitySet<RouteDirection>();

        private int _id;
        private string _label;

        [Association(Storage = "_routeDirections", OtherKey = "DirectionId")]
        public EntitySet<RouteDirection> RouteDirections
        {
            get { return _routeDirections; }
            set { _routeDirections.Assign(value); }
        }

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
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("Label"));
            }
        }

        #region Equality Members

        public bool Equals(Direction other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._id == _id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Direction)) return false;
            return Equals((Direction) obj);
        }

        public override int GetHashCode()
        {
            return _id;
        }

        #endregion
    }
}