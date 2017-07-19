using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGoat.Utility.Configuration
{
    public static class ConfigurationExtensions
    {
        public static string GetLuceneIndexPath(this IConfiguration configuration)
        {
            return configuration["Lucene:IndexPath"];
        }

        public static string GetLuceneJsonDataPath(this IConfiguration configuration)
        {
            return configuration["Lucene:JsonDataPath"];
        }

        public static IEnumerable<string> GetLuceneMediaPaths (this IConfiguration configuration)
        {
            return configuration.GetSection("Lucene:MediaPaths").Get<List<string>>();
        }
    }
}
