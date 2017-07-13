using BusterWood.Collections;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class OHashSetTests
    {
        [Test]
        public void can_add_unique_value()
        {
            var set = new OHashSet<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void cannot_add_duplicate_values()
        {
            var set = new OHashSet<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(false, set.Add(1));
            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void can_add_unique_value2()
        {
            var set = new OHashSet<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Add(2));
            Assert.AreEqual(2, set.Count);
        }

        [Test]
        public void can_fill_set()
        {
            var set = new OHashSet<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Add(2));
            Assert.AreEqual(true, set.Add(3));
            Assert.AreEqual(3, set.Count);
        }

        [Test]
        public void can_resize_set()
        {
            var set = new OHashSet<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Add(2));
            Assert.AreEqual(true, set.Add(3));
            Assert.AreEqual(true, set.Add(4));
            Assert.AreEqual(4, set.Count);
        }
    }
}