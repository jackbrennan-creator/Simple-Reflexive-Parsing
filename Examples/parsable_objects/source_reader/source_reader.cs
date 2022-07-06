using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace parsable_objects
{ 
  public class source_reader
  {
    private static string  lower_case              = "abcdefghijklmnopqrstuvwxyz";
    private static string  upper_case              = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static string  radix_prefix_chars      = "#";
    private static string  digits                  = "0123456789";
    private static string  binary_digits           = "01";
    private static string  octal_digits            = "01234567";
    private static string  hex_digits              = "0123456789ABCDEFabcdef";
    private static char    separator               = '_';
    private static char    string_quote            = '"';
    private static char    character_quote         = '\'';
    private static string  letters                 = lower_case + upper_case;
    private static string  id_chars                = lower_case + upper_case + digits + separator;
    private static string  layout                  = " " + "\t" + Environment.NewLine;
    
    private const  int     puntuation_index_size   = 256;
    
    private List<string>   reserved_words          = new List<string>();
    private List<string>   punctuation             = new List<string>();
    private List<string>[] puntuation_index       = new List<string>[puntuation_index_size];
    
    internal lexeme        current  { get; private set;  }
    
    public int             line     { get; private set;  }
    public int             column   { get; private set;  }
    public int             position { get { return pt; } }
    
    private string         text;
    private int            pt;
    private bool           eof;

    internal static readonly lexeme end_of_file = new punctuation("<eof>");
    
    public source_reader(FileStream input): this(new StreamReader(input))
    {
      input.Close();
    }
    
    public source_reader(StreamReader input): this(input.ReadToEnd())
    {
      input.Close();
    }
    
    public source_reader(string text)
    {
      reserved_words      = new List<string>();
      punctuation         = new List<string>();
      this.text           = text;
      reset();
    }
    
    public void reset()
    {
      current = new lexeme();
      line    = 0;
      column  = 0;
      pt      = 0;
      eof     = text.Length == 0;
      next_symbol();
    }    
    
    public string full_text()
    {
      return text;
    }
    
    internal void add_reserved_word(string reserved_word)
    {
      if (!reserved_words.Contains(reserved_word)) reserved_words.Add(reserved_word);
    }
    
    internal static bool is_id_string(string s)
    {
      if (s.Length > 0) return letters.Contains(s[0]); else return false;
    } 
    
    internal bool is_reserved(string spelling)
    {
      return reserved_words.Contains(spelling);
    }

    internal void define_symbols(List<string> symbols)
    {
      List<string> punctuation = new List<string>(symbols);
      punctuation.Sort();
      punctuation.Reverse();
      foreach (string s in punctuation) if (s != "") add_punctuation(s);
    }

    private void add_punctuation(string s)
    {
      int first = (int)s[0];
      if (first >= puntuation_index_size) first = puntuation_index_size - 1;
      if (puntuation_index[first] == null) puntuation_index[first] = new List<string>();
      puntuation_index[first].Add(s);
    }
    
    internal bool symbol_is(Type kind)
    {
      return current.GetType() == kind;
    }
    
    internal bool symbol_is(Type kind, string spelling)
    {
      return (current.GetType() == kind) && current.spelling == spelling;
    }
    
    internal bool accept(Type kind)
    {
      if (current.GetType() == kind)
      {
        next_symbol();
        return true;
      }
      else return false;
    }
    
    internal bool accept(Type kind, string spelling)
    {
      if ((current.GetType() == kind) && current.spelling == spelling)
      {
        next_symbol();
        return true;
      }
      else return false;
    }

    internal void next_symbol()
    {
      if (!eof) skip_layout();
      if (eof)
        current = end_of_file;
      else if (digits.IndexOf(text[pt]) >= 0)
        get_number();
      else if (letters.IndexOf(text[pt]) >= 0)
        get_identifier();
      else if (text[pt] == string_quote)
        get_string();
      else if (text[pt] == character_quote)
        get_character();
      else if (radix_prefix_chars.IndexOf(text[pt]) >= 0)
        get_radix();
      else
        get_punctuation();
    }
    
    private void advance()
    {
      pt     = pt + 1;
      column = column + 1;
    }
    
    private void advance(int chars)
    {
      pt     = pt + chars;
      column = column + chars;
    }
    
    private void new_line()
    {
      pt     = pt + 1;
      line   = line + 1;
      column = 0;
    }

    private void skip_layout()
    {
      char ch = text[pt];
      while (layout.IndexOf(ch) >= 0 & !eof)
      {
        if (ch == '\n') new_line(); else advance();
        if (pt < text.Length) ch = text[pt]; else eof = true;
      }
    }

    private void get_number()
    {
      bool is_decimal = false;
      int  first_ch   = pt;
      char  ch        = text[pt];
      while (digits.IndexOf(ch) >= 0 && !eof)
      {
        advance();
        if (pt < text.Length) ch = text[pt]; else eof = true;
      }
      if (ch == '.')
      {
        is_decimal = true;
        advance();
        ch = text[pt];
        while (digits.IndexOf(ch) >= 0 && !eof)
        {
          advance();
          if (pt < text.Length) ch = text[pt]; else eof = true;
        }
      }
      if (is_decimal)
        current = new real_number(text.Substring(first_ch, pt - first_ch));
      else
        current = new number(text.Substring(first_ch, pt - first_ch));
    }

    private void get_radix()
    {
      char prefix = text[pt];
      char ch = ' ';
      advance();
      if (pt < text.Length) ch = text[pt]; else eof = true;
      if (!eof)
        switch (ch)
        {
          case 'b':
          case 'B': get_radix_number(binary_digits, 2); break;
          case 'o':
          case 'O': get_radix_number(octal_digits, 8); break;
          case 'd':
          case 'D': get_radix_number(digits, 10); break;
          case 'h':
          case 'H': get_radix_number(hex_digits, 16); break;
          default: current = new punctuation(prefix.ToString()); break;
        }
      else current = new punctuation(prefix.ToString());
    }

    private void get_radix_number(string digit_chars, int radix)
    {
      char ch;
      advance();
      if (pt >= text.Length) eof = true;
      if (!eof)
      {
        int first_ch = pt;
        ch = text[pt];
        while (digit_chars.IndexOf(ch) >= 0 && !eof)
        {
          advance();
          if (pt < text.Length) ch = text[pt]; else eof = true;
        }
        string digits = text.Substring(first_ch, pt - first_ch);
        if (digits == "")
          current = new punctuation("<?>");
        else
        {
          int value = Convert.ToInt32(digits, radix);
          current = new number(value.ToString());
        }
      }
      else current = new punctuation("<?>");
    }

    private void get_identifier()
    {
      int first_ch = pt;
      char ch = text[pt];
      advance();;
      if (pt < text.Length) ch = text[pt]; else eof = true;
      while (id_chars.IndexOf(ch) >= 0 && !eof)
      {
        advance();
        if (pt < text.Length) ch = text[pt]; else eof = true;
      }
      string spelling = text.Substring(first_ch, pt - first_ch);
      current = new reserved_word(spelling);
      if (!is_reserved(spelling))
        if (spelling != spelling.ToLower())
          current = new punctuation("<?>");
        else
          current = new identifier(spelling);
    }

    private void get_punctuation()
    {
      int first_ch = pt;
      int ch = (int)text[pt];
      if (ch >= puntuation_index_size) ch = puntuation_index_size - 1;
      if (puntuation_index[ch] != null)
        foreach (string s in puntuation_index[ch])
        {
          if (first_ch + s.Length <= text.Length)
            if (text.Substring(first_ch, s.Length) == s)
            {
              current = new punctuation(s);
              advance(s.Length);
              eof = pt >= text.Length;
              break;
            }
        }
      else
      {
        current = new punctuation("<?>");
        advance();
        eof = pt >= text.Length;
      }
    }

    private void get_string()
    {
      int first_ch = pt;
      char ch = ' ';
      pt = pt + 1;
      if (pt < text.Length) ch = text[pt]; else eof = true;
      do
      {
        bool done = false;
        while (ch == string_quote && !eof & !done)
          if (pt + 1 < text.Length)
          {
            if (text[pt + 1] == string_quote)
            {
              advance(2);
              if (pt < text.Length) ch = text[pt]; else eof = true;
            }
            else done = true;
          }
          else eof = true;
        if (ch != string_quote & !eof)
        {
          advance();
          if (pt < text.Length) ch = text[pt]; else eof = true;
        }
      }
      while (ch != string_quote & !eof);
      if (eof)
        current = new punctuation("<?>");
      else
      {
        string chars = text.Substring(first_ch + 1, pt - first_ch - 1);
        current = new string_literal(chars.Replace("\"\"", "\""));
        advance();
        eof = pt >= text.Length;
      }
    }

    private void get_character()
    {
      int  first_ch = pt;
      char ch = ' ';
      char value;
      advance();
      if (pt < text.Length) 
      {
        value = text[pt];
        advance();
        if (pt < text.Length) 
        {
          if (value == '\\')
          {
            ch = text[pt];
            if (digits.IndexOf(ch) >= 0)
            {
              int char_code = get_digits();
              if (!eof)
              {
                ch = text[pt];
                if (ch == character_quote)
                {
                  current = new char_literal((char)char_code);
                  advance();
                  eof = pt >= text.Length;
                }
                else current = new punctuation("<?>");
              }
            }
            else
            {
              current = new char_literal(value);
              advance();
              eof = pt >= text.Length;
            }
          }
          else
          {
            ch = text[pt]; 
            if (ch == character_quote)
            {
              current = new char_literal(value);
             advance();
              eof = pt >= text.Length;
            }
            else current = new punctuation("<?>");
          }
        } 
        else eof = true;
      }
      else eof = true;
    }

    private int get_digits()
    {
      int first_ch = pt;
      char ch = text[pt];
      while (digits.IndexOf(ch) >= 0 && !eof)
      {
        advance();
        if (pt < text.Length) ch = text[pt]; else eof = true;
      }
      string chars = text.Substring(first_ch, pt - first_ch);
      return int.Parse(chars);
    }
  }
}
