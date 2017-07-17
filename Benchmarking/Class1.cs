using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BusterWood.Collections;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

namespace Benchmarking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<IntLists>();
            //BenchmarkRunner.Run<StringLists>();
            //BenchmarkRunner.Run<UniqueList<int>>();
        }
    }

    //[MemoryDiagnoser]
    //[InliningDiagnoser]
    public class IntLists
    {
        [Params(5000)]
        public int Size { get; set; }

        [Benchmark]
        public UniqueList<int> UniqueList()
        {
            var l = new UniqueList<int>();
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

        [Benchmark]
        public HashSet<int> HashSet()
        {
            var l = new HashSet<int>();
            for (int i = 0; i < Size; i++)
                l.Add(i);
            return l;
        }
    }

    public class StringLists
    {
        [Params(10, 50, 500, 5000)]
        public int Size { get; set; }

        [Benchmark]
        public UniqueList<string> UniqueList()
        {
            var l = new UniqueList<string>();
            for (int i = 0; i < Size; i++)
                l.Add(i + "abv");
            return l;
        }

        [Benchmark]
        public List<string> ArrayList()
        {
            var l = new List<string>();
            for (int i = 0; i < Size; i++)
                l.Add(i + "abv");
            return l;
        }

        [Benchmark]
        public HashSet<string> HashSet()
        {
            var l = new HashSet<string>();
            for (int i = 0; i < Size; i++)
                l.Add(i + "abv");
            return l;
        }
    }
}
