using System;
using System.Collections.Generic;
using System.Linq;

namespace parsable_objects
{
  class lexeme_set
  {
    private List<string>     symbols;
    private List<string>     words;
    private List<production> productions;
    private bool             contains_number;
    private bool             contains_real;
    private bool             contains_id;
    private bool             contains_string;
    private bool             contains_char;
    
    public lexeme_set()
    {
      reset();
    }
    
    public void reset()
    {
      symbols         = new List<string>();
      words           = new List<string>();
      productions     = new List<production>();
      contains_number = false;
      contains_real   = false;
      contains_id     = false;
      contains_string = false;
      contains_char   = false;
    }
    
    public void insert(component c)
    {
      if      (c is terminal_component      ) insert(((terminal_component)c).spelling,      symbols);
      else if (c is reserved_word_component ) insert(((reserved_word_component)c).spelling, words  );
      else if (c is identifier_component    ) contains_id     = true;
      else if (c is number_component        ) contains_number = true;
      else if (c is real_component          ) contains_real   = true;
      else if (c is string_literal_component) contains_string = true;
      else if (c is char_literal_component  ) contains_char   = true;
      else if (c is non_terminal_component  ) insert(((non_terminal_component)c).definition);
      else                                    insert(c.firsts);
    }
    
    public void insert(production p)
    {
      if (!productions.Contains(p)) productions.Add(p);
    }
    
    public void insert(lexeme_set s)
    {
      symbols = symbols.Union(s.symbols).ToList();
      words   = words.Union(s.words).ToList();
      if (s.contains_id    ) contains_id     = true;
      if (s.contains_number) contains_number = true;
      if (s.contains_real  ) contains_real   = true;
      if (s.contains_string) contains_string = true;
      if (s.contains_char  ) contains_char   = true;
      foreach (var p in s.productions) insert(p);
    }
    
    public bool intersects(lexeme_set s)
    {
      foreach (var p in productions) if (s.intersects(p.firsts)) return true;
      var common_symbols = symbols.Intersect(s.symbols).ToList();
      if (common_symbols.Count > 0) return true;
      var common_words   = words.Intersect(s.words).ToList();
      if (common_words.Count > 0) return true;
      if (s.contains_id     && this.contains_id    ) return true;
      if (s.contains_number && this.contains_number) return true;
      if (s.contains_real   && this.contains_real  ) return true;
      if (s.contains_string && this.contains_string) return true;
      if (s.contains_char   && this.contains_char  ) return true;
      return false;
    }
    
    public bool can_accept(source_reader source)
    {
      if (productions.Count > 0) flattern();
      if (source.symbol_is(typeof(number        ))) return contains_number;
      if (source.symbol_is(typeof(real_number   ))) return contains_real;
      if (source.symbol_is(typeof(identifier    ))) return contains_id;
      if (source.symbol_is(typeof(string_literal))) return contains_string;
      if (source.symbol_is(typeof(char_literal  ))) return contains_char;
      foreach (string s in symbols) if (source.symbol_is(typeof(punctuation  ), s)) return true;
      foreach (string w in words  ) if (source.symbol_is(typeof(reserved_word), w)) return true;
      return false;
    }
    
    private void insert(string s, List<string> target)
    {
      if (!target.Contains(s)) target.Add(s);
    }
    
    public void flattern()
    {
      while (productions.Count > 0)
      {
        production p = productions[0];
        insert(p.firsts);
        productions.RemoveAt(0);
      }
    }
    
    public string unparse()
    {
      string result = "";
      foreach (string     s in symbols    ) unparse("\"" + s + "\"",      ref result);
      foreach (string     w in words      ) unparse("\"" + w + "\"",      ref result);
      foreach (production p in productions) unparse("<" + p.t.Name + ">", ref result);
      if (contains_number) unparse("<number>",         ref result);
      if (contains_real  ) unparse("<real_number>",    ref result);
      if (contains_id    ) unparse("<identifier>",     ref result);
      if (contains_string) unparse("<string_literal>", ref result);
      if (contains_char  ) unparse("<char_literal>",   ref result);
      return result;
    }
    
    private void unparse(string item, ref string result)
    {
      if (result != "") result = result + " "; result = result + item;
    }
  }
}
