using System;
using System.Collections.Generic;

namespace parsable_objects
{
  public class Optional<t>: parsable where t: parsable
  {
    private t    value;
    private bool assigned;
    
    public Optional()
    {
      value    = null;
      assigned = false;
    }
    
    internal void Assign(t value)
    {
      this.value    = value;
      this.assigned = true;
    }
    
    public bool Defined
    {
      get { return this.assigned; }
    }
    
    public t Value
    {
      get { if (assigned) return this.value; else throw new Exception("Attempt to access undefined Optional value"); }
    }

    internal override void parsed(Stack<object> elements, int alternative)
    {
      this.Assign((t)elements.Pop());
    }
  }
}
