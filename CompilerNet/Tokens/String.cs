using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class String: Token
    {
        public static string structure = "\"([^\"])*\"";
        public String(string newValue)
        {
            value = newValue;
        }

        public override string translate(int indentIndex)
        {
            return "\"" + value + "\"";
        }

        public static String checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            } else
            {
                return new String(input.Trim('"'));
            }
        }
    }
}
