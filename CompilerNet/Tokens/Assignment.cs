using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Assignment: Token
    {
        public static string structure = "^(" + Identifier.structure + ") is (" + Identifier.structure + "|" + String.structure + "|" + Int.structure + "|" + Operation.structure + "|" + Call.structure + ").$";
        public Identifier variableName;
        public Assignment(Identifier variable, Token NewValue)
        {
            variableName = variable;
            value = NewValue;
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
            if (variableName.assigned)
            {
                return prefixStringWithTab(variableName.translate(indentIndex) + " = " + value.translate(indentIndex) + ";", indentIndex);
            } else
            {
                variableName.assigned = true;
                variableName.setValue(value.translate(indentIndex));
                return prefixStringWithTab("dynamic " + variableName.translate(indentIndex) + " = " + value.translate(indentIndex) + ";", indentIndex);
            }
        }

        public static Assignment checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            }
            Identifier testName = Identifier.checkToken(matches[0].Groups[1].Value);
            if (testName == null)
            {
                return null;
            }
            Token testAssign = Call.checkToken(matches[0].Groups[2].Value.Trim('.'));
            if (testAssign == null)
            {
                testAssign = Operation.checkToken(matches[0].Groups[2].Value.Trim('.'));
                if (testAssign == null)
                {
                    testAssign = Identifier.checkToken(matches[0].Groups[2].Value.Trim('.'));
                    if (testAssign == null)
                    {
                        testAssign = String.checkToken(matches[0].Groups[2].Value.Trim('.'));
                        if (testAssign == null)
                        {
                            testAssign = Int.checkToken(matches[0].Groups[2].Value.Trim('.'));
                            if (testAssign == null)
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            return new Assignment(testName, testAssign);

        }
    }
}
