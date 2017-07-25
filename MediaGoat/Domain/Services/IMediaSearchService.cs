using System;
using System.Collections.Generic;
using MediaGoat.Domain.Model;

namespace MediaGoat.Domain.Services
{
    public interface IMediaSearchService
    {
        Song GetSong(Guid songId);

        IEnumerable<Song> SearchSongs(string searchString);
    }
}
