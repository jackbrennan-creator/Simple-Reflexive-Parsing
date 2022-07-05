using System.Reflection;

namespace parsable_objects
{
  internal class number_component: component
  { 
    public number_component(): base()
    {
      maybe_empty = false;
      firsts.insert(this);
    }

    public override void parse(source_reader source)
    {
      if (source.symbol_is(typeof(number)))
      {
        parsable.elements.Push(int.Parse(((number)source.current).spelling));
        source.next_symbol();
      }
      else parsable.error("Missing number");
    }
    
    public override void unparse_object(parsable o, FieldInfo field, source_builder source)
    {
      object value = field.GetValue(o);
      source.write((int)value);
    }

    public override void unparse(source_builder source)
    {
      source.write("<number>");
    }
  }
}
