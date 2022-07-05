using System.Reflection;

namespace parsable_objects
{
  internal class string_literal_component: component
  { 
    public string_literal_component(): base()
    {
      maybe_empty = false;
      firsts.insert(this);
    }

    public override void parse(source_reader source)
    {
      if (source.symbol_is(typeof(string_literal)))
      {
        parsable.elements.Push(((string_literal)source.current).spelling);
        source.next_symbol();
      }
      else parsable.error("Missing string literal");
    }
    
    public override void unparse_object(parsable o, FieldInfo field, source_builder source)
    {
      object value = field.GetValue(o);
      source.write("\"" + (string)value + "\"");
    }

    public override void unparse(source_builder source)
    {
      source.write("<string_literal>");
    }
  }
}
