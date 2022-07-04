using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace parsable_objects
{
  internal class iterated_component<t>: component where t: parsable, new()
  {
    public component iterated;
    public component seperator;
    
    public iterated_component(component iterated): base()
    {
      this.iterated    = iterated;
      this.maybe_empty = true;
      this.seperator   = null;
    }
    
    public iterated_component(component iterated, string seperator): base()
    {
      this.iterated    = iterated;
      this.maybe_empty = true;
      if (source_reader.is_id_string(seperator.Trim()))
        this.seperator = parsable.word(seperator);
      else
        this.seperator = parsable.sym(seperator);
    }

    public override void analyse()
    {
      maybe_empty = true;
      iterated.analyse();
      if (iterated.maybe_empty) parsable.grammar_error("Iterated component may not have an empty alternative");
      firsts.insert(iterated.firsts);
    }

    public override void parse(source_reader source)
    {
      t   result     = new t();
      int iterations = 0;
      while (firsts.can_accept(source) || (iterations > 0 && seperator != null && seperator.firsts.can_accept(source)))
      {
        if (seperator != null && iterations > 0) seperator.parse(source);
        iterated.parse(source);
        result.parsed(parsable.elements, 1);
        iterations = iterations + 1;
      }
      parsable.elements.Push(result);
    }
    
    public override void unparse_object(parsable o, get_field_info get_field, source_builder source)
    {
      FieldInfo      field        = get_field();
      Type           field_type   = field.FieldType;
      object         value        = field.GetValue(o);
      Type           element_type = field_type.GetGenericArguments()[0];
      production     element      = parsable.productions[element_type.Name];
      int            i            = 0;
      foreach (parsable e in (IList)value)
      {
        if (i > 0) if (seperator != null) seperator.unparse_object(o, () => field, source); else source.pad(" ");
        element.unparse_object(e, source);
        i = i + 1;
      }
    }

    public override void unparse(source_builder source)
    {
      source.write("{ "); 
        iterated.unparse(source); 
        if (seperator != null)
        {
          source.write(" ");
          seperator.unparse(source);
        }
      source.write(" }");
    }
  }
}
