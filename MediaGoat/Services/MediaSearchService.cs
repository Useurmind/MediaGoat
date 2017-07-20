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

    public interface IMediaSearchService
    {
        Song GetSong(Guid songId);

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

        public Song GetSong(Guid songId)
        {
            using (var indexReader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(indexPath))))
            {
                IndexSearcher searcher = new IndexSearcher(indexReader);

                var queryParser = new MultiFieldQueryParser(Lucene.Net.Util.LuceneVersion.LUCENE_CURRENT, new[] { "Guid" }, analyzer);

                Query query = queryParser.Parse(songId.ToString());
                var topDocs = searcher.Search(query, 2);

                if (topDocs.TotalHits > 1)
                {
                    throw new Exception($"Found multiple Songs with guid {songId}");
                }

                var doc = searcher.Doc(topDocs.ScoreDocs.Single().Doc);
                return this.mapper.Map<Song>(new[] { doc }).First();
            }
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
