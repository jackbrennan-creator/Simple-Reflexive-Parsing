using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace parsable_objects
{
  internal class alternative
  {
    public List<component> components;
    public lexeme_set      firsts;
    public bool            maybe_empty;
    public Type            alternative_type;
    List<FieldInfo>        fields;
    
    public alternative(List<component> components, Type alternative_type)
    {
      this.components       = components;
      this.firsts           = new lexeme_set();
      this.maybe_empty      = false;
      this.alternative_type = alternative_type;
      this.fields           = parsable.get_parsable_fields(alternative_type);
    }
    
    public void analyse(production parent)
    {
      maybe_empty                 = true;
      parent.maybe_left_recursive = true;
      parsable.start_checking_for_left_recursion();
      foreach (var c in components)
      {
        c.analyse();
        if (maybe_empty)
        {
          firsts.insert(c);
          if (!c.maybe_empty)
          {
            maybe_empty                      = false;
            parent.maybe_left_recursive      = false;
            parsable.checking_left_recursion = false;
          }
        }
      }
      parent.maybe_left_recursive = false;
      parsable.end_checking_for_left_recursion();
    }
    
    public void parse(source_reader source)
    {
      foreach (var c in components) c.parse(source);
    }
    
    public void unparse_object(parsable o, source_builder source)
    {
      int current_field = 0;
      foreach (var c in components) 
      {
        if (current_field > 0) source.pad(" ");
        c.unparse_object(o, fields[current_field], source);
        current_field = current_field + 1;  
      }
    }

    public void unparse(source_builder source)
    {
      source.separate<component>(components, (c) => c.unparse(source), " ");
    }
  }
}
