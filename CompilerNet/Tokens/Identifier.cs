using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

namespace CompilerNet.Tokens
{
    class Identifier: Token
    {
        public static string structure = "\\b\\w*\\b";
        public static List<Identifier> identifiersInUse = new List<Identifier>();
        public bool assigned;
        public string name;
        public bool isFunction;
        public Identifier(string newName, string newValue=null)
        {
            name = newName;
            value = newValue;
            assigned = false;
            isFunction = false;
        }

        public void setValue(dynamic newValue)
        {
            value = newValue;
        }

        public override string translate(int indentIndex)
        {
            return name;
        }

        public void makeFunction()
        {
            isFunction = true;
        }

        public static Identifier checkToken(string input)
        {
            var regex = new Regex(structure, RegexOptions.Compiled);
            MatchCollection matches = regex.Matches(input);
            if (matches.Count > 0)
            {
                bool found = false;
                Identifier currentIdentifier = null; ;
                foreach (Identifier currentVar in identifiersInUse)
                {
                    if (currentVar.name == input)
                    {
                        found = true;
                        currentIdentifier = currentVar;
                        break;
                    }
                }
                if (found)
                {
                    return currentIdentifier;
                }
                else
                {
                    currentIdentifier = new Identifier(input);
                    identifiersInUse.Add(currentIdentifier);
                    return currentIdentifier;
                }
            } else
            {
                return null;
            }
        }
    }
}
