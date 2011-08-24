using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace MadMetroTimes.Model
{
    [Table]
    public class BusStop : INotifyPropertyChanged, INotifyPropertyChanging, IEquatable<BusStop>
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private int _id;
        private int _signId;
        private string _intersection;
        private string _label;
        private readonly EntitySet<BusStopRoute> _busStopRoutes = new EntitySet<BusStopRoute>();

        [Association(Storage = "_busStopRoutes", OtherKey = "BusStopId")]
        public EntitySet<BusStopRoute> ButStopRoutes
        {
            get { return _busStopRoutes; }
            set { _busStopRoutes.Assign(value); }
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

        //the number they actually display on the sign
        [Column(CanBeNull = false)]
        public int SignId {
            get
            {
                return _signId;
            }

            set
            {
                if (_signId == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("SignId"));
                _signId = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SignId"));
            }
        }

        [Column(CanBeNull = false)]
        public string Intersection { 
            get
            {
                return _intersection;
            }

            set
            {
                if (_intersection == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("Intersection"));
                _intersection = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Intersection"));
            }
        }

        [Column(CanBeNull = false)]
        public string Label
        {
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

        #region Equality Members
        public bool Equals(BusStop other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._id == _id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BusStop)) return false;
            return Equals((BusStop) obj);
        }

        public override int GetHashCode()
        {
            return _id;
        }
        #endregion
    }

    public class BusStopCollection : ObservableCollection<BusStop>{}
}
