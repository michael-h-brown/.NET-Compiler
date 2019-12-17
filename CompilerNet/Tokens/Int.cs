using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Int: Token
    {
        public static string structure = "\\d+";
        public Int(int newValue)
        {
            value = newValue;
        }

        public override string translate(int indentIndex)
        {
            return value.ToString();
        }

        public static Int checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            } else
            {
                return new Int(int.Parse(input));
            }
        }
    }
}
