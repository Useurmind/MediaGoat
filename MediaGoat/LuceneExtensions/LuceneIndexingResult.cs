using System;

namespace MediaGoat.LuceneExtensions
{
    public class LuceneIndexingResult
    {
        public LuceneIndexingStatus Status { get; set; }
        public string Message { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }

        public static LuceneIndexingResult None()
        {
            return new LuceneIndexingResult()
            {
                Status = LuceneIndexingStatus.None,
                Message = "No indexing performed recently"
            };
        }

        public static LuceneIndexingResult Running(DateTime startTime, long currentNumber)
        {
            return new LuceneIndexingResult()
            {
                Status = LuceneIndexingStatus.Running,
                Message = $"Indexing currently running, already indexed {currentNumber} items",
                StartTime = startTime,
                FinishTime = null
            };
        }

        public static LuceneIndexingResult Success(DateTime startTime, long numberIndexedItems)
        {
            return new LuceneIndexingResult()
            {
                Status = LuceneIndexingStatus.Success,
                Message = $"Indexing finished successfully after {numberIndexedItems} items",
                StartTime = startTime,
                FinishTime = DateTime.Now
            };
        }

        public static LuceneIndexingResult Failed(DateTime startTime, Exception e)
        {
            return new LuceneIndexingResult()
            {
                Status = LuceneIndexingStatus.Failed,
                Message = $"Indexing failed with:\r\n{e.ToString()}",
                StartTime = startTime,
                FinishTime = DateTime.Now
            };
        }
    }
}
