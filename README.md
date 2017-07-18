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


     Method | Size |        Mean |      Error |     StdDev |
----------- |----- |------------:|-----------:|-----------:|
 UniqueList |   50 |    28.93 us |  0.5756 us |  0.5103 us |
  ArrayList |   50 |    21.53 us |  0.4255 us |  0.4553 us |
    HashSet |   50 |    27.73 us |  0.4608 us |  0.4085 us |
 UniqueList |  500 |   320.39 us |  6.1743 us |  5.7755 us |
  ArrayList |  500 |   219.66 us |  3.8685 us |  3.6186 us |
    HashSet |  500 |   298.25 us |  5.9494 us | 10.1026 us |
 UniqueList | 5000 | 3,202.48 us | 60.7205 us | 56.7980 us |
  ArrayList | 5000 | 2,418.89 us | 49.0234 us | 48.1475 us |
    HashSet | 5000 | 4,963.08 us | 78.7155 us | 65.7310 us |
```



### Ideas

Null == empty set
Value == set of one
