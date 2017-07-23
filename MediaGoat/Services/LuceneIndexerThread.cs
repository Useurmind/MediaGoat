using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaGoat.Services
{
    public class LuceneIndexerThread
    {
        int status = 0;

        private ILuceneIndexer indexer;

        public LuceneIndexerThread(ILuceneIndexer indexer)
        {
            this.indexer = indexer;
        }

        public void Run()
        {
            var wasNotRunning = Interlocked.CompareExchange(ref status, 1, 0) == 0;
            if (wasNotRunning)
            {
                new Thread(() =>
                {
                    try
                    {
                        this.indexer.StartIndexing();
                    }
                    finally
                    {
                        Thread.VolatileWrite(ref status, 0);
                    }
                }).Start();
            }else
            {
                throw new Exception("The index thread is already running");
            }
        }
    }
}
