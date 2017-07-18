using MediaGoat.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        public IEnumerable<Song> Songs(string searchString)
        {
            return this.mediaSearchService.SearchSongs(searchString);
        }
    }
}
