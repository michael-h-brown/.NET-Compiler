using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Input: Token
    {
        public static string structure = "^read( (" + Identifier.structure + ")( from (" + Identifier.structure + "|" + String.structure + "))?)?.$";
        public Token filePath;

        public Input(Identifier newValue=null, Token newFilePath=null)
        {
            if (newValue != null)
            {
                value = newValue;
                if (newFilePath != null)
                {
                    filePath = newFilePath;
                    requiredLibraries.Add("System.IO");
                }
            }
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
            if (value != null)
            {
                if (filePath != null)
                {
                    string output = "";
                    if (!value.assigned) {
                        output = output + prefixStringWithTab("dynamic " + value.translate(indentIndex) + ";\n", indentIndex);
                    }
                    output = output + prefixStringWithTab("using (StreamReader reader = new StreamReader(" + filePath.translate(indentIndex) + ")) {\n", indentIndex);
                    output = output + prefixStringWithTab("\t" + value.translate(indentIndex) + " = reader.ReadLine();\n", indentIndex);
                    output = output + prefixStringWithTab("}", indentIndex);
                    return output;
                }
                else
                {
                    if (value.assigned)
                    {
                        return prefixStringWithTab(value.translate(indentIndex) + " = Console.ReadLine();", indentIndex);
                    }
                    else
                    {
                        return prefixStringWithTab("dynamic " + value.translate(indentIndex) + " = Console.ReadLine();", indentIndex);
                    }
                }
            } else
            {
                return prefixStringWithTab("Console.ReadLine();", indentIndex);
            }
        }

        public static Input checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            } else
            {
                dynamic testVal;


                testVal = Identifier.checkToken(matches[0].Groups[2].Value.Trim('.'));
                if (testVal == null)
                {
                    return new Input();
                }
                else
                {
                    dynamic testPath;
                    testPath = String.checkToken(matches[0].Groups[4].Value);
                    if (testPath == null)
                    {
                        testPath = Identifier.checkToken(matches[0].Groups[4].Value);
                        if (testPath == null)
                        {
                            return new Input(testVal);
                        }
                    }
                    return new Input(testVal, testPath);
                }
            }
        }
    }
}
