# MoreLinq
More linq style extensions methods.

# UniqueList<T>

`UniqueList` is a generic collection that:
* does not contain nulls
* only contains unqiue values
* is ordered, i.e. enumeration returns items in the order they were added

`UniqueList` is a mixture of an array-list of values and a hash set, and implements both `IList<T>` and `ISet<T>`.

`Add` performance is similar to a `HashSet<T>`

```
BenchmarkDotNet=v0.10.8, OS=Windows 7 SP1 (6.1.7601)
Processor=Intel Core i7-2600S CPU 2.80GHz (Sandy Bridge), ProcessorCount=8
Frequency=2728359 Hz, Resolution=366.5207 ns, Timer=TSC
  [Host]     : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1590.0
  DefaultJob : Clr 4.0.30319.42000, 64bit RyuJIT-v4.6.1590.0


     Method | Size |         Mean |        Error |       StdDev |
----------- |----- |-------------:|-------------:|-------------:|
 UniqueList |   50 |   1,810.0 ns |    36.258 ns |    72.411 ns |
  ArrayList |   50 |     391.8 ns |     7.841 ns |     8.389 ns |
    HashSet |   50 |   1,484.9 ns |    29.185 ns |    57.609 ns |
 UniqueList |  500 |  13,958.3 ns |   224.520 ns |   187.485 ns |
  ArrayList |  500 |   3,798.1 ns |    38.844 ns |    36.334 ns |
    HashSet |  500 |  13,569.4 ns |   263.744 ns |   333.552 ns |
 UniqueList | 5000 | 128,424.6 ns | 2,416.686 ns | 2,142.329 ns |
  ArrayList | 5000 |  25,287.7 ns |   470.556 ns |   417.136 ns |
    HashSet | 5000 | 140,679.7 ns | 2,649.324 ns | 2,212.305 ns |
```



### Ideas

Null == empty set

Value == set of one

Set of N but preserving order:
* tried array set with linear lookup - too slow
* try [Python3-style ordered dictionary](https://mail.python.org/pipermail/python-dev/2012-December/123028.html), i.e use hashcode to access an array of indexes into array of values, values stored in order added so can be enumerated in sequnece.

Why set of N? Avoid duplicates and nulls (safer, avoiding errors)
