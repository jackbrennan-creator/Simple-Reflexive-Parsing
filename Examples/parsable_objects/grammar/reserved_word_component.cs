using System;
using System.Reflection;

namespace parsable_objects
{
  internal class reserved_word_component: component
  { 
    public  string spelling;
    private string raw_spelling;
    
    public reserved_word_component(string spelling): base()
    {
      this.raw_spelling = spelling;
      this.spelling     = spelling.Trim();
      this.maybe_empty  = false;
      firsts.insert(this);
      parsable.add_word(this.spelling);
    }

    public override void parse(source_reader source)
    {
      if (!source.accept(typeof(reserved_word))) parsable.error("Missing " + spelling);
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
