using System.Linq;
using parsable_objects;

namespace grammar
{
  partial class grammar: unpasable
  {
    public void unparse(source_builder source)
    {
      int width = productions.Max((p) => ("<" + p.name.spelling + ">").Length);
      source.add_column(width);
        source.separate_lines(productions, (p) => p.unparse(source), 2);
      source.remove_column();
    }
  }
  
  partial class production: unpasable
  {
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
  
  partial class alternative: unpasable
  {
    public void unparse(source_builder source)
    {
      source.separate(components, (c) => c.unparse(source), " ");
    }
  }
  
  partial class component: unpasable
  {
    partial class symbol: component
    {
      public override void unparse(source_builder source)
      {
        source.write("\"" + spelling + "\"");
      }
    }
    
    partial class reserved_word: component
    {
      public override void unparse(source_builder source)
      {
        source.write(id.spelling);
      }
    }
    
    partial class non_terminal: component
    {
      public override void unparse(source_builder source)
      {
        source.write("<");
        source.write(name.spelling);
        source.write(">");
      }
    }
    
    partial class option: component
    {
      public override void unparse(source_builder source)
      {
        source.write("[ ");
        component.unparse(source);
        source.write(" ]");
      }
    }
    
    partial class iteration: component
    {
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
    
    partial class separator: unpasable
    {
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

