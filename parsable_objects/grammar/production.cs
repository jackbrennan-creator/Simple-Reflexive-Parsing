using System;
using System.Collections.Generic;
using System.Reflection;

namespace parsable_objects
{
  internal class production
  {
    public Type              t;
    public List<alternative> alternatives;
    public lexeme_set        firsts;
    public bool              maybe_empty;
    public bool              defined;
    public bool              maybe_left_recursive;
    public List<FieldInfo>   fields;
    public List<FieldInfo>   marked_fields;
    
    public production(Type t)
    {
      this.t                    = t;
      this.alternatives         = new List<alternative>();
      this.firsts               = new lexeme_set();
      this.maybe_empty          = false;
      this.defined              = false;
      this.fields               = parsable.get_parsable_fields(t);
      this.marked_fields        = parsable.get_marked_fields(t);
      this.maybe_left_recursive = false;
    }
    
    public void add_alternative(List<component> components, Type alternative_type)
    {
      var a = new alternative(components, alternative_type);
      if (a.maybe_empty) maybe_empty = true;
      alternatives.Add(a);
    }
    
    public void analyse()
    {
      if (!defined)
      {
        defined = true;
        foreach (var a in alternatives)
        {
          a.analyse(this);
          if (a.firsts.intersects(firsts)) 
            parsable.grammar_error("The alternatives of " + t.Name + " are ambigous");
          firsts.insert(a.firsts);
          maybe_empty = maybe_empty || a.maybe_empty;
        }
      }
    }
    
    public void parse(source_reader source)
    {
      int alternative_index = 0;
      int elements_parsed   = 0;
      Type element_type     = t;
      int  start            = source.position;
      if (firsts.can_accept(source) || maybe_empty) 
      {
        foreach (var a in alternatives)
        {
          if (a.firsts.can_accept(source) || a.maybe_empty) 
          {
            int first_element_index = parsable.elements.Count;
            a.parse(source);
            int final_element_index = parsable.elements.Count;
            elements_parsed = final_element_index - first_element_index;
            element_type    = a.alternative_type;
            break;
          }
          alternative_index = alternative_index + 1;
        }
      }
      else parsable.error("Missing " + t.Name);
      push_result(alternative_index, elements_parsed, element_type); 
      if (marked_fields.Count > 0) 
        marked_fields[0].SetValue(parsable.elements.Peek(), new source_position(start, source.position - start));
    }

    private void push_result(int alternative_index, int elements_parsed, Type element_type)
    {
      if (t.Name != element_type.Name) return;
      ConstructorInfo constructor = element_type.GetConstructor(new Type[] { });
      if (constructor != null)
      {
        var parsed_object  = (parsable)constructor.Invoke(new object[] { });
        var field_elements = new Stack<object>();
        for (int i = 1; i <= elements_parsed; i = i + 1) field_elements.Push(parsable.elements.Pop());
        assign_fields(parsed_object, field_elements, alternative_index);
        parsed_object.parsed();
        if (parsable.current_source != null) parsed_object.parsed(parsable.current_source);
        parsable.elements.Push(parsed_object);
      }
      else parsable.grammar_error("Class " + t.Name + " must have an accessible parameterless, default constuctor");
    }
    
    private void assign_fields(parsable o, Stack<object> elements, int alternative_index)
    {
      if (fields.Count > 0)
      {
        foreach (FieldInfo f in fields)
          if (parsable.is_parsable_list(f.FieldType))
          {
            object       iteration_value = elements.Pop();
            Type         iteration_type  = iteration_value.GetType();
            PropertyInfo value_property  = iteration_type.GetProperty("Value");
            object       iteration_list  = value_property.GetValue(iteration_value, new object[]{});
            f.SetValue(o, iteration_list);
          }
          else if (f.FieldType != typeof(punctuation) && f.FieldType != typeof(reserved_word))
          {
            f.SetValue(o, elements.Pop());
          }
      }
      else 
      {
        o.parsed(elements, alternative_index);
        o.parsed(alternative_index);
      }
    }
    
    public void unparse_object(parsable o, source_builder source)
    {
      Type object_type = o.GetType();
      if (object_type == t)
      {
        if (alternatives.Count == 1)
          alternatives[0].unparse_object(o, source);
        else parsable.source_builder_error("Unable to unparse object of type " + object_type.Name + ": Only one level of inheritence allowed.");
      }
      else
      {
        production p = parsable.productions[object_type.Name];
        p.unparse_object(o, source);
      }
    }

    public void unparse(source_builder source, int id_width)
    {
      string definition = ("<" + t.Name + ">").PadRight(id_width) + " ::= ";
      source.write(definition);
      source.indent(definition.Length);
      for (int i = 0; i < alternatives.Count; i = i + 1)
      {
        if (i > 0)
        {
          source.new_line();
          source.write("|");
          source.new_line();
        }
        alternatives[i].unparse(source);
      }
      source.write(";");
      source.outdent(definition.Length);
    }
    
    public void unparse_class(source_builder source)
    {
      source.write("Class " + t.Name);
      source.new_line();
      source.write("{");
        source.indent();
          if (alternatives.Count > 1)
          {
          }
          else ;
        source.outdent();
      source.write("}");
    }
    
    private void unparse_sub_class(alternative a, source_builder source)
    {
    }
  }
}
