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
    public class RouteService
    {
        private static readonly Regex __matchRouteRegex = new Regex(
            "<a class=\"ada\" title=\"(\\d\\d), ([a-z0-9 ]+)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public event Action<ICollection<Route>> RoutesRetrieved;

        public void RetrieveRoutes(bool forceRefresh = false)
        {
            if(!forceRefresh)
            {
                using(var db = MadMetroDataContext.NewInstance())
                {
                    var routes = db.Routes.ToArray();
                    if(routes.Length > 0)
                    {
                        if(RoutesRetrieved != null) RoutesRetrieved(routes);
                        return;
                    }
                }
            }

            var request = WebRequest.CreateHttp("http://webwatch.cityofmadison.com/webwatch/ada.aspx?");
            request.Method = "GET";
            request.BeginGetResponse(ProcessResponse, request);
        }

        private void ProcessResponse(IAsyncResult result)
        {
            var request = (HttpWebRequest) result.AsyncState;
            var routes = new List<Route>();
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
                            routes.Add(new Route {Id = int.Parse(match.Groups[1].Value), Label = match.Groups[2].Value});
                        }
                    }
                }
            }
            if (RoutesRetrieved != null) RoutesRetrieved(routes);
            CacheRoutes(routes);
        }

        private static void CacheRoutes(ICollection<Route> routes)
        {
            var retrievedRouteIds = from r in routes select r.Id;
            using(var db = MadMetroDataContext.NewInstance())
            {
                var existingRouteIds = from r in db.Routes where !retrievedRouteIds.Contains(r.Id) select r.Id;
                foreach(var newRouteId in retrievedRouteIds.Except(existingRouteIds))
                {
                    db.Routes.InsertOnSubmit(routes.Single(r => r.Id == newRouteId));
                }
                db.SubmitChanges();
            }
        }
    }
}