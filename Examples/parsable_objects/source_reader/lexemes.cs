
namespace parsable_objects
{
  public class lexeme
  {
    public string spelling;
    
    internal lexeme()
    {
      spelling = "";
    }
    
    internal string unparse()
    {
      return this.GetType().Name + " " + spelling;
    }
  }
  
  internal class number: lexeme
  {
    public number(string s)
    {
      spelling = s;
    }
  }
  
  internal class real_number: lexeme
  {
    public real_number(string s)
    {
      spelling = s;
    }
  }
  
  public class punctuation: lexeme
  {
    public punctuation(string s)
    {
      spelling = s;
    }
  }
  
  public class identifier: lexeme
  {
    public identifier(string s)
    {
      spelling = s;
    }
  }
  
  public class reserved_word: lexeme
  {
    public reserved_word(string s)
    {
      spelling = s;
    }
  }
  
  public class string_literal: lexeme
  {
    public string_literal(string s)
    {
      spelling = s;
    }
  }
  
  public class char_literal: lexeme
  {
    public char_literal(char ch)
    {
      spelling = "" + ch;
    }
  }
}
