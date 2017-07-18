# MoreLinq
More linq style extensions methods.

# UniqueList<T>

`UniqueList` is a generic collection that:
* does not contain nulls
* only contains unqiue values
* is ordered, i.e. enumeration returns items in the order they were added

`UniqueList` is a mixture of an array-list of values and a hash set, and implements both `IList<T>` and `ISet<T>`.

The design of `UniqueList` was inspired by [Python 3.6's new dict ](https://mail.python.org/pipermail/python-dev/2012-December/123028.html) which uses hashcode to access an array of indexes into the values array of values.

`Add` performance is similar to a adding to a `HashSet<T>` and not that much slower than `List<T>`, for example adding a number strings:
```
BenchmarkDotNet=v0.10.8, OS=Windows 10 Redstone 1 (10.0.14393)
Processor=AMD A4-6210 APU with AMD Radeon R3 Graphics, ProcessorCount=4
Frequency=1754521 Hz, Resolution=569.9561 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2101.1
  DefaultJob : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2101.1


     Method | Size |        Mean |       Error |      StdDev |
----------- |----- |------------:|------------:|------------:|
  ArrayList |   50 |    22.06 us |   0.4055 us |   0.3793 us |
 UniqueList |   50 |    29.79 us |   0.5882 us |   0.5777 us |
    HashSet |   50 |    28.81 us |   0.5722 us |   0.5353 us |
  ArrayList |  500 |   234.22 us |   4.6396 us |   5.6979 us |
 UniqueList |  500 |   327.05 us |   6.4500 us |  11.1260 us |
    HashSet |  500 |   304.02 us |   5.9085 us |   9.3715 us |
  ArrayList | 5000 | 2,478.81 us |  56.2821 us |  64.8145 us |
 UniqueList | 5000 | 3,350.63 us |  70.5560 us |  58.9174 us |
    HashSet | 5000 | 5,107.88 us | 100.1957 us | 143.6977 us |
```



### Ideas

Null == empty set
Value == set of one
