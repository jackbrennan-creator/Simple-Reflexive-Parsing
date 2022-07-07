using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace parsable_objects
{
  public class source_builder
  {
    protected StringBuilder source;
    protected int           char_index;
    protected int           indentation;
    private   Stack<int>    columns;
    private   Stack<int>    previous;
    private   bool          bol;
    private   string        padding;
    private   bool          padded;
    
    public    int           indentation_size {get; set;}
    public    bool          single_line      {get; set;}

    public source_builder()
    {
      reset();
    }
    
    public void reset()
    {
      source = new StringBuilder();
      indentation      = 0;
      char_index       = 0;
      columns          = new Stack<int>();
      previous         = new Stack<int>();
      bol              = true;
      indentation_size = 2;
      single_line      = false;
      padding          = "";
      padded           = false;
      
    }

    public override string ToString()
    {
      return source.ToString();
    }
    
    public void pad(string padding_string)
    {
      padding = padding_string;
    }

    public void write(string value)
    {
      check_bol();
      if (padding != "" && !padded)
      {
        source.Append(padding);
        padding = "";
        padded = true;
      }
      source.Append(value);
      char_index = char_index + value.Length;
      padded = false;
    }

    public void write_file(string path)
    {
      if (!File.Exists(path)) parsable.source_builder_error("Unable to read file " + path + " as it does not exist!");
      try
      {
        StreamReader s = new StreamReader(path);
        new_line();
        string line = s.ReadLine();
        while (line != null)
        { 
          new_line();
          write(line);
          line = s.ReadLine();
        }
        s.Close();
      }
      catch (Exception ex)
      {
        parsable.source_builder_error("Error reading file " + path + Environment.NewLine + ex.Message);
      }
    }

    public void write(bool value)
    {
      write(value.ToString());
    }

    public void write(int value)
    {
      write(value.ToString());
    }

    public void write(double value)
    {
      write(value.ToString());
    }
    
    public void write(int index, params string[] values)
    {
      if (0 <= index && index < values.Length) 
      {
        string value = values[index];
        if (value != "") write(value); 
      }
      else write("???");
    }

    private void check_bol()
    {
      if (bol)
      {
        source.Append("".PadLeft(indentation));
        char_index = indentation;
        bol = false;
        padding = "";
        padded  = false;
      }
    }

    public void new_line()
    {
      if (single_line) return;
      char_index = 0;
      source.Append(Environment.NewLine);
      bol = true;
    }

    public void new_line(int n)
    {
      for (int i = 1; i <= n; i = i + 1) new_line();
    }

    public void indent()
    {
      if (single_line) return;
      indentation = indentation + indentation_size;
      char_index = indentation;
    }

    public void indent(int indentation_size)
    {
      if (single_line) return;
      indentation = indentation + indentation_size;
    }

    public void outdent()
    {
      if (single_line) return;
      indentation = indentation - indentation_size;
    }

    public void outdent(int indentation_size)
    {
      if (single_line) return;
      indentation = indentation - indentation_size;
    }
    
    public delegate void action<t>(t e);
    
    public void iterate<t>(List<t> ts, action<t> writer)
    {
      separate(ts, writer, "");
    }
    
    public void separate<t>(List<t> ts, action<t> writer, string separator)
    {
      for (int i = 0; i < ts.Count; i = i + 1)
      {
        if (i > 0) write(separator);
        writer(ts[i]);
      }
    }
    
    public void separate<t>(List<t> ts, action<t> writer, Action separator_writer)
    {
      for (int i = 0; i < ts.Count; i = i + 1)
      {
        if (i > 0) separator_writer();
        writer(ts[i]);
      }
    }
    
    public void separate_lines<t>(List<t> ts, action<t> writer)
    {
      separate_lines(ts, writer, 1);
    }
    
    public void separate_lines<t>(List<t> ts, action<t> writer, int lines)
    {
      for (int i = 0; i < ts.Count; i = i + 1)
      {
        if (i > 0) new_line(lines);
        writer(ts[i]);
      }
    }
    
    public void add_column(int width)
    {
      columns.Push(indentation + width);
    }
    
    public void next_column()
    {
      if (columns.Count > 0)
      {
        previous.Push(indentation);
        indentation = columns.Peek();
        if (char_index < indentation) write("".PadRight(indentation - char_index));
      }
      else parsable.source_builder_error("source_builder.next_column: no column defined");
    }
    
    public void previous_column()
    {
      if (previous.Count > 0)
      {
        indentation = previous.Peek();
      }
      else parsable.source_builder_error("source_builder.previous_column: no previous column defined");
    }
    
    public void remove_column()
    {
      if (columns.Count > 0)
      {
        indentation = columns.Pop();
      }
      else parsable.source_builder_error("source_builder.remove_column: no column to remove");
    }
  }
}
