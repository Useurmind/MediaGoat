using Lucene.Net.Documents;
using System.Collections.Generic;
using System.Linq;

namespace MediaGoat.LuceneExtensions
{
    public class DocumentCollectionPropertyMapper
    {
        private IDocumentMapper documentMapper;

        public DocumentCollectionPropertyMapper(IDocumentMapper documentMapper)
        {
            this.documentMapper = documentMapper;
        }

        public IEnumerable<T> Map<T>(IEnumerable<Document> documents)
            where T : new()
        {
            return documents.Select(doc => this.documentMapper.Map<T>(doc));
        }

        public IEnumerable<Document> Map<T>(IEnumerable<T> models)
        {
            return models.Select(m => this.documentMapper.Map(m));
        }
    }
}
