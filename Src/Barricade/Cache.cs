/*
 * Barricade
 * Copyright (C)2013 2Toad, LLC.
 * licensing@2toad.com
 * 
 * http://2toad.com/Project/Barricade/License
 */

using System;
using System.Web;

namespace Barricade
{
    /// <summary>
    /// A wrapper around the HttpRuntime.Cache
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// A unique header to prevent key collisions in the cache collection.
        /// </summary>
        private static readonly string Header = Guid.NewGuid().ToString("N");

        /// <summary>
        /// A reference to the application cache.
        /// </summary>
        public static System.Web.Caching.Cache Store { get { return HttpRuntime.Cache; } }

        /// <summary>
        /// Adds an item to the cache.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <param name="value">The item to add to the cache.</param>
        /// <param name="expiration">The number of minutes the item will remain cached.</param>
        /// <param name="slidingExpiration">When true, the expiration is reset each time the item is accessed.</param>
        public static void Add(string key, object value, int expiration, bool slidingExpiration)
        {
            Store.Insert(
                Header + key, 
                value, 
                null,
                slidingExpiration ? System.Web.Caching.Cache.NoAbsoluteExpiration : DateTime.UtcNow.AddMinutes(expiration),
                slidingExpiration ? new TimeSpan(0, expiration, 0) : System.Web.Caching.Cache.NoSlidingExpiration
            );
        }

        /// <summary>
        /// Retrieves the specified item from the cache.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <returns>The item associated with the key.</returns>
        public static T Get<T>(string key)
        {
            return (T)Store.Get(Header + key);
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
        /// <returns>The item associated with the key.</returns>
        public static T Get<T>(string key, Func<T> callback, int expiration, bool slidingExpiration) where T : class
        {
            var item = Store.Get(Header + key);
            if (item != null) return (T)item;

            item = callback();
            if (item != null) Add(Header + key, item, expiration, slidingExpiration);
            return item as T;
        }

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        /// <returns>The item associated with the key.</returns>
        public static T Remove<T>(string key)
        {
            return (T)Store.Remove(Header + key);
        }

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key">The key used to reference the item.</param>
        public static void Remove(string key)
        {
            Store.Remove(Header + key);
        }
    }
}
