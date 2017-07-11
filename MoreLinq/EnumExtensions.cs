﻿using System.Linq;

namespace BusterWood.MoreLinq
{
    public static class EnumExtensions
    {
        public static bool In<T>(this T value, params T[] @in) => @in.Any(i => value.Equals(i)); //NOTE: i will get boxed for enums
    }   
}