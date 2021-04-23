using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp.RuntimeBinder;

using CompilerNet.Tokens;

namespace CompilerNet
{
    class CSCode
    {
        private string code;
        public CSCode()
        {
            code = "";
        }

        public void addCode(string line)
        {
            line = line + "\n";
            code = code + line;
        }

        public string getCode()
        {
            return code;
        }
    }
    class Program
    {
        static string shell()
        {
            string sourceCode = "";
            string input = Console.ReadLine();
            while (input != "run")
            {
                sourceCode += input + "\n";
                input = Console.ReadLine();
            }
            sourceCode = sourceCode.Trim();
            return sourceCode;
        }

        static string fromFile()
        {
            StreamReader reader = new StreamReader("sourceCode.txt");
            string sourceCode = "";
            string line = reader.ReadLine();
            while (line != null)
            {
                sourceCode += line + "\n";
                line = reader.ReadLine();
            }
            reader.Close();
            sourceCode = sourceCode.Trim();
            return sourceCode;
        }

        static void Main(string[] args)
        {
            //string sourceCode = shell();
            string sourceCode = fromFile();
            Console.Clear();

            Console.WriteLine("GOT SOURCE CODE: ");
            Console.WriteLine(sourceCode);
            Console.WriteLine("\n");

            string[] lines = sourceCode.Split('\n');
            List<Token> mainTokens = new List<Token>();
            List<Token> functionTokens = new List<Token>();
            List<string> requiredLibraries = new List<string> { "System" };
            int indentIndex = 0;
            Stack<Token> indentParent = new Stack<Token>();

            foreach (string line in lines)
            {
                bool checkLineIndent = true;
                bool isFunction = false;
                bool isIndentation = false;
                int thisIndent = line.Count(x => x == '\t');
                string thisLine = line.Trim();
                Token testToken = Function.checkToken(thisLine);
                if (testToken == null)
                {
                    testToken = Assignment.checkToken(thisLine);
                    if (testToken == null)
                    {
                        testToken = Input.checkToken(thisLine);
                        if (testToken == null)
                        {
                            testToken = Output.checkToken(thisLine);
                            if (testToken == null)
                            {
                                testToken = Loop.checkToken(thisLine);
                                if (testToken == null)
                                {
                                    testToken = If.checkToken(thisLine);
                                    if (testToken == null)
                                    {
                                        testToken = Or.checkToken(thisLine);
                                        if (testToken == null)
                                        {
                                            testToken = Else.checkToken(thisLine);
                                            if (testToken == null)
                                            {
                                                testToken = Return.checkToken(thisLine);
                                                if (testToken == null)
                                                {
                                                    Console.WriteLine("Error: null token");
                                                    Console.ReadLine();
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                isIndentation = true;
                                            }
                                        }
                                        else
                                        {
                                            isIndentation = true;
                                        }
                                    }
                                    else
                                    {
                                        isIndentation = true;
                                    }
                                }
                                else
                                {
                                    isIndentation = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (indentParent.Count == 0)
                    {
                        indentIndex += 1;
                        checkLineIndent = false;
                        indentParent.Push(testToken);
                        isFunction = true;
                    }
                    else
                    {
                        Console.WriteLine("Error: cannot nest func");
                        Console.ReadLine();
                        return;
                    }
                }

                if (testToken.hasLibraries())
                {
                    List<string> newLibraries = testToken.getLibraries();
                    foreach(string library in newLibraries)
                    {
                        if (!requiredLibraries.Contains(library))
                        {
                            requiredLibraries.Add(library);
                        }
                    }
                }

                if (indentIndex != 0 && checkLineIndent)
                {
                    if (thisIndent > indentIndex)
                    {
                        Console.WriteLine("Error: too many indents");
                        Console.ReadLine();
                        return;
                    }
                    else if (thisIndent == indentIndex)
                    {
                        Token parent = indentParent.Peek();
                        parent.addChild(testToken);
                        if (isIndentation)
                        {
                            indentParent.Push(testToken);
                            indentIndex += 1;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < (indentIndex - thisIndent); i++)
                        {
                            indentParent.Pop();
                        }
                        indentIndex -= (indentIndex - thisIndent);
                        if (indentParent.Count != 0)
                        {
                            indentParent.Peek().addChild(testToken);
                            if (isIndentation)
                            {
                                indentParent.Push(testToken);
                                indentIndex += 1;
                            }
                        }
                        else
                        {
                            mainTokens.Add(testToken);
                            if (isIndentation)
                            {
                                indentParent.Push(testToken);
                                indentIndex += 1;
                            }
                        }
                    }
                }
                else
                {
                    if (isFunction)
                    {
                        functionTokens.Add(testToken);
                    }
                    else if (isIndentation)
                    {
                        if (indentIndex > thisIndent || thisIndent == 0)
                            mainTokens.Add(testToken);
                        indentParent.Push(testToken);
                        indentIndex += 1;
                    }
                    else
                    {
                        mainTokens.Add(testToken);
                    }
                }

            }

            Console.WriteLine("\nOUTPUT\n");

            CSCode csCode = new CSCode();

            foreach(string library in requiredLibraries)
            {
                csCode.addCode("using " + library + ";");
            }

            csCode.addCode("namespace Program {");
            csCode.addCode("\tclass Program {");

            foreach (Token tkn in functionTokens)
            {
                csCode.addCode(tkn.translate(2));
            }

            csCode.addCode("\t\tpublic static void Main(string[] args) {");
            foreach (Token tkn in mainTokens)
            {
                csCode.addCode(tkn.translate(3));
            }
            csCode.addCode("\t\t}");

            csCode.addCode("\t}");
            csCode.addCode("}");

            string code = csCode.getCode();

            Console.WriteLine(code);

            MethodInfo main;

            try
            {
                CSharpCodeProvider provider = new CSharpCodeProvider();
                CompilerParameters parameters = new CompilerParameters(new string[] { "Microsoft.CSharp.dll", "System.Core.dll" }, "Program.exe");
                parameters.GenerateInMemory = true;
                parameters.GenerateExecutable = true;

                CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

                if (results.Errors.HasErrors)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (CompilerError error in results.Errors)
                    {
                        sb.AppendLine(string.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                    }

                    throw new InvalidOperationException(sb.ToString());
                }

                Assembly assembly = results.CompiledAssembly;
                Type program = assembly.GetType("Program.Program");
                main = program.GetMethod("Main");
            }
            catch (Exception e)
            {
                Console.WriteLine("Compilation Error: " + e.Message);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nSuccessfully Compiled");

            try
            {
                Console.WriteLine("Running...\n");
                main.Invoke(new[] { "Microsoft.CSharp.dll", "System.Core.dll" }, new Object[] { null });
            }
            catch (Exception e)
            {
                Console.WriteLine("Runtime Error: " + e.Message);
                Console.ReadLine();
                return;
            }

            Console.WriteLine("\nProgram Finished...");
            Console.ReadLine();
        }
    }
}
