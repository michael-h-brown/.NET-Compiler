using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Output: Token
    {
        public static string structure = "write (" + Operation.structure + "|" + Call.structure + "|" + Identifier.structure + "|" + String.structure + "|" + Int.structure + ")( (to) (" + Identifier.structure + "|" + String.structure + "))?.";
        public Token filePath;
        public Output(Token newValue, Token newFilePath=null)
        {
            value = newValue;
            if (newFilePath != null)
            {
                filePath = newFilePath;
                requiredLibraries.Add("System.IO");
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
            if (filePath == null)
            {
                return prefixStringWithTab("Console.WriteLine(" + value.translate(indentIndex) + ".ToString());", indentIndex);
            }
            else
            {
                string output = prefixStringWithTab("using (StreamWriter writer = new StreamWriter(" + filePath.translate(indentIndex) + ")) {\n", indentIndex);
                output = output + prefixStringWithTab("\twriter.WriteLine(" + value.translate(indentIndex) + ");\n", indentIndex);
                output = output + prefixStringWithTab("}", indentIndex);
                return output;
            }
        }

        public static Output checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            } 
            Token testVal = Call.checkToken(matches[0].Groups[1].Value.Trim('.'));
            if (testVal == null)
            {
                testVal = Operation.checkToken(matches[0].Groups[1].Value.Trim('.'));
                if (testVal == null)
                {
                    testVal = Identifier.checkToken(matches[0].Groups[1].Value.Trim('.'));
                    if (testVal == null)
                    {
                        testVal = String.checkToken(matches[0].Groups[1].Value.Trim('.'));
                        if (testVal == null)
                        {
                            testVal = Int.checkToken(matches[0].Groups[1].Value.Trim('.'));
                            if (testVal == null)
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            if (matches[0].Groups[21].Value == "to")
            {
                Token testPath = Identifier.checkToken(matches[0].Groups[22].Value.Trim());
                if (testPath == null)
                {
                    testPath = String.checkToken(matches[0].Groups[22].Value.Trim());
                    if (testPath == null)
                    {
                        return null;
                    }
                }
                return new Output(testVal, testPath);
            } else
            {
                return new Output(testVal);
            }

            //Filepath

        }
    }
}
