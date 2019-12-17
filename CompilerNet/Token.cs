using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompilerNet.Tokens;

namespace CompilerNet
{
    abstract class Token
    {
        protected List<Token> children;
        protected dynamic value;
        protected bool isReturned;
        protected List<string> requiredLibraries;
        abstract public string translate(int indentIndex);

        public Token()
        {
            children = new List<Token>();
            requiredLibraries = new List<string>();
            isReturned = false;
        }

        public void addChild(Return newChild)
        {
            isReturned = true;
            children.Add(newChild);
        }

        public void addChild(Token newChild)
        {
            children.Add(newChild);
        }

        public bool hasChildren()
        {
            return children.Count != 0;
        }

        public dynamic getValue()
        {
            return value;
        }

        public bool hasLibraries()
        {
            return requiredLibraries.Count > 0;
        }

        public List<string> getLibraries()
        {
            return requiredLibraries;
        }

        public bool checkReturned()
        {
            if (hasChildren())
            {
                foreach (Token child in children)
                {
                    if (child.checkReturned())
                    {
                        return true;
                    }
                }
            }
            if (isReturned)
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
