using System;

namespace MediaGoat.LuceneExtensions
{
    public interface ILuceneIndexer
    {
        /// <summary>
        /// Start the indexing process.
        /// </summary>
        /// <returns>Returns the number of indexed items.</returns>
        long StartIndexing(IObserver<long> currentIndexedNumber);
    }
}
