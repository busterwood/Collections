using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BusterWood.Collections;

namespace Benchmarking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Lists>();
        }
    }

    public class Lists
    {
        [Params(10, 30, 60, 100)]
        public int Size { get; set; }

        [Benchmark]
        public DistinctList<int> Distinct()
        {
            var l = new DistinctList<int>();
            for (int i = 0; i < Size; i++)
                l.Add(i);
            return l;
        }

        [Benchmark]
        public OHashSet<int> OHashSet()
        {
            var l = new OHashSet<int>();
            for (int i = 0; i < Size; i++)
                l.Add(i);
            return l;
        }

        [Benchmark]
        public List<int> ArrayList()
        {
            var l = new List<int>();
            for (int i = 0; i < Size; i++)
                l.Add(i);
            return l;
        }

        //[Benchmark]
        public HashSet<int> HashSet()
        {
            var l = new HashSet<int>();
            for (int i = 0; i < Size; i++)
                l.Add(i);
            return l;
        }
    }
}
