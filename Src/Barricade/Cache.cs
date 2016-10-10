/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

using System;
using Microsoft.Extensions.Caching.Memory;

namespace Barricade
{
    /// <summary>
    /// A wrapper around the HttpRuntime.Cache
    /// </summary>
    public static class Cache
    {
        static Cache()
        {
            Header = Guid.NewGuid().ToString("N");
            Store = new MemoryCache(new MemoryCacheOptions());
        }

        /// <summary>
        /// A unique header to prevent key collisions in the cache collection.
        /// </summary>
        private static readonly string Header;

        /// <summary>
        /// A reference to the application cache.
        /// </summary>
        private static readonly MemoryCache Store;

        /// <summary>
        /// The number of entries in the cache.
        /// </summary>
        public static int Count => Store.Count;

        /// <summary>
        /// Adds an item to the cache, replacing the existing item if the key already exists.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The item to add to the cache.</param>
        /// <param name="expiration">The number of minutes the item will remain cached.</param>
        /// <param name="slidingExpiration">When true, the expiration is reset each time the item is accessed.</param>
        public static void Add(string key, object value, int expiration, bool slidingExpiration = true)
        {
            var offset = TimeSpan.FromMinutes(expiration);
            var options = new MemoryCacheEntryOptions();

            if (slidingExpiration) {
                options.SetSlidingExpiration(offset);
            } else {
                options.SetAbsoluteExpiration(offset);
            }

            Store.Set(
                Header + key,
                value,
                options
            );
        }

        /// <summary>
        /// Retrieves the specified item from the cache.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <returns>The item associated with the key, or default(T) if the key doesn't exist.</returns>
        public static T Get<T>(string key)
        {
            return Store.Get<T>(Header + key);
        }

        /// <summary>
        /// Retrieves the specified item from the cache. If the item isn't cached, the item is produced
        /// via the provided <paramref name="callback"/> delegate, and cached prior to being returned.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="callback">The delegate that will be called if the item does not exist in the cache.</param>
        /// <param name="expiration">The number of minutes the item will remain cached.</param>
        /// <param name="slidingExpiration">When true, the expiration is reset each time the item is accessed.</param>
        /// <returns>The item associated with the key, or default(T) if the key doesn't exist.</returns>
        public static T GetOrCreate<T>(string key, Func<T> callback, int expiration, bool slidingExpiration = true)
        {
            T item;
            if (TryGet(key, out item)) return item;

            item = callback();
            Add(key, item, expiration, slidingExpiration);
            return item;
        }

        /// <summary>
        /// Retrieves the specified item from the cache.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The item associated with the key.</param>
        /// <returns>True if the key exists.</returns>
        public static bool TryGet<T>(string key, out T value)
        {
            return Store.TryGetValue(Header + key, out value);
        }

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        public static void Remove(string key)
        {
            Store.Remove(Header + key);
        }

        /// <summary>
        /// Gets the specified item and removes it from the cache.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <returns>The item associated with the key, or default(T) if the key doesn't exist.</returns>
        public static T Pop<T>(string key)
        {
            T item;
            if (TryGet(key, out item)) Remove(key);
            return item;
        }
    }
}
