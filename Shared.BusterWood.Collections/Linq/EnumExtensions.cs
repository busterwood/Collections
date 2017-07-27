using System.Linq;

namespace BusterWood.Linq
{
#if UNIQUELIST_INTERNAL
    internal
#else
    public
#endif
    static class EnumExtensions
    {
        public static bool In<T>(this T value, params T[] @in) => @in.Any(i => value.Equals(i)); //NOTE: i will get boxed for enums
    }   
}