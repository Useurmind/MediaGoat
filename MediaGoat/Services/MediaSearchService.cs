using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers.Classic;
using MediaGoat.Utility.Lucene;
using Microsoft.Extensions.Configuration;
using MediaGoat.Utility.Configuration;

namespace MediaGoat.Services
{
    public class Song
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
    }

    public interface IMediaSearchService
    {
        IEnumerable<Song> SearchSongs(string searchString);
    }

    public class MediaSearchService : IMediaSearchService
    {
        private Analyzer analyzer;
        private DocumentCollectionPropertyMapper mapper;
        private string indexPath;

        public MediaSearchService(IConfiguration configuration, DocumentCollectionPropertyMapper mapper)
        {
            this.indexPath = configuration.GetLuceneIndexPath();
            this.analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_CURRENT);
            this.mapper = mapper;

        }

        public IEnumerable<Song> SearchSongs(string searchString)
        {
            using (var indexReader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(indexPath))))
            {
                IndexSearcher searcher = new IndexSearcher(indexReader);

                var documents = new List<Document>();
                if (string.IsNullOrEmpty(searchString))
                {
                    for (int i = 0; i < Math.Min(100, indexReader.MaxDoc); i++)
                    {
                        documents.Add(searcher.Doc(i));
                    }
                }
                else
                {
                    var queryParser = new MultiFieldQueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_CURRENT, new[] { "Title", "Album", "Artist" }, analyzer);

                    Query query = queryParser.Parse(searchString);
                    var topDocs = searcher.Search(query, 100);

                    foreach (var topDoc in topDocs.ScoreDocs)
                    {
                        documents.Add(searcher.Doc(topDoc.Doc));
                    }
                }
                return this.mapper.Map<Song>(documents);
            }
        }
    }
}
