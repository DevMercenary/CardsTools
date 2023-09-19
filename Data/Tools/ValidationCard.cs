using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsTools.Data.Tools
{
    public static class ValidationCard
    {
        public static bool IsStringNull(string str)
        {
            return string.IsNullOrEmpty(str);
        }
        public static bool IsNumeric(string input)
        {
            bool isNumeric = int.TryParse(input, out _);
            return isNumeric;
        }
    }
}
