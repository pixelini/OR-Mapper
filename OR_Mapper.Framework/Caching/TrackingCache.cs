using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using OR_Mapper.Framework.Extensions;

namespace OR_Mapper.Framework.Caching
{
    public class TrackingCache : ICache
    {
        public Dictionary<Type, Dictionary<object, object>> SingleCache { get; set; } =
            new Dictionary<Type, Dictionary<object, object>>();

        public Dictionary<Type, List<object>> CollectionCache { get; set; } =
            new Dictionary<Type, List<object>>();
        
        public Dictionary<Type, Dictionary<object, string>> Hashes { get; set; } =
            new Dictionary<Type, Dictionary<object, string>>();

        
        public void Add(object obj)
        {
            var hashDictionary = GetHash(obj.GetType());
            var type = obj.GetType();
            
            if (SingleCache.ContainsKey(type))
            {
                var model = new Model(type);
                var pk = model.PrimaryKey.GetValue(obj);
                SingleCache[type][pk] = obj;
                hashDictionary.Add(pk, CreateHash(obj));
            }
            else
            {
                var objects = new Dictionary<object, object>();
                SingleCache[type] = objects;
                
                var model = new Model(type);
                var pk = model.PrimaryKey.GetValue(obj);
                SingleCache[type][pk] = obj;
                hashDictionary.Add(pk, CreateHash(obj));
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
            var hashDictionary = GetHash(obj.GetType());
            var type = obj.GetType();

            if (!SingleCache.ContainsKey(type)) return;
            
            var model = new Model(type);
            var pk = model.PrimaryKey.GetValue(obj);
            SingleCache[type].Remove(pk);
            hashDictionary.Remove(pk);
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
            var hashDictionary = GetHash(obj.GetType());
            var model = new Model(obj.GetType());
            var pk = model.PrimaryKey.GetValue(obj);

            if (pk != null && hashDictionary.ContainsKey(pk))
            {
                return hashDictionary[pk] == CreateHash(obj);
            }

            return true;
        }

        public Dictionary<object, string> GetHash(Type type)
        {
            if (Hashes.ContainsKey(type))
            {
                return Hashes[type];
            }

            var hashDictionary = new Dictionary<object, string>();
            Hashes.Add(type, hashDictionary);

            return hashDictionary;
        }
        
        public string CreateHash(object obj)
        {
            var hash = "";
            var type = obj.GetType();
            var model = new Model(type);
            
            foreach (var field in model.Fields)
            {
                var value = field.GetValue(model);
                if (value is null)
                {
                    continue;
                }

                if (field.IsForeignKey)
                {
                    var valueModel = new Model(value.GetType());
                    var properties = type.GetProperties();
                    var correspondingProperty = properties.First(x => x.PropertyType.GetUnderlyingType() == valueModel.Member);
                    var entity = correspondingProperty.GetValue(obj);
                    hash += valueModel.PrimaryKey.GetValue(entity)?.ToString();
                }
                else
                {
                    hash += field.ColumnName + "=" + value.ToString() + "?";
                }
            }

            foreach (var externalField in model.ExternalFields)
            {
                var value = externalField.GetValue(model);
                if (value is null)
                {
                    continue;
                }

                hash += externalField.Model.TableName + "=" + value.ToString() + "?";
            }

            return Encoding.UTF8.GetString(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(hash)));
        }
    }
}