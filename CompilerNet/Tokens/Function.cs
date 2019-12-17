using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Function: Token
    {
        public Identifier functionName;
        public Multiple_Identifiers functionParameter;
        public static string structure = "^(" + Identifier.structure + ") is a function( (with) (" + Multiple_Identifiers.structure + ")|func (" + Identifier.structure + "))?,$";
        public Function(Identifier Name, Multiple_Identifiers Parameter=null)
        {
            functionName = Name;
            if (Parameter != null)
            {
                functionParameter = Parameter;
            }
            isReturned = false;
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
            isReturned = checkReturned();
            string output = prefixStringWithTab("public static " + (isReturned ? "dynamic" : "void") + " " + functionName.translate(indentIndex).Replace("()", "") + (functionParameter != null ? "(" + functionParameter.translate(indentIndex) + ") " : "() ") + "{\n", indentIndex);
            if (this.hasChildren())
            {
                foreach (Token child in children)
                {
                    output = output + child.translate(indentIndex+1) + "\n";
                }
            }
            output = output + prefixStringWithTab("}", indentIndex);
            return output;
        }

        public static Function checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            }
            Identifier testName;
            Multiple_Identifiers testParameter;

                testName = Identifier.checkToken(matches[0].Groups[1].Value.Trim(','));
                if (testName == null)
                {
                    return null;
                } else
                {
                    if (matches[0].Groups[3].Value == "with")
                    {
                        testParameter = Multiple_Identifiers.checkToken(matches[0].Groups[4].Value.Trim(','));
                        if (testParameter == null)
                        {
                            return null;
                        } else
                        {
                            testName.makeFunction();
                            return new Function(testName, testParameter);
                        }
                    } else
                    {
                        testName.makeFunction();
                        return new Function(testName);
                    }
                }
        }
    }
}
