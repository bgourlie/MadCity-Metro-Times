﻿
using System.Data.Linq;
using MadMetroTimes.Model;

namespace MadMetroTimes
{
    public class MadMetroDataContext : DataContext
    {
        private const string CONN_STRING = "Data Source=isostore:/MadMetroTimes.sdf";
        private MadMetroDataContext(string fileOrConnection) : base(fileOrConnection){}
        
        public Table<BusStop> BusStops;
        public Table<Route> Routes;
        public Table<Direction> Directions;
        public Table<RouteDirection> RouteDirections;
        public Table<BusStopRoute> BusStopRoutes;
        public Table<BusStopDirection> BusStopDirections;
 
        public static MadMetroDataContext GetInstance()
        {
            return new MadMetroDataContext(CONN_STRING);
        }
    }
}