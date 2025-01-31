using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace parsable_objects
{
  public struct source_position
  {
    public readonly int start;
    public readonly int length;
    
    public source_position(int start, int length)
    {
      this.start  = start;
      this.length = length;
    } 
  }
}
