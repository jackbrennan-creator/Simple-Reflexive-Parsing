using System.Reflection;

namespace parsable_objects
{
  internal class real_component: component
  { 
    public real_component(): base()
    {
      maybe_empty = false;
      firsts.insert(this);
    }

    public override void parse(source_reader source)
    {
      if (source.symbol_is(typeof(real_number)))
      {
        parsable.elements.Push(double.Parse(((real_number)source.current).spelling));
        source.next_symbol();
      }
      else parsable.error("Missing real number");
    }
    
    public override void unparse_object(parsable o, FieldInfo field, source_builder source)
    {
      object value = field.GetValue(o);
      source.write((double)value);
    }

    public override void unparse(source_builder source)
    {
      source.write("<real number>");
    }
  }
}
