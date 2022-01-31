using System;
using System.Collections.Generic;

namespace OR_Mapper.Framework.Caching
{
    /// <summary>
    /// This interfaces is implemented by caches.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Holds single cache.
        /// </summary>
        Dictionary<Type, Dictionary<object, object>> SingleCache { get; set; }

        /// <summary>
        /// Adds an object to a cache.
        /// </summary>
        /// <param name="obj">Object.</param>
        public void Add(object obj);

        /// <summary>
        /// Adds a list of objects to a cache.
        /// </summary>
        /// <param name="all">List of objects.</param>
        /// <typeparam name="T">Class of given objects.</typeparam>
        public void AddCollection<T>(List<T> all) where T : class;
        
        /// <summary>
        /// Removes an object from a cache.
        /// </summary>
        /// <param name="obj">Object.</param>
        public void Remove(object obj);
        
        /// <summary>
        /// Checks if object is already in cache.
        /// </summary>
        /// <param name="id">Object id.</param>
        /// <param name="type">Type.</param>
        /// <returns>True if object already exists in cache.</returns>
        public bool ExistsById(object id, Type type);

        /// <summary>
        /// Gets an object from cache.
        /// </summary>
        /// <param name="id">Object id.</param>
        /// <param name="type">Type.</param>
        /// <returns>Cached object.</returns>
        public object Get(object id, Type type);
        
        /// <summary>
        /// Gets an object list from cache.
        /// </summary>
        /// <typeparam name="T">Class of objects.</typeparam>
        /// <returns>Cached objects.</returns>
        public List<T> GetAll<T>();
        
        /// <summary>
        /// Checks if cached object has changed.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>True if object has changed.</returns>
        public bool HasChanged(object obj);

    }
}