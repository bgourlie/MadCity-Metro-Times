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
    public class DirectionService
    {
        private static readonly Regex __matchDirectionRegex =
            new Regex("<a class=\"ada\" title=\"([a-z0-9\\- ]+)\" href=\"\\?r=(?:\\d+)&d=(\\d+)\">",
                      RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public event Action<ICollection<Direction>> DirectionsRetrieved;

        public void RetrieveDirections(int routeId, bool forceRefresh = false)
        {
            if(!forceRefresh)
            {
                using(var db = MadMetroDataContext.NewInstance())
                {
                    var directions = (from bsr in db.RouteDirections where bsr.RouteId == routeId select bsr.Direction).ToArray();
                    if (directions.Length > 0)
                    {
                        if (DirectionsRetrieved != null) DirectionsRetrieved(directions);
                        return;
                    }
                }
            }

            var request =
                WebRequest.CreateHttp(string.Format("http://webwatch.cityofmadison.com/webwatch/ada.aspx?r={0}", routeId));
            request.Method = "GET";
            request.BeginGetResponse(ProcessResponse, new object[]{routeId, request});
        }

        private void ProcessResponse(IAsyncResult result)
        {
            var routeId = (int)((object[])result.AsyncState)[0];
            var request = (HttpWebRequest)((object[])result.AsyncState)[1];
            var returnedDirections = new List<Direction>();
            using (WebResponse response = request.EndGetResponse(result))
            {
                //we could assume they're using utf-8, but this future proofs things
                Match encodingMatch = Util.MatchEncodingRegex.Match(response.Headers["Content-Type"]);
                if (!encodingMatch.Success) throw new Exception("Unabled to determine response encoding.");
                string encoding = encodingMatch.Groups[1].Value;

                using (Stream responseStream = response.GetResponseStream())
                {
                    using (var textReader = new StreamReader(responseStream, Encoding.GetEncoding(encoding)))
                    {
                        string html = textReader.ReadToEnd();
                        foreach (Match match in __matchDirectionRegex.Matches(html))
                        {
                            returnedDirections.Add(new Direction {Id = int.Parse(match.Groups[2].Value), Label = match.Groups[1].Value});
                        }
                    }
                }
            }

            if (DirectionsRetrieved != null) DirectionsRetrieved(returnedDirections);
            CacheDirections(returnedDirections, routeId);
        }

        private static void CacheDirections(ICollection<Direction> directions, int routeId)
        {
            var directionIds = from d in directions select d.Id;
            //cache the results in the database.  
            //Insert any Directions that don't already exist
            using (var db = MadMetroDataContext.NewInstance())
            {
                var existingDirections = (from d in db.Directions where directionIds.Contains(d.Id) select d).ToArray();
                foreach (var newDirection in directions.Except(existingDirections))
                {
                    db.Directions.InsertOnSubmit(newDirection);
                }

                //delete existing RouteDirection rows and just re-add 
                var existingRouteDirections = from rd in db.RouteDirections
                                              where directionIds.Contains(rd.DirectionId) && rd.RouteId == routeId
                                              select rd;

                foreach(var existingRouteDirection in existingRouteDirections)
                {
                    db.RouteDirections.DeleteOnSubmit(existingRouteDirection);
                }

                db.SubmitChanges();

                foreach(var directionId in directionIds)
                {
                    db.RouteDirections.InsertOnSubmit(new RouteDirection{DirectionId = directionId, RouteId = routeId});
                }
                
                db.SubmitChanges();
            }
        }
    }
}