using System;
using System.Runtime.Caching;

namespace ELMAR.DevHtmlHelper.Helpers
{
    public static class MemoryCacheObject<T> where T : class
    {
        private static readonly MemoryCache _cache = MemoryCache.Default;

        //Store Stuff in the cache  
        public static void StoreDataInCache(string key, T cachedObject)
        {
            //Do what you need to do here. Database Interaction, Serialization,etc.
            var cacheItemPolicy = new CacheItemPolicy()
            {
                //Set your Cache expiration.
                AbsoluteExpiration = DateTime.Now.AddHours(12)
            };

            if(_cache.Contains(key))
                _cache.Remove(key);

            //remember to use the above created object as third parameter.
            _cache.Add(key, cachedObject, cacheItemPolicy);
        }

        //Get stuff from the cache
        public static T GetDataFromCache(string key)
        {
            if (_cache.Contains(key))
                return _cache.Get(key) as T;

            return null;
        }

        //Remove stuff from the cache. If no key supplied, all data will be erased.
        public static void RemoveItemsFromCache(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                _cache.Dispose();
            }
            else
            {
                _cache.Remove(key);
            }
        }
    }
}
