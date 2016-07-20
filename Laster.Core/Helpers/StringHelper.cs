using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Laster.Core.Helpers
{
    public class StringHelper
    {
        public static bool LikeString(string input, string pattern)
        {
            if (input == pattern) return true;
            if (string.IsNullOrEmpty(pattern)) return false;

            return Operators.LikeString(input, pattern, CompareMethod.Text);
        }
    }
}