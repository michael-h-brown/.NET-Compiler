using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Operation: Token
    {
        public static string structure = "(" + Identifier.structure + "|" + String.structure + "|" + Int.structure + ") (add|minus|divide by|times) (" + Identifier.structure + "|" + String.structure + "|" + Int.structure + ")";
        Token parameterOne;
        Token parameterTwo;
        Operator op;
        public Operation(Token ParameterOne, Operator Op, Token ParameterTwo)
        {
            parameterOne = ParameterOne;
            op = Op;
            parameterTwo = ParameterTwo;
        }

        public override string translate(int indentIndex)
        {
            return "(" + parameterOne.translate(indentIndex) + " " + op.translate(indentIndex) + " " + parameterTwo.translate(indentIndex) + ")";
        }

        public static Operation checkToken(string input)
        {
            Regex regex = new Regex("^" + structure + "$", RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            }

            string testOpStr = matches[0].Groups[3].Value;
            Operator testOp = Operator.checkToken(testOpStr);
            if (testOp == null)
            {
                return null;
            }
            else
            {
                string testParam1Str = matches[0].Groups[1].Value;
                Token testParam1 = String.checkToken(testParam1Str);
                if (testParam1 == null)
                {
                    testParam1 = Int.checkToken(testParam1Str);
                    if (testParam1 == null)
                    {
                        testParam1 = Identifier.checkToken(testParam1Str);
                        if (testParam1 == null)
                        {
                            return null;
                        }
                    }
                }
                string testParam2Str = matches[0].Groups[4].Value;
                Token testParam2 = String.checkToken(testParam2Str);
                if (testParam2 == null)
                {
                    testParam2 = Int.checkToken(testParam2Str);
                    if (testParam2 == null)
                    {
                        testParam2 = Identifier.checkToken(testParam2Str);
                        if (testParam2 == null)
                        {
                            return null;
                        }
                    }
                }
                return new Operation(testParam1, testOp, testParam2);
            }
        }
    }
}
