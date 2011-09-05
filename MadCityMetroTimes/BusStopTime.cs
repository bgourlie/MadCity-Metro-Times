using System;
using MadCityMetroTimes.Model;

namespace MadCityMetroTimes
{
    public class BusStopTime
    {
        public Route Route { get; set; }
        public BusStop BusStop { get; set; }
        public DateTime Time { get; set; }
    }
}