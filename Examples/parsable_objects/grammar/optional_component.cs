using System;
using System.Reflection;

namespace parsable_objects
{
  internal class optional_component<T>: component where T: parsable, new()
  {
    public component option;
    
    public optional_component(component option): base()
    {
      this.option      = option;
      this.maybe_empty = true;
    }

    public override void analyse()
    {
      maybe_empty = true;
      option.analyse();
      if (option.maybe_empty) parsable.grammar_error("Optional component may not have an empty alternative");
      firsts.insert(option.firsts);
    }

    public override void parse(source_reader source)
    {
      T result = new T();
      if (firsts.can_accept(source))
      {
        option.parse(source);
        result.parsed(parsable.elements, 1);
      }
      parsable.elements.Push(result);
    }
    
    public override void unparse_object(parsable o, FieldInfo field, source_builder source)
    {
      object       value         = field.GetValue(o);
      Type         field_type    = field.FieldType;
      Type         optional_type = field_type.GetGenericArguments()[0];
      PropertyInfo defined       = field_type.GetProperty("Defined");
      bool         is_defined    = (bool)defined.GetValue(value, new object[]{});
      if (is_defined)
      {
        production   option         = parsable.productions[optional_type.Name];
        PropertyInfo value_property = field_type.GetProperty("Value");
        parsable     optional_value = (parsable)value_property.GetValue(value, new object[]{});
        option.unparse_object(optional_value, source);
      }
    }

    public override void unparse(source_builder source)
    {
      source.write("[ "); option.unparse(source); source.write(" ]");
    }
  }
}
