using BusterWood.Collections;
using NUnit.Framework;
using System.Linq;

namespace UnitTests
{
    [TestFixture]
    public class UniqueListTests
    {
        [Test]
        public void can_add_unique_value()
        {
            var set = new UniqueList<int>();
            Assert.AreEqual(true, set.Add(1), "set.Add(1)");
            Assert.AreEqual(1, set.Count, "set.Count");
        }

        [Test]
        public void can_add_unique_strings()
        {
            var set = new UniqueList<string>();
            Assert.AreEqual(true, set.Add(1+"a"));
            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void cannot_add_duplicate_values()
        {
            var set = new UniqueList<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(false, set.Add(1));
            Assert.AreEqual(1, set.Count);
        }

        [Test]
        public void can_add_unique_value2()
        {
            var set = new UniqueList<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Add(2));
            Assert.AreEqual(2, set.Count);
        }

        [Test]
        public void can_fill_set()
        {
            var set = new UniqueList<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Add(2));
            Assert.AreEqual(true, set.Add(3));
            Assert.AreEqual(3, set.Count);
        }

        [Test]
        public void can_resize_set()
        {
            var set = new UniqueList<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Add(2));
            Assert.AreEqual(true, set.Add(3));
            Assert.AreEqual(true, set.Add(4));
            Assert.AreEqual(true, set.Add(5));
            Assert.AreEqual(5, set.Count);
        }

        [Test]
        public void can_access_items_by_index_of_order_added()
        {
            var set = new UniqueList<int>();
            Assert.AreEqual(true, set.Add(3));
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Add(2));
            Assert.AreEqual(3, set[0]);
            Assert.AreEqual(1, set[1]);
            Assert.AreEqual(2, set[2]);
        }

        [Test]
        public void enumerates_items_in_the_order_they_were_added()
        {
            var set = new UniqueList<int>();
            Assert.AreEqual(true, set.Add(3));
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Add(2));
            Assert.AreEqual(3, set.ElementAt(0));
            Assert.AreEqual(1, set.ElementAt(1));
            Assert.AreEqual(2, set.ElementAt(2));
        }

        [Test]
        public void contains_item_we_have_added()
        {
            var set = new UniqueList<int>();
            Assert.AreEqual(true, set.Add(3));
            Assert.AreEqual(true, set.Contains(3));
        }

        [Test]
        public void does_not_contain_item_we_have_not_added()
        {
            var set = new UniqueList<int>();
            Assert.AreEqual(true, set.Add(1));
            Assert.AreEqual(true, set.Add(2));
            Assert.AreEqual(true, set.Add(4));
            Assert.AreEqual(false, set.Contains(3));
        }

        [Test]
        public void can_delete_item_that_exists()
        {
            var set = new UniqueList<int> { 1, 2, 3, 4 };
            Assert.AreEqual(true, set.Remove(3));

            Assert.AreEqual(0, set.IndexOf(1), "set.IndexOf(1)");
            Assert.AreEqual(1, set.IndexOf(2), "set.IndexOf(2)");
            Assert.AreEqual(2, set.IndexOf(4), "set.IndexOf(4)");
            Assert.AreEqual(-1, set.IndexOf(3), "set.IndexOf(3)");
            Assert.AreEqual(3, set.Count, "count");
        }

        [Test]
        public void can_delete_last_item()
        {
            var set = new UniqueList<int> { 1, 2, 3, 4 };
            Assert.AreEqual(true, set.Remove(4));

            Assert.AreEqual(0, set.IndexOf(1), "set.IndexOf(1)");
            Assert.AreEqual(1, set.IndexOf(2), "set.IndexOf(2)");
            Assert.AreEqual(2, set.IndexOf(3), "set.IndexOf(3)");
            Assert.AreEqual(-1, set.IndexOf(4), "set.IndexOf(4)");
            Assert.AreEqual(3, set.Count, "count");
        }

        [Test]
        public void can_add_same_value_after_deleting()
        {
            var set = new UniqueList<int> { 1, 2, 3, 4 };
            Assert.AreEqual(true, set.Remove(4));
            Assert.AreEqual(true, set.Add(4));

            Assert.AreEqual(4, set.Count, "count");
            Assert.AreEqual(0, set.IndexOf(1), "set.IndexOf(1)");
            Assert.AreEqual(1, set.IndexOf(2), "set.IndexOf(2)");
            Assert.AreEqual(2, set.IndexOf(3), "set.IndexOf(3)");
            Assert.AreEqual(3, set.IndexOf(4), "set.IndexOf(4)");
        }

        [Test]
        public void can_add_same_value_into_deleted_slot()
        {
            Assert.AreEqual(3.GetHashCode() % 7, 10.GetHashCode() % 7);

            var set = new UniqueList<int> { 1, 2, 3, 4 };
            Assert.AreEqual(true, set.Remove(3));
            Assert.AreEqual(true, set.Add(10));

            Assert.AreEqual(4, set.Count, "count");
            Assert.AreEqual(0, set.IndexOf(1), "set.IndexOf(1)");
            Assert.AreEqual(1, set.IndexOf(2), "set.IndexOf(2)");
            Assert.AreEqual(2, set.IndexOf(4), "set.IndexOf(3)");
            Assert.AreEqual(3, set.IndexOf(10), "set.IndexOf(10)");
        }


    }
}