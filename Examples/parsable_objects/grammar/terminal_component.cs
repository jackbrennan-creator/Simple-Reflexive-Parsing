using System;
using System.Reflection;

namespace parsable_objects
{
  internal class terminal_component: component
  {
    public  string spelling;
    private string raw_spelling;
    
    public terminal_component(string spelling): base()
    {
      this.raw_spelling = spelling;
      this.spelling     = spelling.Trim();
      this.maybe_empty  = false;
      firsts.insert(this);
      parsable.add_symbol(this.spelling);
    }

    public override void parse(source_reader source)
    {
      if (!source.accept(typeof(punctuation), spelling)) parsable.error("Missing " + spelling);
    }
    
    public override void unparse_object(parsable o, FieldInfo field, source_builder source)
    {
      source.write(raw_spelling);
    }

    public override void unparse(source_builder source)
    {
      source.write("\"" + raw_spelling + "\"");
    }
  }
}
