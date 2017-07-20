using System;

namespace MediaGoat.Services
{
    public class Song
    {
        public Guid Guid { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        //public IEnumerable<string> Artists { get; set; }

        public string Album { get; set; }

        public string FilePath { get; set; }

        public string ContentType { get; set; }
    }
}
