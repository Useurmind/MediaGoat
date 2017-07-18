using Lucene.Net.Documents;

namespace MediaGoat.Utility.Lucene
{
    public interface IDocumentMapper
    {
        T Map<T>(Document doc)
            where T : new();

        Document Map<T>(T model);
    }
}
