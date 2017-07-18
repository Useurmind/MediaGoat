using Lucene.Net.Index;
using Lucene.Net.Store;
using MediaGoat.Utility.Configuration;
using MediaGoat.Utility.Lucene;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGoat.Services
{
    public interface ILuceneIndexer
    {
        void StartIndexing();
    }

    public class LuceneJsonIndexer<T> : ILuceneIndexer
    {
        private string indexPath;
        private string jsonDataPath;
        private DocumentCollectionPropertyMapper mapper;

        public LuceneJsonIndexer(IConfiguration configuration, DocumentCollectionPropertyMapper mapper)
        {
            this.indexPath = configuration.GetLuceneIndexPath();
            this.jsonDataPath = configuration.GetLuceneJsonDataPath();
            this.mapper = mapper;
        }

        public void StartIndexing()
        {
            var jsonData = string.Join(@"", File.ReadAllLines(jsonDataPath));
            var dataModels = JsonConvert.DeserializeObject<IEnumerable<T>>(jsonData);

            if (System.IO.Directory.Exists(indexPath))
            {
                System.IO.Directory.Delete(indexPath, true);
            }

            var analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_CURRENT);

            using (var luceneIndexDirectory = FSDirectory.Open(new DirectoryInfo(indexPath)))
            using (var writer = new IndexWriter(luceneIndexDirectory, new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_CURRENT, analyzer)))
            {

                var documents = mapper.Map<T>(dataModels);
                foreach (var doc in documents)
                {
                    writer.AddDocument(doc);
                }
                writer.Commit();
            }
        }
    }
}
