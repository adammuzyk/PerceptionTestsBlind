using System.Linq;

namespace PerceptionTests.Helpers
{
    public static class Extensions
    {
        public static bool In<T>(this T self, params T[] values)
        {
            return values.Any(v => v.Equals(self));
        }
    }
}
