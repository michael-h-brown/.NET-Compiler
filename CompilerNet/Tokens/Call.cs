using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Call: Token
    {
        public static string structure = "(" + Identifier.structure + ") (of) (" + Identifier.structure + ")|(" + Identifier.structure + ") (with) (" + Operation.structure + "|" + Multiple_Identifiers.structure + "|" + Int.structure + "|" + String.structure + ").?|(" + Identifier.structure + ")";
        //public static string structure = "(" + Identifier.structure + ")(( (of) (" + Identifier.structure + "))| (with) (" + Multiple_Identifiers.structure + "|" + Int.structure + "|" + String.structure + "|" + Operation.structure + "))?.?";
        Identifier name;
        private bool isObjectCall;
        public Call(Identifier Name, Token newValue, bool newIsObjectCall = false)
        {
            name = Name;
            value = newValue;
            isObjectCall = newIsObjectCall;
        }

        public override string translate(int indentIndex)
        {
            if (isObjectCall)
            {
                return "(" + value.translate(indentIndex) + "." + name.translate(indentIndex) + ")";
            }
            else
            {
                return "(" + name.translate(indentIndex) + "(" + (value != null ? value.translate(indentIndex) : "") + ")" + ")";
            }
        }

        public static Call checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            }

            if (matches[0].Groups[2].Value == "of")
            {
                Identifier testName = Identifier.checkToken(matches[0].Groups[1].Value);
                if (testName == null)
                {
                    return null;
                }
                Identifier testValue = Identifier.checkToken(matches[0].Groups[3].Value);
                if (testValue == null)
                {
                    return null;
                }
                return new Call(testName, testValue, true);
            }
            else if (matches[0].Groups[5].Value == "with")
            {
                Identifier testName = Identifier.checkToken(matches[0].Groups[4].Value);
                if (testName == null || testName.isFunction == false)
                {
                    return null;
                }

                Token testValue = Operation.checkToken(matches[0].Groups[6].Value);
                if (testValue == null)
                {
                    testValue = Identifier.checkToken(matches[0].Groups[6].Value);
                    if (testValue == null)
                    {
                        testValue = String.checkToken(matches[0].Groups[6].Value);
                        if (testValue == null)
                        {
                            testValue = Int.checkToken(matches[0].Groups[6].Value);
                            if (testValue == null)
                            {
                                return null;
                            }
                        }
                    }
                }
                return new Call(testName, testValue);
            } else
            {
                Identifier testName = Identifier.checkToken(matches[0].Groups[4].Value);
                if (testName == null || testName.isFunction == false)
                {
                    return null;
                }

                return new Call(testName, null);
            }
        }
    }
}
