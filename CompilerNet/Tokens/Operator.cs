using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Operator: Token
    {
        public static string structure = "(" + Add.structure + "|" + Minus.structure + "|" + Divide.structure + "|" + Multiply.structure + ")";
        public Operator(Token newValue)
        {
            value = newValue;
        }

        public override string translate(int indentIndex)
        {
            return value.translate(indentIndex);
        }

        public static Operator checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            }
            Token testValue = Add.checkToken(input);
            if (testValue == null)
            {
                testValue = Minus.checkToken(input);
                if (testValue == null)
                {
                    testValue = Divide.checkToken(input);
                    if (testValue == null)
                    {
                        testValue = Multiply.checkToken(input);
                        if (testValue == null)
                        {
                            return null;
                        }
                    }
                }
            }
            return new Operator(testValue);
        }
    }
}
