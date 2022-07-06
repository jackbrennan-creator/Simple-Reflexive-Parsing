using System;

namespace parsable_objects
{
  public class parse_error: Exception
  { 
    public enum error_kind {grammar, parser, source_builder};
    
    public error_kind kind { get; private set; }
    
    public parse_error(string message, error_kind kind): base(message)
    {
      this.kind = kind;
    }
  } 
}
