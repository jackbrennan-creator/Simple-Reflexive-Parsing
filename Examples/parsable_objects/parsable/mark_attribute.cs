using System;

namespace parsable_objects
{
  [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
  public class Mark: Attribute
  {    
    public Mark()
    {
    }
  }
}
