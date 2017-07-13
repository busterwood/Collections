using BusterWood.Collections;
using NUnit.Framework;
using System.Linq;

namespace UnitTests
{
    [TestFixture]
    public class DistinctListTests
    {
        [Test]
        public void can_add_unique_value()
        {
            var set = new DistinctList<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void does_not_add_duplicate_values()
        {
            var set = new DistinctList<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(false, set.Add(1));
            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void set_contains_added_item()
        {
            var set = new DistinctList<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Contains(1));
        }

        [Test]
        public void can_remove_only_item()
        {
            var set = new DistinctList<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Remove(1));
            Assert.AreEqual(false, set.Contains(1));
            Assert.AreEqual(0, set.Count);
        }

        [Test]
        public void can_remove_first_item()
        {
            var set = new DistinctList<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Add(2));
            Assert.AreEqual(true, set.Remove(1));
            Assert.AreEqual(false, set.Contains(1));
            Assert.AreEqual(true, set.Contains(2));
            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void can_iterate_added_items()
        {
            var set = new DistinctList<int>();
            Assert.AreEqual(true, set.Add(10));
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(10, set.ElementAt(0));
            Assert.AreEqual(1, set.ElementAt(1));
            Assert.AreEqual(2, set.Count());
        }

        [Test]
        public void can_detect_proper_subset()
        {
            var set = new DistinctList<int> { 2 };
            var list = new int [] { 1, 2 };
            Assert.AreEqual(true, set.IsProperSubsetOf(list));
        }

        [Test]
        public void can_detect_proper_superset()
        {
            var set = new DistinctList<int> { 1, 2 };
            var list = new int[] { 1 };
            Assert.AreEqual(true, set.IsProperSupersetOf(list));
        }

        [Test]
        public void can_detect_subset()
        {
            var set = new DistinctList<int> { 1, 3};
            var list = new int[] { 1, 2, 3 };
            Assert.AreEqual(true, set.IsSubsetOf(list));
        }

        [Test]
        public void can_detect_subset_when_equal()
        {
            var set = new DistinctList<int> { 1, 2, 3};
            var list = new int[] { 1, 2, 3 };
            Assert.AreEqual(true, set.IsSubsetOf(list));
        }

        [Test]
        public void can_detect_superset()
        {
            var set = new DistinctList<int> { 1, 2, 3 };
            var list = new int[] { 1, 3 };
            Assert.AreEqual(true, set.IsSupersetOf(list));
        }

        [Test]
        public void can_detect_superset_when_equal()
        {
            var set = new DistinctList<int> { 1, 2, 3 };
            var list = new int[] { 1, 2, 3 };
            Assert.AreEqual(true, set.IsSupersetOf(list));
        }
    }
}
