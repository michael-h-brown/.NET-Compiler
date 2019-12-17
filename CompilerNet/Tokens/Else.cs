using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Else: Token
    {
        public static string structure = "^otherwise,$";
        public Else()
        {
        }

        public string prefixStringWithTab(string input, int tabCount)
        {
            string output = "";
            for (int i = 0; i < tabCount; i++)
            {
                output = output + "\t";
            }
            return output + input;
        }

        public override string translate(int indentIndex)
        {
            string output = prefixStringWithTab("else {\n", indentIndex);
            foreach (Token child in children)
            {
                output = output + child.translate(indentIndex + 1) + "\n";
            }
            output = output + prefixStringWithTab("}", indentIndex);
            return output;
        }

        public static Else checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            }

            return new Else();
        }
    }
}
