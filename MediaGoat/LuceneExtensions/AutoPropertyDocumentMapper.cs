using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace MediaGoat.LuceneExtensions
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
                var stringValue = doc.Get(property.Name);

                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(model, stringValue);
                }
                else if (property.PropertyType == typeof(Guid))
                {
                    property.SetValue(model, new Guid(stringValue));
                }
                else
                {
                    throw new NotImplementedException($"The type {property.PropertyType.Name} is not mapped to a model type");
                }
            }
            return model;
        }

        public Document Map<T>(T model)
        {
            var doc = new Document();
            foreach (var property in this.GetMappedProperties(typeof(T)))
            {
                var propertyValue = property.GetValue(model);
                if(propertyValue == null)
                {
                    propertyValue = string.Empty;
                }
                doc.Add(new Field(property.Name, propertyValue?.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            }
            return doc;
        }
    }
}
