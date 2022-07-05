using System.Reflection;

namespace parsable_objects
{
  internal class char_literal_component: component
  { 
    public char_literal_component(): base()
    {
      maybe_empty = false;
      firsts.insert(this);
    }

    public override void parse(source_reader source)
    {
      if (source.symbol_is(typeof(char_literal)))
      {
        char c = ((char_literal)source.current).spelling[0];
        parsable.elements.Push(c);
        source.next_symbol();
      }
      else parsable.error("Missing char literal");
    }
    
    public override void unparse_object(parsable o, FieldInfo field, source_builder source)
    {
      object value = field.GetValue(o);
      source.write("'" + (char)value + "'");
    }

    public override void unparse(source_builder source)
    {
      source.write("<char_literal>");
    }
  }
}
