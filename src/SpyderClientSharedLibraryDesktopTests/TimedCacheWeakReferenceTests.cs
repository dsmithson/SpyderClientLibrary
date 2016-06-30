using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Knightware;

namespace Spyder.Client
{
    [TestClass]
    public class TimedCacheWeakReferenceTests
    {
        [TestMethod]
        public void CacheExpirationConstructorTest()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(1);
            var expected = new TestClass();
            var weakReference = new TimedCacheWeakReference<TestClass>(expected, timeout);
            Assert.IsTrue(weakReference.StrongReferenceAvailable, "Strong reference should have been available");
            Assert.AreEqual(timeout, weakReference.CacheDuration, "Cache duration reported by class is incorrect");

            //Ensure we can get our reference immediately
            TestClass actual;
            Assert.IsTrue(weakReference.TryGetTarget(out actual), "Failed to get reference while within cache expiration time");
            Assert.AreSame(expected, actual, "Different reference returned");

            //Now sleep and let our cache expire
            Thread.Sleep(weakReference.CacheDuration.Add(TimeSpan.FromSeconds(2)));
            Assert.IsFalse(weakReference.StrongReferenceAvailable, "A strong reference should not still be available after cache expiration");

            //Collect GC, and then ensure we no longer have access to our item
            expected = null;
            actual = null;
            GC.Collect();
            Assert.IsFalse(weakReference.TryGetTarget(out actual), "Should not have been able to access our class after GC");
        }

        [TestMethod]
        public void GetTargetTest()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(1);
            var weakReference = new TimedCacheWeakReference<TestClass>(new TestClass(), timeout);
            for (int i = 0; i < 6; i++)
            {
                Thread.Sleep(500);

                //We should be able to get access to a strong reference to our test class, since cache expiration hasn't occurred.
                //the act of getting the item should reset the cache
                TestClass actual;
                Assert.IsTrue(weakReference.StrongReferenceAvailable, "Strong target reference is no longer available, which should not have happened");
                Assert.IsTrue(weakReference.TryGetTarget(out actual), "Failed to get instance of test class");

                actual = null;
                GC.Collect();
            }
        }

        private void cacheExpirationTest()
        {
        }

        private class TestClass
        {
            public Guid Identifier { get; set; }

            public TestClass()
            {
                Identifier = Guid.NewGuid();
            }
        }
    }
}
