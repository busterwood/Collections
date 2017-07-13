# MoreLinq
More linq style extensions methods.

# Ideas

Null == empty set

Value == set of one

Set of N but preserving order:
* tried array set with linear lookup - too slow
* try [Python3-style ordered dictionary](https://mail.python.org/pipermail/python-dev/2012-December/123028.html), i.e use hashcode to access an array of indexes into array of values, values stored in order added so can be enumerated in sequnece.

Why set of N? Avoid duplicates and nulls (safer, avoiding errors)
