using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using MadCityMetroTimes.Model;

namespace MadCityMetroTimes.ViewModels
{
    public class NewMainViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
        private PivotViewModel _selectedPivotViewModel;
        private ObservableCollection<PivotViewModel> _pivotViewModels;
        private bool _noRoutesTracked;
        
        public void Initialize(IEnumerable<BusStopRoute> busStopRoutes)
        {
            PivotViewModels = new ObservableCollection<PivotViewModel>();

            foreach (var busStopRoute in busStopRoutes)
            {
                //create the pivot view for this bus stop if it doesn't already exist
                if (!_pivotViewModels.Any(pvm => pvm.BusStop.Equals(busStopRoute.BusStop)))
                {
                    var pvm = new PivotViewModel
                                  {
                                      BusStop = busStopRoute.BusStop,
                                      BusStopTimes = new ObservableCollection<BusStopTime>(),
                                      RouteDirections = new ObservableCollection<RouteDirection>()
                                  };

                    PivotViewModels.Add(pvm);
                }
                
                var viewModel =_pivotViewModels.Single(pvm => pvm.BusStop.Equals(busStopRoute.BusStop));
                viewModel.RouteDirections.Add(new RouteDirection{Direction = busStopRoute.Direction, Route = busStopRoute.Route});
            }
            
            if(PivotViewModels.Count > 0) SelectedPivotViewModel = PivotViewModels[0];
            NoRoutesTracked = PivotViewModels.Count == 0;
        }
    
        public ObservableCollection<PivotViewModel> PivotViewModels
        {
            get
            {
                return _pivotViewModels;
            }
            set
            {
                if (_pivotViewModels == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("PivotViewModels"));
                _pivotViewModels = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PivotViewModels"));
            }
        }

        public PivotViewModel SelectedPivotViewModel
        {
            get
            {
                return _selectedPivotViewModel;
            }
            set
            {
                if (_selectedPivotViewModel == value) return;
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("SelectedPivotViewModel"));
                _selectedPivotViewModel = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("SelectedPivotViewModel"));
            }
        }

        public Visibility PivotVisibility
        {
            get
            {
                return NoRoutesTracked ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool NoRoutesTracked
        {
            get
            {
                return _noRoutesTracked;
            }
            
            set
            {
                if (_noRoutesTracked == value) return;
                //since this is the property that really determines the PivotVisibility, we fire the
                //changing events for PivotVisiblity here
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("NoRoutesTracked"));
                if (PropertyChanging != null) PropertyChanging(this, new PropertyChangingEventArgs("PivotVisibility"));
                _noRoutesTracked = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("NoRoutesTracked"));
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PivotVisibility"));
            }
        }
    }
}
