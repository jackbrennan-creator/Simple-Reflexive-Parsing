# Reflexive Parsing and Unparsing for C# Objects

This library allows automatic parsing of text into C# objects and unparsing back into text. It has a number of applications including data transformation and implementing experimental programming languages.

The example program shows how it can be used to 
1. Parse and unparse a simple tree structure. 
2. Parse, unparse and evaluate a complex expression. 
1. Parse text representing an EBNF grammar and unparse it back into a fully formatted text.

![Parsing Examples](https://github.com/jackbrennan-creator/Reflexive-Parsing/blob/main/example_form.png)

The example solution contains both the example project and the parsable library project. The latter can be compiled into a standalone component for use with other projects.

The [User Manual](https://github.com/jackbrennan-creator/Reflexive-Parsing/blob/main/User%20Manual.pdf) PDF document should provide enough information to get started using the library.

The [Implementation Notes](https://github.com/jackbrennan-creator/Reflexive-Parsing/blob/main/Implementation%20Notes.pdf) (comments) document provides details of the internal implementation of the library.

## The tree example

Suppose we have a simple class representing a tree structure in which each node has a name represented by some type identifier, essentially a string, and a list of subtrees. 
```
class tree
{
  private identifier  name;
  private List<tree>  subtrees;
}
```
We wish to parse text similar to the following to generate instances of the class tree.

A tree with no subtrees.
```
root()
```
A tree with two subtrees, the second of which itself has two subtrees. Note: White space is ignored by the parser.                    
```
root( 
      a() 
      b( 
        b1() 
        b2() 
      ) 
    )
```
To achieve this we first make the tree class inherit from the class parsable, defined by the library, and decorate the original fields of the class with some attributes. Two extra “place holder” fields are required to indicate where punctuation symbols are required in the input text. The parsing process does not assign values to place holder fields.
```
using System.Collections.Generic;
using parsable_objects;

class tree: parsable
{
  [Parse(1)     ] private identifier  name;
  [Parse(2, "(")] private punctuation open;
  [Parse(3)     ] private List<tree>  subtrees;
  [Parse(4, ")")] private punctuation close;
}
```
The first parameter of the Parse attribute indicates the order in which fields are to be parsed as there is no clean, backwards comparable way of determining the order of declaration via reflection. Fields of type punctuation are used to indicate where fixed symbols are required in the source text. The actual symbol is specified in the Parse attribute. Given this new class definition, all that is required (minus some exception handling) to parse a file containing text in the format shown above is the following function.
```
using System.IO;
using parsable_objects;

tree parse_tree(FileStream file)
{
  return parsable.parse<tree>(new source_reader(file));
}
```
