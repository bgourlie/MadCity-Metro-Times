using System.Collections.ObjectModel;
using MadMetroTimes.Model;

namespace MadMetroTimes.ViewModels
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
