# .NET-Compiler
A .NET Compiler that takes source code, converts it to C# then assembles and runs it

# Source Code Documentation (TODO)

## General

The source code is very dynamic, but also very simplified. Indentation is used to parent functions, if statements and loops to whatever statements are inside. Statements end with a "." while statements that cause indentation (such as loops, if statements etc.) end with a ","

There are two data types:
  -String
  -Int

## Declaring Variables

Variables are dynamic and can be declared with the following:

<identifier> is <Idenfier>|<String>|<Int>|<Operation>|<Call>
  
  e.g. 
  
      "thisVariable is 1.",
       "x is "hello"."

## Operations

Operators are different in this language, take a look:

    + | add
    - | minus
    / | divide by
    * | times

Examples: 

    "sum is 2 add 3.",
    "div is 4 divide by 2."

## If/Else

## Loops

## Functions

## Input/Output

# Command Line Arguments

By default (if no argument is provided), the program runs a shell for you to enter your source code into, it will not print the generated C# code and will not run it once compiled. But you can change these options using command line arguments:

--s/--shell: run the shell and enter source code

--f/--filepath {path-to-file}: use the file {path-to-file} as the source code

--p/--print: print the C# code that is generated

--r/--run: Run the code once it is compiled
