using MediaGoat.LuceneExtensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace MediaGoat.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private LuceneIndexerThread luceneIndexThread;

        public AdminController(LuceneIndexerThread luceneIndexThread)
        {
            this.luceneIndexThread = luceneIndexThread;
        }

        [HttpPost("[action]")]
        public void StartIndexRun()
        {
            this.luceneIndexThread.Run();
        }

        [HttpGet("[action]")]
        public LuceneIndexingResult CurrentIndexingResult()
        {
            // it's ok to wait, its a replay subject that always has a value
            var result = this.luceneIndexThread.StatusStream.First();
            return result;
        }
    }
}
