using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spyder.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spyder.Client.Collections
{
    [TestClass]
    public class ListExtensionsTests
    {
        [TestMethod]
        public void CopyToTest()
        {
            TestClass sourceItem = new TestClass() { ID = 2, Name = "Test 2" };
            TestClass destItem = new TestClass() { ID = 2, Name = "Updated Name" };

            var sourceList = new List<TestClass>()
            {
                new TestClass() { ID = 1, Name = "Test 1" },
                sourceItem
            };

            var targetList = new List<TestClass>()
            {
                destItem
            };

            sourceList.CopyTo(targetList,
                (item) => item.ID,
                (item) => item.ID,
                (item) => new TestClass(item),
                (copyFrom, copyTo) => copyTo.CopyFrom(copyFrom));

            Assert.AreEqual(sourceList.Count, targetList.Count, "List counts don't match after copy");

            //The copy should actually update the target class, not replace it
            Assert.AreSame(destItem, targetList.First(item => item.ID == destItem.ID), "Object reference was replaced for dest item");
            Assert.AreEqual(sourceItem.Name, destItem.Name, "Name was not updated on dest item");
        }

        [TestMethod]
        public void ConstrainedCopyToTest()
        {
            var sourceLists = new List<List<TestClass>>()
            {
                new List<TestClass>() { new TestClass() { ID = 1, Name = "Test 1" } },
                new List<TestClass>() { new TestClass() { ID = 2, Name = "Test 2" } },
                new List<TestClass>() { new TestClass() { ID = 3, Name = "Test 3" } },
                new List<TestClass>() { new TestClass() { ID = 4, Name = "Test 4" } },
                new List<TestClass>() { new TestClass() { ID = 5, Name = "Test 5" } }
            };

            const int filterOutID = 3;

            var list3 = new List<TestClass>()
            {
                new TestClass() { ID = filterOutID, Name = "Filter me out" }
            };

            foreach (var sourceList in sourceLists)
            {
                sourceList.ConstrainedCopyTo(list3,
                    (item) => item.ID == filterOutID,
                    (item) => item.ID,
                    (item) => item.ID,
                    (item) => new TestClass(item),
                    (copyFrom, copyTo) => copyTo.CopyFrom(copyFrom));
            }

            int expectedCount = sourceLists.SelectMany(l => l.Where(item => item.ID != filterOutID)).Count();
            Assert.AreEqual(expectedCount, list3.Count, "Destination list count was unexpected");
            Assert.IsFalse(list3.Any(item => item.ID == filterOutID), $"ID {filterOutID} should not have been present");
        }

        private class TestClass
        {
            public int ID { get; set; }
            public string Name { get; set; }

            public TestClass()
            {
            }

            public TestClass(TestClass copyFrom)
            {
                CopyFrom(copyFrom);
            }

            public void CopyFrom(TestClass copyFrom)
            {
                this.ID = copyFrom.ID;
                this.Name = copyFrom.Name;
            }
        }
    }
}
