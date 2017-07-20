using MediaGoat.Models;
using MediaGoat.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGoat.Controllers
{
    [Route("api/[controller]")]
    public class SongController : Controller
    {
        private IMediaSearchService mediaSearchService;

        public SongController(IMediaSearchService mediaSearchService)
        {
            this.mediaSearchService = mediaSearchService;
        }

        [HttpGet("[action]")]
        public IEnumerable<SongModel> Songs(string searchString)
        {
            return this.mediaSearchService.SearchSongs(searchString).Select(x => new SongModel(x, $"/api/Song/Song?songId={x.Guid}"));
        }

        [HttpGet("[action]")]
        public IActionResult Song(Guid songId)
        {
            var song = this.mediaSearchService.GetSong(songId);
            var fileStream = new FileStream(song.FilePath, FileMode.Open);
            return new FileStreamResult(fileStream, song.ContentType);
        }
    }
}
