using Lucene.Net.Index;
using Lucene.Net.Store;
using MediaGoat.Utility;
using MediaGoat.Utility.Lucene;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MediaGoat.Services
{
    public class LuceneSongFileIndexer : ILuceneIndexer
    {
        private IEnumerable<string> mediaPaths;
        private string luceneIndexPath;
        private DocumentCollectionPropertyMapper mapper;
        private ILogger logger;

        public LuceneSongFileIndexer(IEnumerable<string> mediaPaths, string luceneIndexPath, DocumentCollectionPropertyMapper mapper, ILogger logger)
        {
            this.mediaPaths = mediaPaths;
            this.luceneIndexPath = luceneIndexPath;
            this.mapper = mapper;
            this.logger = logger;
        }

        public void StartIndexing()
        {
            logger.Debug("Starting indexing of song files");

            if (System.IO.Directory.Exists(luceneIndexPath))
            {
                System.IO.Directory.Delete(luceneIndexPath, true);
            }

            var analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(Lucene.Net.Util.LuceneVersion.LUCENE_CURRENT);

            long counter = 0;
            using (var luceneIndexDirectory = FSDirectory.Open(new DirectoryInfo(luceneIndexPath)))
            using (var writer = new IndexWriter(luceneIndexDirectory, new IndexWriterConfig(Lucene.Net.Util.LuceneVersion.LUCENE_CURRENT, analyzer)))
            {
                foreach (var mediaPath in this.mediaPaths)
                {
                    logger.Debug($"Indexing files in path {mediaPath}");

                    logger.Debug("Starting to retrieve files with Id3 tags");
                    var dataModels = GetMp3SongModelsRecursive(mediaPath).ToList();
                    logger.Debug("Finished to retrieve files with Id3 tags, starting indexing");

                    var documents = mapper.Map<Song>(dataModels).ToList();
                    foreach (var doc in documents)
                    {
                        writer.AddDocument(doc);
                        counter++;
                    }

                    if (counter % 1000 == 0)
                    {
                        logger.Debug($"Starting intermediate index comit with {counter} elements");
                        writer.Commit();
                        logger.Debug($"Finished intermediate index comit with {counter} elements");
                    }
                }

                writer.Commit();
            }

            logger.Debug("Finished indexing of song files");
        }

        private IEnumerable<Song> GetMp3SongModelsRecursive(string mediaPath)
        {
            var files = new List<string>();
            FileHelper.GetFilesRecursive(mediaPath, files);

            return files.Where(x => Regex.IsMatch(x, @"(\.mp3$)|(\.ogg$)|(\.wav$)")).Select(x =>
            {
                var tagLibFile = TagLib.File.Create(x);
                return new Song()
                {
                    Guid = Guid.NewGuid(),
                    Artist = tagLibFile.Tag.FirstAlbumArtist,
                    Album = tagLibFile.Tag.Album,
                    Title = tagLibFile.Tag.Title,
                    FilePath = x,
                    ContentType = ContentTypeHelper.GetContentType(x)
                };
            });
        }
    }
}
