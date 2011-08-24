using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MadMetroTimes.Model;

namespace MadMetroTimes
{
    public static class DirectionService
    {
        private static readonly Regex __matchDirectionRegex =
            new Regex("<a class=\"ada\" title=\"([a-z0-9\\- ]+)\" href=\"\\?r=(?:\\d+)&d=(\\d+)\">",
                      RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static event Action<ICollection<Direction>> DirectionsRetrieved;

        public static void RetrieveDirections(int routeId, bool forceRefresh = false)
        {
            if(!forceRefresh)
            {
                using(var db = MadMetroDataContext.GetInstance())
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
            request.BeginGetResponse(ProcessResponse, request);
        }

        private static void ProcessResponse(IAsyncResult result)
        {
            var request = (HttpWebRequest) result.AsyncState;
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
            var directionIds = (from d in returnedDirections select d.Id).ToArray();
            //cache the results in the database.  We just erase all existin rows for this row and reinsert
            using(var db = MadMetroDataContext.GetInstance())
            {
                var existingDirections = from d in db.Directions where directionIds.Contains(d.Id) select d;
                foreach(var newDirection in returnedDirections.Except(existingDirections))
                {
                    db.Directions.InsertOnSubmit(newDirection);
                }

                db.SubmitChanges();
            }
        }
    }
}