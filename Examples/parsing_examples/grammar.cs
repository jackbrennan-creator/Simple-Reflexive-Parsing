using System.Linq;
using System.Collections.Generic;
using parsable_objects;

namespace grammar
{
  class grammar: parsable, unpasable
  {
    [Parse(1)] private List<production> productions;

    public void unparse(source_builder source)
    {
      int width = productions.Max((p) => ("<" + p.name.spelling + ">").Length);
      source.add_column(width);
        source.separate_lines(productions, (p) => p.unparse(source), 2);
      source.remove_column();
    }
  }
  
  class production: parsable, unpasable
  {
    [Parse(1, "<")  ] private punctuation       less;
    [Parse(2)       ] public  identifier        name;
    [Parse(3, ">")  ] private punctuation       greater;
    [Parse(4, "::=")] private punctuation       defines;
    [Parse(5, "|")  ] private List<alternative> alternatives;
    [Parse(6, ";")  ] private punctuation       semicolon;

    public void unparse(source_builder source)
    {
      source.write("<" + name.spelling + ">");
      source.next_column();
      
      source.write(" ::= ");
      source.separate(alternatives, (a) => a.unparse(source), " | ");
      source.write(";");
      
      source.previous_column();
    }
  }
  
  class alternative: parsable, unpasable
  {
    [Parse(1)] private List<component> components;

    public void unparse(source_builder source)
    {
      source.separate(components, (c) => c.unparse(source), " ");
    }
  }
  
  class component: parsable, unpasable
  {
    class symbol: component
    {
      [Parse(1)] private string spelling;

      public override void unparse(source_builder source)
      {
        source.write("\"" + spelling + "\"");
      }
    }
    
    class reserved_word: component
    {
      [Parse(1)] private identifier id;

      public override void unparse(source_builder source)
      {
        source.write(id.spelling);
      }
    }
    
    class non_terminal: component
    {
      [Parse(1, "<")  ] private punctuation  less;
      [Parse(2)       ] private identifier   name;
      [Parse(3, ">")  ] private punctuation  greater;

      public override void unparse(source_builder source)
      {
        source.write("<");
        source.write(name.spelling);
        source.write(">");
      }
    }
    
    class option: component
    {
      [Parse(1, "[")] private punctuation sub;
      [Parse(2)     ] private component   component;
      [Parse(3, "]")] private punctuation bus;

      public override void unparse(source_builder source)
      {
        source.write("[ ");
        component.unparse(source);
        source.write(" ]");
      }
    }
    
    class iteration: component
    {
      [Parse(1, "{")] private punctuation         open_brace;
      [Parse(2)     ] private component           component;
      [Parse(3)     ] private Optional<separator> separator;
      [Parse(4, "}")] private punctuation         close_brace;

      public override void unparse(source_builder source)
      {
        source.write("{ ");
        component.unparse(source);
        if (separator.Defined)
        {
          source.write(" ");
          separator.Value.unparse(source);
        }
        source.write(" }");
      }
    }
    
    class separator: parsable, unpasable
    {
      [Parse(1)] public string spelling;

      public void unparse(source_builder source)
      {
        source.write("\"" + spelling + "\"");
      }
    }

    public virtual void unparse(source_builder source)
    {
    }
  }
}
