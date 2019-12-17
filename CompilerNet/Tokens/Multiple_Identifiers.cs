using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Multiple_Identifiers: Token
    {
        public static string structure = "(" + Identifier.structure + "(, " + Identifier.structure + ")* and " + Identifier.structure + "|" + Identifier.structure + ")";
        public List<Identifier> identifiers;
        public Multiple_Identifiers(List<Identifier> newIdentifiers)
        {
            identifiers = newIdentifiers;
        }

        public override string translate(int indentIndex)
        {
            string output = "";
            foreach (Identifier id in identifiers)
            {
                output = output + "dynamic " + id.translate(indentIndex) + ", ";
            }
            output = output.Trim().Trim(',');

            return output;
        }

        public static Multiple_Identifiers checkToken(string input)
        {
            var regex = new Regex(structure, RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(input);
            if (matches.Count > 0)
            {
                string inputGroup = matches[0].Groups[0].Value.Replace("and", ",");
                string[] idStrings = inputGroup.Split(',');
                List<Identifier> newIdentifiers = new List<Identifier>();
                for (int i = 0; i < idStrings.Length; i++)
                {
                    Identifier newIdentifier = Identifier.checkToken(idStrings[i].Trim());
                    if (newIdentifier == null)
                    {
                        return null;
                    }
                    newIdentifiers.Add(newIdentifier);
                }

                return new Multiple_Identifiers(newIdentifiers);
            } else
            {
                return null;
            }
        }
    }
}
