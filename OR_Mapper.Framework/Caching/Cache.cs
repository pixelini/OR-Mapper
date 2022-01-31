using System;
using System.Collections.Generic;
using System.Linq;

namespace OR_Mapper.Framework.Caching
{
    public class Cache : ICache
    {
        public Dictionary<Type, Dictionary<object, object>> SingleCache { get; set; } =
            new Dictionary<Type, Dictionary<object, object>>();

        public Dictionary<Type, List<object>> CollectionCache { get; set; } =
            new Dictionary<Type, List<object>>();

        
        public void Add(object obj)
        {
            if (SingleCache.ContainsKey(obj.GetType()))
            {
                var model = new Model(obj.GetType());
                var pk = model.PrimaryKey.GetValue(obj);
                SingleCache[obj.GetType()][pk] = obj;
            }
            else
            {
                var objects = new Dictionary<object, object>();
                SingleCache[obj.GetType()] = objects;

                
                var model = new Model(obj.GetType());
                var pk = model.PrimaryKey.GetValue(obj);
                SingleCache[obj.GetType()][pk] = obj;
            }
        }
        
        public void AddCollection<T>(List<T> all) where T : class
        {
            if (CollectionCache.ContainsKey(typeof(T)))
            {
                CollectionCache[typeof(T)] = all.OfType<object>().ToList();
            }
            else
            {
                var collection = new List<object>();
                CollectionCache[typeof(T)] = collection;

                CollectionCache[typeof(T)] = all.OfType<object>().ToList();
            }
        }

        public void Remove(object obj)
        {
            if (!SingleCache.ContainsKey(obj.GetType())) return;
            
            var model = new Model(obj.GetType());
            var pk = model.PrimaryKey.GetValue(obj);
            SingleCache[obj.GetType()].Remove(pk);
        }

        public bool ExistsById(object id, Type type)
        {
            if (!SingleCache.ContainsKey(type)) return false;
            
            var model = new Model(type);
            return SingleCache[type].ContainsKey(id);
        }

        public object Get(object id, Type type)
        {
            if (!SingleCache.ContainsKey(type)) return false;
            
            var model = new Model(type);
            return SingleCache[type][id];
        }

        public List<T> GetAll<T>()
        {
            if (!CollectionCache.ContainsKey(typeof(T)))
            {
                return new List<T>();
            }
            
            return CollectionCache[typeof(T)].Cast<T>().ToList();
        }

        public bool HasChanged(object obj)
        {
            return true;
        }
    }
}