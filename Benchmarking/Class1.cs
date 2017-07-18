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
            //BenchmarkRunner.Run<IntLists>();
            //BenchmarkRunner.Run<StringLists>();
            //BenchmarkRunner.Run<UniqueList<int>>();
            BenchmarkRunner.Run<ContainsStringLists>();
        }
    }

    //[MemoryDiagnoser]
    //[InliningDiagnoser]
    public class IntLists
    {
        [Params(50, 500, 5000)]
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
        [Params(50, 500, 5000)]
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

    public class ContainsStringLists
    {
        private int _size;

        [Params(50, 500, 5000)]
        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            ul = new UniqueList<string>();
            for (int i = 0; i < Size; i++)
                ul.Add(i + "abv");

            l = new List<string>();
            for (int i = 0; i < Size; i++)
                l.Add(i + "abv");

            hs = new HashSet<string>();
            for (int i = 0; i < Size; i++)
                hs.Add(i + "abv");
        }

        UniqueList<string> ul;
        List<string> l;
        HashSet<string> hs;

        [Benchmark]
        public bool UniqueList()
        {
            bool c = true;
            for (int i = 0; i < Size; i++)
                c = c | ul.Contains(i + "abv");
            return c;
        }

        [Benchmark]
        public bool ArrayList()
        {
            bool c = true;
            for (int i = 0; i < Size; i++)
                c = c | l.Contains(i + "abv");
            return c;
        }

        [Benchmark]
        public bool HashSet()
        {
            bool c = true;
            for (int i = 0; i < Size; i++)
                c = c | hs.Contains(i + "abv");
            return c;

        }
    }

}
