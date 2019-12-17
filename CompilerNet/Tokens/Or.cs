using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Or: Token
    {
        public static string structure = "^or (" + Operation.structure + "|" + Identifier.structure + "|" + String.structure + "|" + Int.structure + ") (is less than|is less than or equal to|is equal to|is greater than or equal to|is greater than) (" + Operation.structure + "|" + Identifier.structure + "|" + String.structure + "|" + Int.structure + "),$";
        public Token parameterOne;
        public Token parameterTwo;
        public string comparator;
        public Or(Token ParameterOne, string Comparator, Token ParameterTwo)
        {
            parameterOne = ParameterOne;
            comparator = Comparator;
            parameterTwo = ParameterTwo;
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
            string output = prefixStringWithTab("else if (" + parameterOne.translate(indentIndex) + " " + comparator + " " + parameterTwo.translate(indentIndex) + ") {\n", indentIndex);
            foreach (Token child in children)
            {
                output = output + child.translate(indentIndex + 1) + "\n";
            }
            output = output + prefixStringWithTab("}", indentIndex);
            return output;
        }

        public static Or checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            }

            string testComp = matches[0].Groups[8].Value;
            if (testComp != "is less than" && testComp != "is less than or equal to" && testComp != "is equal to" && testComp != "is greater than or equal to" && testComp != "is greater than")
            {
                return null;
            } else
            {
                switch (testComp)
                {
                    case "is less than":
                        testComp = "<";
                        break;
                    case "is less than or equal to":
                        testComp = "<=";
                        break;
                    case "is equal to":
                        testComp = "==";
                        break;
                    case "is greater than or equal to":
                        testComp = ">=";
                        break;
                    case "is greater than":
                        testComp = ">";
                        break;
                }

                string testParam1Str = matches[0].Groups[1].Value;
                Token testParam1 = Operation.checkToken(testParam1Str);
                if (testParam1 == null)
                {
                    testParam1 = String.checkToken(testParam1Str);
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
                }

                string testParam2Str = matches[0].Groups[9].Value;
                Token testParam2 = Operation.checkToken(testParam2Str);
                if (testParam2 == null)
                {
                    testParam2 = String.checkToken(testParam2Str);
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
                }

                return new Or(testParam1, testComp, testParam2);
            }
        }
    }
}
