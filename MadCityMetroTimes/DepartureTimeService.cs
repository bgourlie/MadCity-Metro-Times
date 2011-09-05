using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MadCityMetroTimes.Model;

namespace MadCityMetroTimes
{
    public class DepartureTimeService
    {
        private static readonly Regex __matchUpdatedTime = new Regex("<a class=\"ada\" title=\"Prediction last updated ([0-9:/ pam.]+)\">",
                                                                     RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex __matchArrivalTime = new Regex("<a class=\"ada\" title=\"([0-9: pam.]+)\">",
                                                                     RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public readonly BusStop BusStop;

        public DepartureTimeService(BusStop busStop)
        {
            BusStop = busStop;
        }

        public event Action<ICollection<BusStopTime>> TimesDetermined;

        public void GetTimes(IEnumerable<RouteDirection> routeDirections)
        {
            foreach (var routeDirection in routeDirections)
            {
                var request =
                    WebRequest.CreateHttp(
                        string.Format("http://webwatch.cityofmadison.com/webwatch/ada.aspx?r={0}&d={1}&s={2}",
                                      routeDirection.RouteId,
                                      routeDirection.DirectionId, BusStop.Id));
                request.Method = "GET";
                request.BeginGetResponse(ProcessResponse, new object[] {request, routeDirection});
            }
        }

        private void ProcessResponse(IAsyncResult result)
        {
            var state = (object[]) result.AsyncState;
            var request = (HttpWebRequest) state[0];
            var routeDirection = (RouteDirection) state[1];
            var ret = new List<BusStopTime>();
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
                        var updateTimeString = __matchUpdatedTime.Match(html).Groups[1].Value;
                        var updateTime = DateTime.Parse(updateTimeString);
                        foreach (Match match in __matchArrivalTime.Matches(html))
                        {
                            var departureTimeString = match.Groups[1].Value.Replace(".", "");
                            DateTime departureTime;
                            if(departureTimeString.ToLower().EndsWith("am") && updateTime.ToString("tt").ToLower() == "pm")
                            {
                                //the bus is actually arriving the next day (past midnight), so we can't just parse it, we'll have to add a day too
                                departureTime = DateTime.Parse(departureTimeString).AddDays(1);
                            }
                            else
                            {
                                //no need to add a day, the departure time occurs today
                                departureTime = DateTime.Parse(departureTimeString);       
                            }
                            ret.Add(new BusStopTime { Route = routeDirection.Route, BusStop = BusStop, Time = departureTime });
                        }
                    }
                }
            }
            if (TimesDetermined != null) TimesDetermined(ret);
        }
    }
}