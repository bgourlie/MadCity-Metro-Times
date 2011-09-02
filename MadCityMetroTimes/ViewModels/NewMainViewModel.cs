using System.Collections.Generic;
using System.Collections.ObjectModel;
using MadCityMetroTimes.Model;

namespace MadCityMetroTimes.ViewModels
{
    public class NewMainViewModel
    {
        public ObservableCollection<BusStop> BusStops { get; set; }
        public Dictionary<BusStop, BusStopTime> BusStopTimes { get; set; }
    }
}
