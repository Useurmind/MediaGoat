using Lucene.Net.Documents;

namespace MediaGoat.LuceneExtensions
{
    public interface IDocumentMapper
    {
        T Map<T>(Document doc)
            where T : new();

        Document Map<T>(T model);
    }
}
