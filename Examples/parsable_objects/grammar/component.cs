using System;
using System.Collections.Generic;
using System.Reflection;

namespace parsable_objects
{
  internal class component
  {
    public lexeme_set firsts;
    public bool       maybe_empty;
    
    protected component()
    {
      firsts      = new lexeme_set();
      maybe_empty = false;
    }
    
    public virtual void analyse()
    {
    }
    
    public virtual void parse(source_reader source)
    {
    }
    
    public delegate FieldInfo get_field_info();
    
    public virtual void unparse_object(parsable o, get_field_info get_field, source_builder source)
    {
    }
    
    public virtual void unparse(source_builder source)
    {
      source.write("???");
    }
  }
}
