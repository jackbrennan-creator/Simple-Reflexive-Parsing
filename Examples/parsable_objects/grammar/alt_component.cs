using System;
using System.Linq;
using System.Reflection;

namespace parsable_objects
{
  internal class alt_component: component
  { 
    public string   [] symbols;
    public Type        t;
    public component[] alternatives;
    public int         default_index;
    
    public alt_component(string[] symbols, Type t): base()
    {
      maybe_empty  = symbols.Contains("");
      this.symbols = symbols;
      this.t       = t;
      this.default_index = Array.FindIndex(symbols, (s) => s == "");
      alternatives = symbols.Select((s) => source_reader.is_id_string(s) ? (component)new reserved_word_component(s) : (component)new terminal_component(s)).ToArray(); 
      for (int i = 0; i < alternatives.Length; i = i + 1)
        if (symbols[i] != "")
        {
          component c = alternatives[i];
          firsts.insert(c);
          if (c is terminal_component)
            parsable.add_symbol(symbols[i]);
          else
            parsable.add_word(symbols[i]);
        }
    }

    public override void parse(source_reader source)
    {
      int alternative = 0;
      if (firsts.can_accept(source))
      {
        foreach (component c in alternatives)
          if (!c.firsts.can_accept(source)) alternative = alternative + 1; else break;
        source.next_symbol();
        parsable.elements.Push(alternative);
      }
      else
      {
        if (!maybe_empty) parsable.error("Missing " + t.Name); else parsable.elements.Push(default_index);
      }
    }
    
    public override void unparse_object(parsable o, FieldInfo field, source_builder source)
    {
      object value = field.GetValue(o);
      source.write((int)value, symbols);
    }

    public override void unparse(source_builder source)
    { 
      source.write("(");
      for (int i = 0; i < alternatives.Length; i = i + 1) 
      {
        if (i > 0) source.write(" | ");
        alternatives[i].unparse(source);
      }
      source.write(")");
    }
  }
}
