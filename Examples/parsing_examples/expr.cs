using System.Collections.Generic;
using System.Linq;
using parsable_objects;

namespace expr
{
  interface evaluator
  {
    void evaluate(Stack<object> values, Dictionary<string, object> environment);
  }
  
  class expr: parsable, evaluator
  {
    [Parse(1)] private simple_expr          simple_expr;
    [Parse(2)] private Optional<comparison> comparison;

    public void evaluate(Stack<object> values, Dictionary<string, object> environment)
    {
      simple_expr.evaluate(values, environment);
      if (comparison.Defined) comparison.Value.evaluate(values, environment);
    }
  } 
  
  class comparison: parsable, evaluator
  { 
    private enum relation_op {equal, not_equal, less, less_than_equal, greater, greater_than_equal};
        
    [Parse(1, "==", "!=", "<", "<=", ">", ">=")] private relation_op relation;
    [Parse(2)                                  ] private simple_expr simple_expr;

    public void evaluate(Stack<object> values, Dictionary<string, object> environment)
    {
      simple_expr.evaluate(values, environment);
      object b = values.Pop();
      object a = values.Pop();
      switch (relation)
      {
        case relation_op.equal             : values.Push((double)a == (double)b); break;
        case relation_op.not_equal         : values.Push((double)a != (double)b); break;
        case relation_op.less              : values.Push((double)a <  (double)b); break;
        case relation_op.less_than_equal   : values.Push((double)a <= (double)b); break;
        case relation_op.greater           : values.Push((double)a >  (double)b); break;
        case relation_op.greater_than_equal: values.Push((double)a >= (double)b); break;
      }
    }
  }
  
  enum unary_op {plus, minus, none};
  
  class simple_expr: parsable, evaluator
  {
    [Parse(1, "+", "-", "")] private unary_op    unary;
    [Parse(2)              ] private term        term;
    [Parse(3)              ] private List<terms> terms;

    public void evaluate(Stack<object> values, Dictionary<string, object> environment)
    {
      term.evaluate(values, environment);
      switch (unary)
      {
        case unary_op.plus:  values.Push(+(double)values.Pop()); break;
        case unary_op.minus: values.Push(-(double)values.Pop()); break;
        case unary_op.none:                              break;
      }
      terms.ForEach((t) => t.evaluate(values, environment));
    }
  }
  
  class terms: parsable, evaluator
  {  
    private enum adding_op {plus, minus, or};
    
    [Parse(1, "+", "-", "||")] private adding_op op;
    [Parse(2, "+", "-", "")  ] private unary_op  unary;
    [Parse(3)                ] private term      term;

    public void evaluate(Stack<object> values, Dictionary<string, object> environment)
    {
      term.evaluate(values, environment);
      switch (unary)
      {
        case unary_op.plus:  values.Push(+(double)values.Pop()); break;
        case unary_op.minus: values.Push(-(double)values.Pop()); break;
        case unary_op.none:                              break;
      }
      object b = values.Pop();
      object a = values.Pop();
      switch (op)
      {
        case adding_op.plus : values.Push((double)a +  (double)b); break;
        case adding_op.minus: values.Push((double)a -  (double)b); break;
        case adding_op.or   : values.Push((bool  )a || (bool  )b); break;
      }
    }
  }
  
  class term: parsable, evaluator
  {   
    [Parse(1)] private factor        factor;
    [Parse(2)] private List<factors> factors;

    public void evaluate(Stack<object> values, Dictionary<string, object> environment)
    {
      factor.evaluate(values, environment);
      factors.ForEach((f) => f.evaluate(values, environment));
    }
  }
  
  class factors: parsable, evaluator
  {
    private enum product_op {times, divide, mod, and};
    
    [Parse(1, "*", "/", "%", "&&")] private product_op op;
    [Parse(2)                     ] private factor     factor;

    public void evaluate(Stack<object> values, Dictionary<string, object> environment)
    {
      factor.evaluate(values, environment);
      object b = values.Pop();
      object a = values.Pop();
      switch (op)
      {
        case product_op.times : values.Push((double)a *  (double)b); break;
        case product_op.divide: values.Push((double)a /  (double)b); break;
        case product_op.mod   : values.Push((double)a %  (double)b); break;
        case product_op.and   : values.Push((bool  )a && (bool  )b); break;
      }
    }
  }
  
  class factor: parsable, evaluator
  { 
    public class number_factor: factor
    {      
      [Parse(1)] private int value;
      
      public override void evaluate(Stack<object> values, Dictionary<string, object> environment)
      {
        values.Push((double)value);
      }
    }
    
    public class real_factor: factor
    {      
      [Parse(1)] private double value;
      
      public override void evaluate(Stack<object> values, Dictionary<string, object> environment)
      {
        values.Push(value);
      }
    }
    
    public class id_factor: factor
    {      
      [Parse(1)] private identifier id;
      
      public override void evaluate(Stack<object> values, Dictionary<string, object> environment)
      {
        if (environment.Keys.Contains(id.spelling)) values.Push(environment[id.spelling]);
        else values.Push(0.0);
      }
    }
    
    public class expr_factor: factor
    {      
      [Parse(1, "(")] private punctuation open;
      [Parse(2)     ] private expr        expr;
      [Parse(3, ")")] private punctuation close;
      
      public override void evaluate(Stack<object> values, Dictionary<string, object> environment)
      {
        expr.evaluate(values, environment);
      }
    }
    
    public class if_factor: factor
    {      
      [Parse(1, "If")  ] private reserved_word word_if;
      [Parse(2)        ] private expr          condition;
      [Parse(3, "Then")] private reserved_word word_then;
      [Parse(4)        ] private expr          then_expr;
      [Parse(5, "Else")] private reserved_word word_else;
      [Parse(6)        ] private expr          else_expr;
      
      public override void evaluate(Stack<object> values, Dictionary<string, object> environment)
      {
        condition.evaluate(values, environment);
        bool condition_value = (bool)values.Pop();
        then_expr.evaluate(values, environment);
        object then_value = values.Pop();
        else_expr.evaluate(values, environment);
        object else_value = values.Pop();
        values.Push(condition_value ? then_value : else_value);
      }
    }

    public virtual void evaluate(Stack<object> values, Dictionary<string, object> environment)
    {
    }
  }
}
