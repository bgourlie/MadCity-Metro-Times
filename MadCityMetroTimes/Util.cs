﻿using System.Text.RegularExpressions;

namespace MadCityMetroTimes
{
    public class Util
    {
        public static readonly Regex MatchEncodingRegex = new Regex(@"charset=([a-z0-9\-]+)",
                                                                    RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}