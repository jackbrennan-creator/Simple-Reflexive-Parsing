using System;
using System.Reflection;

namespace parsable_objects
{
  internal class non_terminal_component: component
  {
    public Type       t;
    public production definition;
    
    public non_terminal_component(Type t): base()
    {
      this.t          = t;
      this.definition = null;
    }

    public override void analyse()
    {
      parsable.analyse(t);
      definition = parsable.productions[t.Name];
      if (definition.maybe_left_recursive && parsable.checking_left_recursion) 
        parsable.grammar_error(t.Name + " is left recursive");
      firsts.insert(definition);
    }

    public override void parse(source_reader source)
    {
      definition.parse(source);
    }
    
    public override void unparse_object(parsable o, FieldInfo field, source_builder source)
    {
      object value = field.GetValue(o);
      definition.unparse_object((parsable)value, source);
    }

    public override void unparse(source_builder source)
    {
      source.write("<" + t.Name + ">");
    }
  }
}
