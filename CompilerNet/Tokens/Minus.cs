using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Minus: Token
    {
        public static string structure = "minus";
        public Minus()
        {
        }

        public override string translate(int indentIndex)
        {
            return "-";
        }

        public static Minus checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            } else
            {
                return new Minus();
            }
        }
    }
}
