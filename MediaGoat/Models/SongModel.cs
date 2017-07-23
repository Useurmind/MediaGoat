using MediaGoat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGoat.Models
{
    public class SongModel
    {
        public SongModel(Song song, string url)
        {
            this.Guid = song.Guid;
            this.Album = song.Album;
            this.Title = song.Title;
            this.Artist = song.Artist;
            this.Url = url;
            this.ContentType = song.ContentType;
        }

        public Guid Guid { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        //public IEnumerable<string> Artists { get; set; }

        public string Album { get; set; }

        public string Url { get; set; }

        public string ContentType { get; set; }
    }
}
