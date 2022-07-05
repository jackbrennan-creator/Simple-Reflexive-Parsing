using System.Collections.Generic;
using parsable_objects;

namespace tree
{  
  class tree: parsable
  {
    [Parse(1)     ] private identifier  name;
    [Parse(2, "(")] private punctuation open;
    [Parse(3)     ] private List<tree>  subtrees;
    [Parse(4, ")")] private punctuation close;
  }
}
