using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MadMetroTimes.Model;

namespace MadMetroTimes
{
    public static class RouteService
    {
        private static readonly Regex __matchRouteRegex = new Regex(
            "<a class=\"ada\" title=\"(\\d\\d), ([a-z0-9 ]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static event Action<ICollection<Route>> RoutesRetrieved;

        public static void RetrieveRoutes()
        {
            HttpWebRequest request = WebRequest.CreateHttp("http://webwatch.cityofmadison.com/webwatch/ada.aspx?");
            request.Method = "GET";
            request.BeginGetResponse(ProcessResponse, request);
        }

        private static void ProcessResponse(IAsyncResult result)
        {
            var request = (HttpWebRequest) result.AsyncState;
            var ret = new List<Route>();
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
                        foreach (Match match in __matchRouteRegex.Matches(html))
                        {
                            ret.Add(new Route {Id = int.Parse(match.Groups[1].Value), Label = match.Groups[2].Value});
                        }
                    }
                }
            }
            if (RoutesRetrieved != null) RoutesRetrieved(ret);
        }
    }
}