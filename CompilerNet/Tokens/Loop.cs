using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Loop: Token
    {
        public Identifier indexer;
        public Token number;
        public bool backwards;
        public static string structure = "^loop (" + Operation.structure + "|" + Identifier.structure + "|" + String.structure + "|" + Int.structure + ") times (backwards )?with (" + Identifier.structure + "),$";
        public Loop(Identifier Indexer, Token Number, bool Backwards)
        {
            indexer = Indexer;
            number = Number;
            backwards = Backwards;
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
            string output = prefixStringWithTab("for (int " + indexer.translate(indentIndex) + " = " + (backwards ? number.translate(indentIndex) : "0") + "; " + indexer.translate(indentIndex) + " " + (backwards ? ">= 0" : "< " + number.translate(indentIndex)) + "; " + indexer.translate(indentIndex) + (backwards ? "--" : "++") + ") {\n", indentIndex);
            foreach (Token child in children)
            {
                output = output + child.translate(indentIndex + 1) + "\n";
            }
            output = output + prefixStringWithTab("}", indentIndex);
            return output;
        }

        public static Loop checkToken(string input)
        {
            Regex regex = new Regex(structure, RegexOptions.Compiled);
            var matches = regex.Matches(input);
            if (matches.Count == 0)
            {
                return null;
            }

            Identifier testIndexer = Identifier.checkToken(matches[0].Groups[9].Value.Trim(','));
            if (testIndexer == null)
            {
                return null;
            }
            Token testNumber = Operation.checkToken(matches[0].Groups[1].Value);
            if (testNumber == null)
            {
                testNumber = Int.checkToken(matches[0].Groups[1].Value);
                if (testNumber == null)
                {
                    return null;
                }
            } 
            string backwardsGroup = matches[0].Groups[8].Value;
            return new Loop(testIndexer, testNumber, backwardsGroup == "backwards ");
        }
    }
}
