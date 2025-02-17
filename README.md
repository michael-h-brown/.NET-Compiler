# .NET-Compiler
A .NET Compiler that takes source code, converts it to C# then assembles and runs it

# Source Code Documentation

## General

The source code is very dynamic, but also very simplified. Indentation is used to parent functions, if statements and loops to whatever statements are inside. Statements end with a "." while statements that cause indentation (such as loops, if statements etc.) end with a ","

There are two data types:

  -String
  
  -Int

## Declaring Variables

Variables are dynamic and can be declared with the following:

    <identifier> is <Idenfier>|<String>|<Int>|<Operation>|<Call>
  
  Examples: 
  
      thisVariable is 1.
       x is "hello".

## Operations

Operators are different in this language, take a look:

    + | add
    - | minus
    / | divide by
    * | times

Examples: 

    sum is 2 add 3.
    div is 4 divide by 2.

## If/Else

 If statements take the following format:
 
    if <Operation>|<Identifier>|<String>|<Int> <is less than>|<is less than or equal to>|<is equal to>|<is greater than or equal to>|<is greater than> <Operation>|<Identifier>|<String>|<Int>,
      <statements>
    or <Operation>|<Identifier>|<String>|<Int> <is less than>|<is less than or equal to>|<is equal to>|<is greater than or equal to>|<is greater than> <Operation>|<Identifier>|<String>|<Int>,
      <statements>
    otherwuse,
      <statements>
      
As you can see "or" takes the place of "elif/else if" in other languages and "otherwise" takes the place of "else"
  
Example:

    if x is greater than or equal to 5,
      <statements>
    or x is less than 4,
      <statements>
    otherwise,
      <statements>

## Loops

There are no while loops in the language (this may change in the future). But for loops are available and take the following format:

    loop <Operation>|<Identifier>|<String>|<Int> times <backwards> with <Identifier>,
      <statements>

Using a string in a loop makes no sense (and does not work) and should be removed in the future (or changed to automatically use the length).

The "backwards" is optional and allows for a loop to be run backwards
  
Examples:
    
    loop 10 times backwards with i,
      <statement>
      
     OR
     
    loop 10 times with i,
      <statement>
      
In the first example i would go from 9 to 0. In the second i would go from 0 to 9.

## Functions

## Input/Output

IO is very simple in the language (being based around a Windows Console Application). <read> is used for input, while <write> is used for output. They use the following format: 
  
      read <Identifier> from <Identifier>|<String>.
      write <Operation>|<Call>|<Identifier>|<String>|<Int> to <Identifier>|<String>.
      
The "read" statement has an optional Identifier (used to store input as a variable), and the "from <Identifier/String>" is optional as way to read a SINGLE LINE from a file (Note: an identifier must be specified to read from a file)
  
The "write" statement requires something the output (the first group of items), but the "to <Identifier/String>" is optional as a way to write a SINGLE LINE to a file
  
Examples:

    read.
    read input.
    read fileInput from "inputFile.txt".
    
    write output.
    write fileOutput to "outputFile.txt".

# Command Line Arguments

By default (if no argument is provided), the program runs a shell for you to enter your source code into, it will not print the generated C# code and will not run it once compiled. But you can change these options using command line arguments:

    --s/--shell: run the shell and enter source code
    --f/--filepath {path-to-file}: use the file {path-to-file} as the source code
    --p/--print: print the C# code that is generated
    --r/--run: Run the code once it is compiled
