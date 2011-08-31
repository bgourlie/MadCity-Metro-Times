using System.Data.Linq;
using MadCityMetroTimes.Model;

namespace MadCityMetroTimes
{
    public class MadMetroDataContext : DataContext
    {
        private const string CONN_STRING = "Data Source=isostore:/MadCityMetroTimes.sdf";
        private MadMetroDataContext(string fileOrConnection) : base(fileOrConnection){}
        
        public Table<BusStop> BusStops;
        public Table<Route> Routes;
        public Table<Direction> Directions;
        public Table<RouteDirection> RouteDirections;
        public Table<BusStopRouteDirection> BusStopRouteDirections;
 
        public static MadMetroDataContext NewInstance()
        {
            return new MadMetroDataContext(CONN_STRING);
        }
    }
}
