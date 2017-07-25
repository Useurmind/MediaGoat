using Lucene.Net.Index;
using Lucene.Net.Store;
using MediaGoat.Domain.Model;
using MediaGoat.Utility;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MediaGoat.LuceneExtensions
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

        public long StartIndexing(IObserver<long> currentIndexedNumber)
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

                    ForEachSongInMediaDirectory(mediaPath, song =>
                    {
                        var documents = mapper.Map<Song>(new[] { song }).ToList();
                        foreach (var doc in documents)
                        {
                            writer.AddDocument(doc);
                            counter++;
                            currentIndexedNumber.OnNext(counter);
                        }

                        if (counter % 1000 == 0)
                        {
                            logger.Debug($"Starting intermediate index comit with {counter} elements");
                            writer.Commit();
                            logger.Debug($"Finished intermediate index comit with {counter} elements");
                        }
                    });
                }

                writer.Commit();
            }

            logger.Debug("Finished indexing of song files");

            return counter;
        }

        private void ForEachSongInMediaDirectory(string mediaPath, Action<Song> forEachSong)
        {
            FileHelper.ForEachFileRecursive(mediaPath, filePath =>
            {
                if (Regex.IsMatch(filePath, @"(\.mp3$)|(\.ogg$)|(\.wav$)"))
                {
                    try
                    {
                        var tagLibFile = TagLib.File.Create(filePath);
                        var allArtists = tagLibFile.Tag.AlbumArtists.Union(new[] { tagLibFile.Tag.FirstAlbumArtist })
                        .Union(tagLibFile.Tag.Performers)
                        .Where(x => !string.IsNullOrEmpty(x))
                        .Distinct(StringComparer.OrdinalIgnoreCase);

                        var song = new Song()
                        {
                            Guid = Guid.NewGuid(),
                            Artist = string.Join(", ", allArtists),
                            Album = tagLibFile.Tag.Album,
                            Title = tagLibFile.Tag.Title,
                            FilePath = filePath,
                            ContentType = ContentTypeHelper.GetContentType(filePath)
                        };

                        forEachSong(song);
                    }
                    catch (TagLib.CorruptFileException cfe)
                    {
                        logger.Error(cfe, $"File {filePath} is corrupt according to tag lib.");
                    }
                }
            });
        }
    }
}
