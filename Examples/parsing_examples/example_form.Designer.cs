namespace parsing_examples
{
  partial class example_form
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(example_form));
      this.tree_input_box = new System.Windows.Forms.TextBox();
      this.grammar_box = new System.Windows.Forms.TextBox();
      this.parse_grammar_button = new System.Windows.Forms.Button();
      this.output_box = new System.Windows.Forms.TextBox();
      this.parse_tree_button = new System.Windows.Forms.Button();
      this.parse_expr_button = new System.Windows.Forms.Button();
      this.expr_input_box = new System.Windows.Forms.TextBox();
      this.grammar_input_box = new System.Windows.Forms.TextBox();
      this.grammar_details_label = new System.Windows.Forms.Label();
      this.output_label = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // tree_input_box
      // 
      this.tree_input_box.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tree_input_box.Location = new System.Drawing.Point(107, 13);
      this.tree_input_box.Multiline = true;
      this.tree_input_box.Name = "tree_input_box";
      this.tree_input_box.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.tree_input_box.Size = new System.Drawing.Size(438, 80);
      this.tree_input_box.TabIndex = 1;
      this.tree_input_box.Text = "a(b() c(d(d1() d2() d3()) e()))";
      this.tree_input_box.WordWrap = false;
      // 
      // grammar_box
      // 
      this.grammar_box.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.grammar_box.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grammar_box.Location = new System.Drawing.Point(551, 28);
      this.grammar_box.Multiline = true;
      this.grammar_box.Name = "grammar_box";
      this.grammar_box.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.grammar_box.Size = new System.Drawing.Size(446, 620);
      this.grammar_box.TabIndex = 7;
      this.grammar_box.WordWrap = false;
      // 
      // parse_grammar_button
      // 
      this.parse_grammar_button.Location = new System.Drawing.Point(12, 187);
      this.parse_grammar_button.Name = "parse_grammar_button";
      this.parse_grammar_button.Size = new System.Drawing.Size(89, 23);
      this.parse_grammar_button.TabIndex = 4;
      this.parse_grammar_button.Text = "Parse Grammar";
      this.parse_grammar_button.UseVisualStyleBackColor = true;
      this.parse_grammar_button.Click += new System.EventHandler(this.parse_grammar_button_Click);
      // 
      // output_box
      // 
      this.output_box.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)));
      this.output_box.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.output_box.Location = new System.Drawing.Point(107, 346);
      this.output_box.Multiline = true;
      this.output_box.Name = "output_box";
      this.output_box.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.output_box.Size = new System.Drawing.Size(438, 302);
      this.output_box.TabIndex = 6;
      this.output_box.WordWrap = false;
      // 
      // parse_tree_button
      // 
      this.parse_tree_button.Location = new System.Drawing.Point(12, 12);
      this.parse_tree_button.Name = "parse_tree_button";
      this.parse_tree_button.Size = new System.Drawing.Size(89, 23);
      this.parse_tree_button.TabIndex = 0;
      this.parse_tree_button.Text = "Parse Tree";
      this.parse_tree_button.UseVisualStyleBackColor = true;
      this.parse_tree_button.Click += new System.EventHandler(this.parse_tree_button_Click);
      // 
      // parse_expr_button
      // 
      this.parse_expr_button.Location = new System.Drawing.Point(12, 99);
      this.parse_expr_button.Name = "parse_expr_button";
      this.parse_expr_button.Size = new System.Drawing.Size(89, 23);
      this.parse_expr_button.TabIndex = 2;
      this.parse_expr_button.Text = "Evaluate Expr";
      this.parse_expr_button.UseVisualStyleBackColor = true;
      this.parse_expr_button.Click += new System.EventHandler(this.evaluate_expr_button_Click);
      // 
      // expr_input_box
      // 
      this.expr_input_box.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.expr_input_box.Location = new System.Drawing.Point(107, 99);
      this.expr_input_box.Multiline = true;
      this.expr_input_box.Name = "expr_input_box";
      this.expr_input_box.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.expr_input_box.Size = new System.Drawing.Size(438, 82);
      this.expr_input_box.TabIndex = 3;
      this.expr_input_box.Text = "(If 2 * x + 1 > 0 Then y - 1 Else 0) * pi\r\n";
      this.expr_input_box.WordWrap = false;
      // 
      // grammar_input_box
      // 
      this.grammar_input_box.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grammar_input_box.Location = new System.Drawing.Point(107, 187);
      this.grammar_input_box.Multiline = true;
      this.grammar_input_box.Name = "grammar_input_box";
      this.grammar_input_box.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.grammar_input_box.Size = new System.Drawing.Size(438, 140);
      this.grammar_input_box.TabIndex = 5;
      this.grammar_input_box.Text = resources.GetString("grammar_input_box.Text");
      this.grammar_input_box.WordWrap = false;
      // 
      // grammar_details_label
      // 
      this.grammar_details_label.AutoSize = true;
      this.grammar_details_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.grammar_details_label.Location = new System.Drawing.Point(552, 12);
      this.grammar_details_label.Name = "grammar_details_label";
      this.grammar_details_label.Size = new System.Drawing.Size(99, 13);
      this.grammar_details_label.TabIndex = 14;
      this.grammar_details_label.Text = "Grammar Details";
      // 
      // output_label
      // 
      this.output_label.AutoSize = true;
      this.output_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.output_label.Location = new System.Drawing.Point(104, 330);
      this.output_label.Name = "output_label";
      this.output_label.Size = new System.Drawing.Size(45, 13);
      this.output_label.TabIndex = 15;
      this.output_label.Text = "Output";
      // 
      // example_form
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1009, 660);
      this.Controls.Add(this.output_label);
      this.Controls.Add(this.grammar_details_label);
      this.Controls.Add(this.grammar_input_box);
      this.Controls.Add(this.expr_input_box);
      this.Controls.Add(this.parse_expr_button);
      this.Controls.Add(this.parse_tree_button);
      this.Controls.Add(this.output_box);
      this.Controls.Add(this.parse_grammar_button);
      this.Controls.Add(this.grammar_box);
      this.Controls.Add(this.tree_input_box);
      this.Name = "example_form";
      this.Text = "Parsing Examples";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox tree_input_box;
    private System.Windows.Forms.TextBox grammar_box;
    private System.Windows.Forms.Button parse_grammar_button;
    private System.Windows.Forms.TextBox output_box;
    private System.Windows.Forms.Button parse_tree_button;
    private System.Windows.Forms.Button parse_expr_button;
    private System.Windows.Forms.TextBox expr_input_box;
    private System.Windows.Forms.TextBox grammar_input_box;
    private System.Windows.Forms.Label grammar_details_label;
    private System.Windows.Forms.Label output_label;
  }
}

