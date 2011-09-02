using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MadCityMetroTimes.Model;

namespace MadCityMetroTimes
{
    public class BusStopService
    {
        public event Action<ICollection<BusStop>> BusStopsRetrieved;
        private static readonly Regex __matchBusStopRegex = new Regex(@"<a class=\""ada\"" title=\""([a-z0-9& ]+) \[ID#([0-9]+)\](?:\(([a-z& ]+)\))?\"" href=\""\?r=(?:[0-9]+)&d=(?:[0-9]+)&s=([0-9]+)\"">", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        public void RetrieveBusStops(int routeId, int directionId, bool forceRefresh = false)
        {
            if(!forceRefresh)
            {
                using(var db = MadMetroDataContext.NewInstance())
                {
                    var busStops = (from bs in db.BusStopRouteDirections
                                   where bs.RouteId == routeId && bs.DirectionId == directionId
                                   select bs.BusStop).ToArray();

                    if (busStops.Length > 0)
                    {
                        if (BusStopsRetrieved != null) BusStopsRetrieved(busStops);
                        return;
                    }
                }
            }

            var request =
                WebRequest.CreateHttp(string.Format("http://webwatch.cityofmadison.com/webwatch/ada.aspx?r={0}&d={1}", routeId, directionId));
            request.Method = "GET";
            request.BeginGetResponse(ProcessResponse, new object[]{request, routeId, directionId});
        }

        private void ProcessResponse(IAsyncResult result)
        {
            var state = (object[]) result.AsyncState;
            var request = (HttpWebRequest) state[0];
            var routeId = (int) state[1];
            var directionId = (int) state[2];

            var ret = new List<BusStop>();
            using (var response = request.EndGetResponse(result))
            {
                //we could assume they're using utf-8, but this future proofs things
                Match encodingMatch = Util.MatchEncodingRegex.Match(response.Headers["Content-Type"]);
                if (!encodingMatch.Success) throw new Exception("Unabled to determine response encoding.");
                string encoding = encodingMatch.Groups[1].Value;

                using (var responseStream = response.GetResponseStream())
                {
                    using (var textReader = new StreamReader(responseStream, Encoding.GetEncoding(encoding)))
                    {
                        string html = textReader.ReadToEnd();
                        foreach (Match match in __matchBusStopRegex.Matches(html))
                        {
                            //If it doesn't have a more descriptive label, then the label is the intersection
                            var busStopLabel = !string.IsNullOrEmpty(match.Groups[3].Value) ? match.Groups[3].Value : match.Groups[1].Value;
                            ret.Add(new BusStop
                                        {
                                            Intersection = match.Groups[1].Value, 
                                            Label = busStopLabel, 
                                            SignId = int.Parse(match.Groups[2].Value), 
                                            Id = int.Parse(match.Groups[4].Value)
                                        });
                        }
                    }
                }
            }
            if (BusStopsRetrieved != null) BusStopsRetrieved(ret);
            CacheBusStops(ret, routeId, directionId);
        }

        private static void CacheBusStops(IEnumerable<BusStop> retrievedBusStops, int routeId, int directionId)
        {
            using(var db = MadMetroDataContext.NewInstance())
            {
                var retrievedBusStopIds = from bs in retrievedBusStops select bs.Id;
                var existingBusStops = from bs in db.BusStops where retrievedBusStopIds.Contains(bs.Id) select bs;
                var busStopsCopy = retrievedBusStops;
                db.BusStops.InsertAllOnSubmit(busStopsCopy.Except(existingBusStops));
                db.SubmitChanges();

                var busStopRouteDirections = from bsId in retrievedBusStopIds 
                                             select new BusStopRoute {BusStopId = bsId, RouteId = routeId, DirectionId = directionId};

                var retrievedBusStopIdsCopy = retrievedBusStopIds;
                var existingBusStopRouteDirectionIds = from bsrd in db.BusStopRouteDirections
                                                       where
                                                           retrievedBusStopIdsCopy.Contains(bsrd.BusStopId) &&
                                                           bsrd.RouteId == routeId && bsrd.DirectionId == directionId
                                                       select bsrd;

                db.BusStopRouteDirections.InsertAllOnSubmit(busStopRouteDirections.Except(existingBusStopRouteDirectionIds));
                db.SubmitChanges();
            }
        }
    }
}
