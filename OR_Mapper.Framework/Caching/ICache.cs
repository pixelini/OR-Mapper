using System;
using System.Collections.Generic;

namespace OR_Mapper.Framework.Caching
{
    public interface ICache
    {
        Dictionary<Type, Dictionary<object, object>> SingleCache { get; set; }

        public void Add(object obj);

        public void AddCollection<T>(List<T> all) where T : class;
        
        public void Remove(object obj);
        
        public bool ExistsById(object id, Type type);

        public object Get(object id, Type type);
        
        public List<T> GetAll<T>();

    }
}