using System;

namespace parsable_objects
{
  [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
  public class Parse: Attribute
  {
    internal int      order        { get; private set; }
    internal string   spelling     { get; private set; }
    internal string[] alternatives { get; private set; }
    
    public Parse(int order)
    {
      this.order = order;
    }
    
    public Parse(int order, string spelling)
    {
      this.order    = order;
      this.spelling = spelling;
    }
    
    public Parse(int order, params string[] alternatives)
    {
      this.order        = order;
      this.spelling     = "";
      this.alternatives = alternatives;
    }
  }
}
