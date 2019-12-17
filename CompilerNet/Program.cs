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
            Console.Write("1| ");
            int lineCount = 1;
            string input = Console.ReadLine();
            while (input != "run")
            {
                ++lineCount;
                sourceCode += input + "\n";
                Console.Write(lineCount.ToString() + "| ");
                input = Console.ReadLine();
            }
            sourceCode = sourceCode.Trim();
            return sourceCode;
        }

        static string fromFile(string FilePath)
        {
            //OG: sourceCode.txt
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(FilePath);
                string sourceCode = "";
                string line = reader.ReadLine();
                while (line != null)
                {
                    sourceCode += line + "\n";
                    line = reader.ReadLine();
                }
                reader.Close();
                reader = null;
                sourceCode = sourceCode.Trim();
                return sourceCode;
            } catch (Exception e)
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                Console.WriteLine(e);
                return "ERROR";
            }
        }

        static string[] shellArgs = new string[]
        {
            "--shell",
            "--s"
        };

        static string[] fileArgs = new string[]
        {
            "--filepath",
            "--f",
        };

        static void Main(string[] args)
        {
            string sourceCode = "";
            List<int> commandIndexes = new List<int>();
            bool useShell = true;
            string filePath = "";
            for (int i = 0; i < args.Length; ++i)
            {
                if (args[i].Substring(0, 2) == "--")
                {
                    commandIndexes.Add(i);
                }
            }

            foreach (int index in commandIndexes)
            {
                if (shellArgs.Contains(args[index]))
                {
                    useShell = true;
                    break;
                }
                else if (fileArgs.Contains(args[index]))
                {
                    if (index < args.Length - 1)
                    {
                        useShell = false;
                        filePath = args[index + 1];
                    }
                    break;
                }
            }

            if (useShell)
            {
                sourceCode = shell();
            }
            else
            {
                sourceCode = fromFile(filePath);
            }

            if (sourceCode == "ERROR")
            {
                Console.WriteLine("There was an error reading the file");
                Console.ReadLine();
                return;
            }

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
            int lineNo = 1;

            foreach (string line in lines)
            {
                bool checkLineIndent = true;
                bool isFunction = false;
                bool isIndentation = false;
                int thisIndent = line.Count(x => x == '\t');
                string thisLine = line.Trim();

                List<int> indentIndexes = new List<int>
                {
                    3, 4, 5, 6
                };

                Token testToken = Function.checkToken(thisLine);
                if (testToken == null)
                {
                    List<Token> testTokens = new List<Token>
                    {
                        Assignment.checkToken(thisLine),
                        Input.checkToken(thisLine),
                        Output.checkToken(thisLine),
                        Loop.checkToken(thisLine),
                        If.checkToken(thisLine),
                        Or.checkToken(thisLine),
                        Else.checkToken(thisLine),
                        Return.checkToken(thisLine)
                    };

                    bool tokenFound = false;
                    for (int i = 0; i < testTokens.Count; i++)
                    {
                        if (testTokens[i] != null)
                        {
                            tokenFound = true;
                            testToken = testTokens[i];
                            break;
                        }
                    }
                    if (!tokenFound)
                    {
                        Console.WriteLine("No matching token found for line " + lineNo.ToString() + ": " + thisLine);
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
                ++lineNo;
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
