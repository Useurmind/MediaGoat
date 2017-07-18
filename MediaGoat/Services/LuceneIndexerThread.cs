using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaGoat.Services
{
    public class LuceneIndexerThread
    {
        private ILuceneIndexer indexer;

        public LuceneIndexerThread(ILuceneIndexer indexer)
        {
            this.indexer = indexer;
        }

        public void Run()
        {
            new Thread(() =>
            {
                this.indexer.StartIndexing();
            }).Start();            
        }
    }
}
