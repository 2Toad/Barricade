using System;
using NUnit.Framework;

namespace Barricade.Tests
{
    public class CacheTest
    {
        [SetUp]
        public void ResetCache()
        {
            // Cache is a static class, and MemoryCache does not contain
            // a Clear() method, so we have to reset the static class
            // before each test, if we want counts to be accurate
            var staticType = typeof(Cache);
            var ci = staticType.TypeInitializer;
            var parameters = new object[0];
            ci.Invoke(null, parameters);
        }

        [Test]
        public void AddNull()
        {
            var key = Guid.NewGuid().ToString("N");

            Cache.Add(key, null, 5);

            Assert.AreEqual(1, Cache.Count);
        }

        [Test]
        public void AddReferenceType()
        {
            var key = Guid.NewGuid().ToString("N");
            var item = new object();

            Cache.Add(key, item, 5);

            Assert.AreEqual(1, Cache.Count);
        }

        [Test]
        public void AddValueType()
        {
            var key = Guid.NewGuid().ToString("N");
            const int item = 1;

            Cache.Add(key, item, 5);

            Assert.AreEqual(1, Cache.Count);
        }

        [Test]
        public void AddItemWithSlidingExpiration()
        {
            var key = Guid.NewGuid().ToString("N");
            const int item = 1;

            Cache.Add(key, item, 5);

            Assert.AreEqual(1, Cache.Count);
        }

        [Test]
        public void AddItemWithAbsoluteExpiration()
        {
            var key = Guid.NewGuid().ToString("N");
            const int item = 1;

            Cache.Add(key, item, 5, false);

            Assert.AreEqual(1, Cache.Count);
        }

        [Test]
        public void GetReferenceType()
        {
            var key = Guid.NewGuid().ToString("N");
            var item = Guid.NewGuid().ToString("N");

            Cache.Add(key, item, 5);
            var output = Cache.Get<string>(key);

            Assert.IsNotNull(output);
            Assert.AreEqual(item, output);
        }

        [Test]
        public void GetValueType()
        {
            var key = Guid.NewGuid().ToString("N");
            var item = DateTime.Now.Ticks;

            Cache.Add(key, item, 5);
            var output = Cache.Get<long?>(key);

            Assert.IsNotNull(output);
            Assert.AreEqual(item, output);
        }

        [Test]
        public void GetItemDoesNotExist()
        {
            var key = Guid.NewGuid().ToString("N");
            var output = Cache.Get<int?>(key);
            Assert.IsNull(output);
        }

        [Test]
        public void GetOrCreate()
        {
            var key = Guid.NewGuid().ToString("N");
            var item1 = Guid.NewGuid();
            var item2 = Guid.NewGuid();

            Cache.Add(key, item1, 5);
            var output = Cache.GetOrCreate(key, () => item2, 5);

            Assert.AreEqual(1, Cache.Count);
            Assert.IsNotNull(output);
            Assert.AreEqual(item1, output);
            Assert.AreNotEqual(item1, item2);
        }

        [Test]
        public void GetOrCreateItemDoesNotExists()
        {
            var key = Guid.NewGuid().ToString("N");
            var item = Guid.NewGuid();

            var output = Cache.GetOrCreate(key, () => item, 5);
            var output2 = Cache.Get<Guid>(key);

            Assert.AreEqual(1, Cache.Count);
            Assert.IsNotNull(output);
            Assert.AreEqual(item, output);
            Assert.AreEqual(output, output2);
        }

        [Test]
        public void TryGet()
        {
            var key = Guid.NewGuid().ToString("N");
            var item = Guid.NewGuid();

            Cache.Add(key, item, 5);

            Guid output;
            var success = Cache.TryGet(key, out output);

            Assert.IsTrue(success);
            Assert.IsNotNull(output);
            Assert.AreEqual(item, output);
        }

        [Test]
        public void TryGetItemDoesNotExist()
        {
            var key = Guid.NewGuid().ToString("N");

            Guid? output;
            var success = Cache.TryGet(key, out output);

            Assert.IsFalse(success);
            Assert.IsNull(output);
        }

        [Test]
        public void ReplaceItem()
        {
            var key = Guid.NewGuid().ToString("N");
            var item1 = Guid.NewGuid();
            var item2 = Guid.NewGuid();

            Cache.Add(key, item1, 5);
            Cache.Add(key, item2, 5);
            var output = Cache.Get<Guid>(key);

            Assert.IsNotNull(output);
            Assert.AreNotEqual(item1, item2);
            Assert.AreEqual(item2, output);
        }

        [Test]
        public void Remove()
        {
            var key = Guid.NewGuid().ToString("N");
            var item = Guid.NewGuid();

            Cache.Add(key, item, 5);
            Cache.Remove(key);

            Assert.AreEqual(0, Cache.Count);
        }

        [Test]
        public void RemoveItemDoesNotExist()
        {
            var key = Guid.NewGuid().ToString("N");

            Cache.Remove(key);

            Assert.AreEqual(0, Cache.Count);
        }

        [Test]
        public void Pop()
        {
            var key1 = Guid.NewGuid().ToString("N");
            var key2 = Guid.NewGuid().ToString("N");
            var item = Guid.NewGuid();

            Cache.Add(key1, item, 5);
            Cache.Add(key2, item, 5);

            Assert.AreNotEqual(key1, key2);
            Assert.AreEqual(2, Cache.Count);

            var output = Cache.Pop<Guid>(key1);

            Assert.AreEqual(1, Cache.Count);
            Assert.AreEqual(item, output);
        }

        [Test]
        public void PopItemDoesNotExist()
        {
            var key1 = Guid.NewGuid().ToString("N");
            var key2 = Guid.NewGuid().ToString("N");
            var item = Guid.NewGuid();

            Cache.Add(key1, item, 5);

            Assert.AreNotEqual(key1, key2);
            Assert.AreEqual(1, Cache.Count);

            var output = Cache.Pop<Guid?>(key2);

            Assert.AreEqual(1, Cache.Count);
            Assert.IsNull(output);
        }
    }
}
