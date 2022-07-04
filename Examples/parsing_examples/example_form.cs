using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using parsable_objects;

namespace parsing_examples
{
  public partial class example_form : Form
  {
    public example_form()
    {
      InitializeComponent();
      this.CenterToScreen();
    }

    private void parse_tree_button_Click(object sender, EventArgs e)
    {
      var tree = parse<tree.tree>(tree_input_box.Text);
      if (tree != null) 
      {
        var output = new source_builder();
        parsable.unparse_object(tree, output);
        output_box.Text = output.ToString();
      }
    }

    private void evaluate_expr_button_Click(object sender, EventArgs e)
    {
      var expr = parse<expr.expr>(expr_input_box.Text);
      if (expr != null) 
      {
        Stack<object>              results     = new Stack<object>();
        var                        output      = new source_builder();
        parsable.unparse_object(expr, output);
        output.new_line(2);
        Dictionary<string, object> environment = new Dictionary<string, object>();
        environment.Add("x",     5.0    );
        environment.Add("y",     7.0    );
        environment.Add("z",     3.0    );
        environment.Add("e",     Math.E );
        environment.Add("pi",    Math.PI);
        environment.Add("false", false  );
        environment.Add("true",  true   );
        expr.evaluate(results, environment);
        if (results.Count > 0)
        {
          object result = results.Pop();
          output.write(result.ToString());
        }
        else output.write("No result");
        output_box.Text = output.ToString();
      }
    }

    private void parse_grammar_button_Click(object sender, EventArgs e)
    {
      var grammar = parse<grammar.grammar>(grammar_input_box.Text);
      if (grammar != null) 
      {
        var output = new source_builder();
        grammar.unparse(output);
        output_box.Text = output.ToString();
      }
    }

    private t parse<t>(string input_text) where t: parsable, new()
    {
      output_box.Clear();
      source_reader input = new source_reader(input_text);
      try
      {
        parsable.reset();
        var output = new source_builder();
        parsable.define<t>(input);
        output.write("Grammar");
        output.new_line();
        output.write("-------");
        output.new_line(2);
        parsable.unparse_grammar(output);
        output.new_line(2);
        output.write("Firsts");
        output.new_line();
        output.write("------");
        output.new_line(2);
        parsable.unparse_firsts(output);
        grammar_box.Text = output.ToString();
        output.reset();
        return parsable.parse<t>(input);
      }
      catch (parse_error ex)
      {
        output_box.AppendText(ex.Message);
        tree_input_box.Select(input.position, 1);
        tree_input_box.Focus();
        return null;
      }
    }
  }
}
