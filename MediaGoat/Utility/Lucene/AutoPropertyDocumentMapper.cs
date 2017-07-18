using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MediaGoat.Utility.Lucene
{

    public class AutoPropertyDocumentMapper : IDocumentMapper
    {
        private IEnumerable<PropertyInfo> GetMappedProperties(Type modelType)
        {
            return modelType.GetProperties();
        }

        public T Map<T>(Document doc)
            where T : new()
        {
            var model = new T();
            foreach (var property in this.GetMappedProperties(typeof(T)))
            {
                property.SetValue(model, doc.Get(property.Name));
            }
            return model;
        }

        public Document Map<T>(T model)
        {
            var doc = new Document();
            foreach (var property in this.GetMappedProperties(typeof(T)))
            {
                var propertyValue = property.GetValue(model);
                doc.Add(new Field(property.Name, propertyValue.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            }
            return doc;
        }
    }
}
