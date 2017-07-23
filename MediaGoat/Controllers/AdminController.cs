using MediaGoat.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
