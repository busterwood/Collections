# MoreLinq
More linq style extensions methods.

# UniqueList<T>

`UniqueList` is a generic collection that:
* does not contain nulls
* only contains unqiue values
* is ordered, i.e. enumeration returns items in the order they were added

`UniqueList` is a mixture of an array-list of values and a hash set, and implements both `IList<T>` and `ISet<T>`.

The design of `UniqueList` was inspired by [Python 3.6's new dict ](https://mail.python.org/pipermail/python-dev/2012-December/123028.html) which uses hashcode to access an array of indexes into the values array of values.

## Add Performance

`Add` performance is similar to a adding to a `HashSet<T>` and not that much slower than `List<T>`, for example adding a number strings:
```
BenchmarkDotNet=v0.10.8, OS=Windows 10 Redstone 1 (10.0.14393)
Processor=AMD A4-6210 APU with AMD Radeon R3 Graphics, ProcessorCount=4
Frequency=1754521 Hz, Resolution=569.9561 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2101.1
  DefaultJob : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2101.1


     Method | Size |        Mean |       Error |      StdDev |
----------- |----- |------------:|------------:|------------:|
 UniqueList |   50 |    29.79 us |   0.5882 us |   0.5777 us |
  ArrayList |   50 |    22.06 us |   0.4055 us |   0.3793 us |
    HashSet |   50 |    28.81 us |   0.5722 us |   0.5353 us |
 UniqueList |  500 |   327.05 us |   6.4500 us |  11.1260 us |
  ArrayList |  500 |   234.22 us |   4.6396 us |   5.6979 us |
    HashSet |  500 |   304.02 us |   5.9085 us |   9.3715 us |
 UniqueList | 5000 | 3,350.63 us |  70.5560 us |  58.9174 us |
  ArrayList | 5000 | 2,478.81 us |  56.2821 us |  64.8145 us |
    HashSet | 5000 | 5,107.88 us | 100.1957 us | 143.6977 us |
```
## Contains / IndexOf performance

`Contains` performance is similar to a `HashSet<T>` and *much* faster than `List<T>`, for example lookup up all the strings in a list or set of different sizes:

BenchmarkDotNet=v0.10.8, OS=Windows 10 Redstone 1 (10.0.14393)
Processor=AMD A4-6210 APU with AMD Radeon R3 Graphics, ProcessorCount=4
Frequency=1754521 Hz, Resolution=569.9561 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2101.1
  DefaultJob : Clr 4.0.30319.42000, 64bit RyuJIT-v4.7.2101.1


     Method | Size |          Mean |         Error |        StdDev |
----------- |----- |--------------:|--------------:|--------------:|
 UniqueList |   50 |      23.60 us |     0.4546 us |     0.4668 us |
  ArrayList |   50 |      56.86 us |     1.1189 us |     1.6401 us |
    HashSet |   50 |      24.01 us |     0.4411 us |     0.4126 us |
 UniqueList |  500 |     243.04 us |     4.6906 us |     5.0189 us |
  ArrayList |  500 |   3,697.55 us |    73.0742 us |    68.3536 us |
    HashSet |  500 |     246.22 us |     4.4495 us |     3.7155 us |
 UniqueList | 5000 |   2,716.73 us |    52.8352 us |    56.5331 us |
  ArrayList | 5000 | 350,595.46 us | 6,076.2343 us | 5,386.4229 us |
    HashSet | 5000 |   2,750.42 us |    16.3910 us |    11.8517 us |

## Memory usage

In comparision to a `List<T>` on a x64 system:

```
Data structure | Size | Memory Held |
-------------- |----- |------------ |  
UniqueList     | 50   |       1.0 K |
ArrayList      | 50   |       0.6 K |
UniqueList     | 500  |       8.5 K |
ArrayList      | 500  |       4.5 K |
UniqueList     | 5000 |       139 K |
ArrayList      | 5000 |        73 K |
```

### Ideas

Null == empty set
Value == set of one
