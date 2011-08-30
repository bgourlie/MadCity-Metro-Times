using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MadCityMetroTimes
{
    public class ArrivalTimes
    {
        private static readonly Regex __matchArrivalTime = new Regex("<a class=\"ada\" title=\"([0-9: pam.]+)\">",
                                                                     RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly int _direction;
        private readonly int _route;
        private readonly int _stopId;

        public ArrivalTimes(int route, int direction, int stopId)
        {
            _stopId = stopId;
            _route = route;
            _direction = direction;
        }

        public event Action<ICollection<BusStopTime>> TimesDetermined;

        public void GetTimes()
        {
            HttpWebRequest request =
                WebRequest.CreateHttp(
                    string.Format("http://webwatch.cityofmadison.com/webwatch/ada.aspx?r={0}&d={1}&s={2}", _route,
                                  _direction, _stopId));
            request.Method = "GET";
            request.BeginGetResponse(ProcessResponse, request);
        }

        private void ProcessResponse(IAsyncResult result)
        {
            var request = (HttpWebRequest) result.AsyncState;
            var ret = new List<BusStopTime>();
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
                        foreach (Match match in __matchArrivalTime.Matches(html))
                        {
                            ret.Add(new BusStopTime{Route = _route, StopId = _stopId, Time = match.Groups[1].Value});
                        }
                    }
                }
            }
            if (TimesDetermined != null) TimesDetermined(ret);
        }
    }
}