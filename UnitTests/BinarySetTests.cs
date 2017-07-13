using BusterWood.Collections.Immutable;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class BinarySetTests
    {
        [Test]
        public void can_be_empty()
        {
            var s = new BinarySet<int>();
            Assert.AreEqual(0, s.Count);
            Assert.AreEqual(false, s.Contains(2));
        }

        [Test]
        public void can_contain_one_item()
        {
            var s = new BinarySet<int>(2);
            Assert.AreEqual(1, s.Count);
            Assert.AreEqual(true, s.Contains(2));
        }

        [Test]
        public void union_empty_and_empty_returns_empty()
        {
            var s = new BinarySet<int>();
            var u = s.Union(new BinarySet<int>());
            Assert.AreEqual(0, u.Count);
        }

        [Test]
        public void union_empty_and_item_returns_item()
        {
            var s = new BinarySet<int>();
            var u = s.Union(new BinarySet<int>(7));
            Assert.AreEqual(1, u.Count);
            Assert.AreEqual(true, u.Contains(7));
        }

        [Test]
        public void union_item_and_empty_returns_item()
        {
            var s = new BinarySet<int>(8);
            var u = s.Union(new BinarySet<int>());
            Assert.AreEqual(1, u.Count);
            Assert.AreEqual(true, u.Contains(8));
        }
    }
}
