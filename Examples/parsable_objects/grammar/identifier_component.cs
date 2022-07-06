using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace parsable_objects
{
  internal class identifier_component: component
  { 
    public identifier_component(): base()
    {
      maybe_empty = false;
      firsts.insert(this);
    }

    public override void parse(source_reader source)
    {
      if (source.symbol_is(typeof(identifier)))
      {
        parsable.elements.Push(source.current);
        source.next_symbol();
      }
      else parsable.error("Missing identifier");
    }
    
    public override void unparse_object(parsable o, FieldInfo field, source_builder source)
    {
      object value = field.GetValue(o);
      source.write(((identifier)value).spelling);
    }

    public override void unparse(source_builder source)
    {
      source.write("<identifier>");
    }
  }
}
