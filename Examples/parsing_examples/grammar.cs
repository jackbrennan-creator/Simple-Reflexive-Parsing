using System.Collections.Generic;
using parsable_objects;

namespace grammar
{
  partial class grammar: parsable
  {
    [Parse(1)] private List<production> productions;
  }
  
  partial class production: parsable
  {
    [Parse(1, "<")  ] private punctuation       less;
    [Parse(2)       ] public  identifier        name;
    [Parse(3, ">")  ] private punctuation       greater;
    [Parse(4, "::=")] private punctuation       defines;
    [Parse(5, "|")  ] private List<alternative> alternatives;
    [Parse(6, ";")  ] private punctuation       semicolon;
  }
  
  partial class alternative: parsable
  {
    [Parse(1)] private List<component> components;
  }
  
  partial class component: parsable
  {
    partial class symbol: component
    {
      [Parse(1)] private string spelling;
    }
    
    partial class reserved_word: component
    {
      [Parse(1)] private identifier id;
    }
    
    partial class non_terminal: component
    {
      [Parse(1, "<")  ] private punctuation  less;
      [Parse(2)       ] private identifier   name;
      [Parse(3, ">")  ] private punctuation  greater;
    }
    
    partial class option: component
    {
      [Parse(1, "[")] private punctuation sub;
      [Parse(2)     ] private component   component;
      [Parse(3, "]")] private punctuation bus;
    }
    
    partial class iteration: component
    {
      [Parse(1, "{")] private punctuation         open_brace;
      [Parse(2)     ] private component           component;
      [Parse(3)     ] private Optional<separator> separator;
      [Parse(4, "}")] private punctuation         close_brace;
    }
    
    partial class separator: parsable
    {
      [Parse(1)] public string spelling;
    }
  }
}
