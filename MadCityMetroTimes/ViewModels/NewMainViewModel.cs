using System.Collections.ObjectModel;
using MadCityMetroTimes.Model;

namespace MadCityMetroTimes.ViewModels
{
    public class NewMainViewModel
    {
        public ObservableCollection<BusStop> BusStops { get; private set; }

        public NewMainViewModel()
        {
            BusStops = new ObservableCollection<BusStop>();
        }
    }
}
