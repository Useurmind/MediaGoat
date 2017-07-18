using Microsoft.Extensions.Configuration;
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
    }
}
