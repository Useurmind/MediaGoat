using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace MediaGoat.LuceneExtensions
{
    public class LuceneIndexerThread
    {
        int isRunning = 0;

        private ILuceneIndexer indexer;

        private ISubject<LuceneIndexingResult> statusSubject;

        public IObservable<LuceneIndexingResult> StatusStream
        {
            get
            {
                return this.statusSubject;
            }
        }

        public LuceneIndexerThread(ILuceneIndexer indexer)
        {
            this.indexer = indexer;
            this.statusSubject = new ReplaySubject<LuceneIndexingResult>(1);
            this.statusSubject.OnNext(LuceneIndexingResult.None());
        }

        public void Run()
        {
            var wasNotRunning = Interlocked.CompareExchange(ref isRunning, 1, 0) == 0;
            if (wasNotRunning)
            {
                new Thread(() =>
                {
                    DateTime startTime = DateTime.Now;
                    this.statusSubject.OnNext(LuceneIndexingResult.Running(startTime, 0));

                    var currentNumberObserver = new Subject<long>();
                    using (var subscription = currentNumberObserver.Subscribe(n => this.statusSubject.OnNext(LuceneIndexingResult.Running(startTime, n))))
                    {
                        try
                        {
                            long indexedItems = this.indexer.StartIndexing(currentNumberObserver);

                            this.statusSubject.OnNext(LuceneIndexingResult.Success(startTime, indexedItems));
                        }
                        catch (Exception e)
                        {
                            this.statusSubject.OnNext(LuceneIndexingResult.Failed(startTime, e));
                            throw e;
                        }
                        finally
                        {
                            Thread.VolatileWrite(ref isRunning, 0);
                        }
                    }
                }).Start();
            }
            else
            {
                throw new Exception("The index thread is already running");
            }
        }
    }
}
