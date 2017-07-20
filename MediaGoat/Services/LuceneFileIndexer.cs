using Lucene.Net.Index;
using Lucene.Net.Store;
using MediaGoat.Utility.Lucene;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MediaGoat.Services
{
    public class LuceneSongFileIndexer : ILuceneIndexer
    {
        private IEnumerable<string> mediaPaths;
        private string luceneIndexPath;
        private DocumentCollectionPropertyMapper mapper;

        public LuceneSongFileIndexer(IEnumerable<string> mediaPaths, string luceneIndexPath, DocumentCollectionPropertyMapper mapper)
        {
            this.mediaPaths = mediaPaths;
            this.luceneIndexPath = luceneIndexPath;
            this.mapper = mapper;
        }

        public void StartIndexing()
        {
            if (System.IO.Directory.Exists(luceneIndexPath))
            {
                System.IO.Directory.Delete(luceneIndexPath, true);
            }

            var analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_CURRENT);

            using (var luceneIndexDirectory = FSDirectory.Open(new DirectoryInfo(luceneIndexPath)))
            using (var writer = new IndexWriter(luceneIndexDirectory, new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_CURRENT, analyzer)))
            {
                foreach (var mediaPath in this.mediaPaths)
                {
                    var dataModels = GetMp3SongModelsRecursive(mediaPath);

                    var documents = mapper.Map<Song>(dataModels);
                    foreach (var doc in documents)
                    {
                        writer.AddDocument(doc);
                    }
                }

                writer.Commit();
            }


        }

        private IEnumerable<Song> GetMp3SongModelsRecursive(string mediaPath)
        {
            var files = System.IO.Directory.GetFiles(mediaPath);

            return files.Where(x => x.EndsWith(".mp3")).Select(x =>
            {
                var tagLibFile = TagLib.File.Create(x);
                return new Song()
                {
                    Guid = Guid.NewGuid(),
                    Artist = tagLibFile.Tag.FirstAlbumArtist,
                    Album = tagLibFile.Tag.Album,
                    Title = tagLibFile.Tag.Title,
                    FilePath = x,
                    ContentType = "audio/mpeg"
                };
            });
        }
    }
}
