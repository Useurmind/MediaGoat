using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGoat.Utility
{
    public static class ContentTypeHelper
    {
        public static string GetContentType(string filePath)
        {
            switch (Path.GetExtension(filePath))
            {
                case ".mp3":
                    return "audio/mpeg";
                case ".wav":
                    return "audio/wav";
                case ".ogg":
                    return "audio/ogg";
                default:
                    throw new NotImplementedException($"Content type for unkown file type '{Path.GetExtension(filePath)}' requested");
            }
        }
    }
}
