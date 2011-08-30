using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MadCityMetroTimes.Model;

namespace MadCityMetroTimes
{
    public static class BusStopService
    {
        public static event Action<ICollection<BusStop>> BusStopsRetrieved;
        private static readonly Regex __matchBusStopRegex = new Regex(@"<a class=\""ada\"" title=\""([a-z0-9& ]+) \[ID#([0-9]+)\](?:\(([a-z& ]+)\))?\"" href=\""\?r=(?:[0-9]+)&d=(?:[0-9]+)&s=([0-9]+)\"">", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static void RetrieveBusStops(int routeId, int directionId)
        {
            HttpWebRequest request =
                WebRequest.CreateHttp(string.Format("http://webwatch.cityofmadison.com/webwatch/ada.aspx?r={0}&d={1}", routeId, directionId));
            request.Method = "GET";
            request.BeginGetResponse(ProcessResponse, request);
        }

        private static void ProcessResponse(IAsyncResult result)
        {
            var request = (HttpWebRequest)result.AsyncState;
            var ret = new List<BusStop>();
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
        }
    }
}
