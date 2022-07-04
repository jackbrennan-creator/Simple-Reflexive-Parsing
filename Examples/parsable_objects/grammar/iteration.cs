using System.Collections.Generic;

namespace parsable_objects
{
  internal class Iteration<t>: parsable where t: parsable
  {
    private List<t>   value;
    private component separator;
    
    public Iteration()
    {
      value     = new List<t>();
      separator = null;
    }
    
    public Iteration(string separator)
    {
      this.value     = new List<t>();
      this.separator = sym(separator);
    }
    
    public void Add(t value)
    {
      this.value.Add(value);
    }
    
    public List<t> Value
    {
      get { return this.value; }
    }
    
    public object get_value()
    {
      return this.value;
    }
    
    internal override void parsed(Stack<object> elements, int alternative)
    {
      value.Add((t)elements.Pop());
    }
  }
}
