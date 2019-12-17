using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Multiply: Token
    {
        public static string structure = "times";
        public Multiply()
        {
        }

        public override string translate(int indentIndex)
        {
            return "*";
        }

        public static Multiply checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            } else
            {
                return new Multiply();
            }
        }
    }
}
