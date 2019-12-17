using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Return: Token
    {
        public static string structure = "^return (" + Operation.structure + "|" + Call.structure + "|" + Identifier.structure + "|" + String.structure + "|" + Int.structure + ").$";
        public Return(Token newValue)
        {
            isReturned = true;
            value = newValue;
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
            return prefixStringWithTab("return " + value.translate(indentIndex) + ";", indentIndex);
        }

        public static Return checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            } 
            Token testVal = Operation.checkToken(matches[0].Groups[1].Value);
            if (testVal == null)
            {
                testVal = String.checkToken(matches[0].Groups[1].Value);
                if (testVal == null)
                {
                    testVal = Identifier.checkToken(matches[0].Groups[1].Value);
                    if (testVal == null)
                    {
                        testVal = Int.checkToken(matches[0].Groups[1].Value);
                        if (testVal == null)
                        {
                            return null;
                        }
                    }
                }
            }
            return new Return(testVal);
        }
    }
}
