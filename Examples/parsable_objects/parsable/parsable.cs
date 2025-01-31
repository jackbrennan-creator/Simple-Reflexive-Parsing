using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace parsable_objects
{ 
  public class parsable
  {   
    internal static Dictionary<string, production> productions                   = new Dictionary<string, production>();
    internal static List<string>                   symbols                       = new List<string>();
    internal static List<string>                   words                         = new List<string>();
    internal static Stack<bool>                    checking_left_recursion_state = new Stack<bool>();
    internal static bool                           checking_left_recursion       = false;
    internal static Stack<object>                  elements                      = new Stack<object>();
    internal static source_reader                  current_source                = null;
    
    public static void reset()
    {
      productions                   = new Dictionary<string, production>();
      elements                      = new Stack<object>();
      symbols                       = new List<string>();
      words                         = new List<string>();
      checking_left_recursion_state = new Stack<bool>();
      checking_left_recursion       = false;
    }
    
    public static void define<t>(source_reader source) where t: parsable, new()
    {
      current_source = source;
      analyse(typeof(t));
      foreach (production p in parsable.productions.Values) p.firsts.flattern();
      source.define_symbols(symbols);
      foreach (string w in words) source.add_reserved_word(w);
      source.reset();
    }
    
    public static t parse<t>(source_reader source) where t: parsable, new()
    {
      define<t>(source);
      production root = productions[typeof(t).Name];
      root.parse(source);
      if (!source.symbol_is(typeof(punctuation), "<eof>")) error("Extra symbols before end of source");
      return (t)elements.Pop();
    }
    
    public static void unparse_object<t>(t o, source_builder source) where t: parsable, new()
    {
      if (productions.Keys.Contains(typeof(t).Name))
      {
        production root = productions[typeof(t).Name];
        root.unparse_object(o, source);
      }
      else source_builder_error("Unable to unparse object of type " + typeof(t).Name + ". No corresponding production.");
    }
    
    public static void unparse_grammar(source_builder source)
    {
      var definitions = productions.Values.ToArray();
      int id_width    = definitions.Max((p) => p.t.Name.Length + 2);
      for (int i = 0; i < definitions.Length; i = i + 1)
      {
        if (i > 0)
        {
          source.new_line();
          source.new_line();
        }
        definitions[i].unparse(source, id_width);
      }
    }
    
    public static void unparse_firsts(source_builder source)
    {
      var definitions = productions.Values.ToArray();
      int id_width    = definitions.Max((p) => p.t.Name.Length + 2);
      for (int i = 0; i < definitions.Length; i = i + 1)
      {
        production definition = definitions[i];
        if (i > 0)
        {
          source.new_line();
          source.new_line();
        }
        source.write(("<" + definition.t.Name + ">").PadRight(id_width) + ": " + definition.firsts.unparse());
      }
    }
    
    public delegate void method_unparser(string access_mode, source_builder source);
    
    public static void unparse_classes(bool partial, source_builder source)
    {
      unparse_classes(partial, "", source);
    }
    
    public static void unparse_classes(bool partial, string interface_name, source_builder source, params method_unparser[] inteface_methods)
    {
      if (interface_name != "")
      {
        unparse_interface(interface_name, source, inteface_methods);
        source.new_line();
      }
      List<production> top_level_productions = productions.Values.Where((p1) => !p1.t.IsNested).ToList();
      source.separate_lines(top_level_productions, (p) => unparse_class(partial, p, interface_name, source, inteface_methods));
    }
    
    private static void unparse_interface(string interface_name, source_builder source, params method_unparser[] inteface_methods)
    {
      source.write("interface " + interface_name);
      source.new_line();
      source.write("{");
        source.indent();
        source.new_line();
        if (inteface_methods.Length > 0)
          unparse_interface_methods("", source, inteface_methods);
        else source.new_line();
        source.outdent();
      source.write("}");
      source.new_line();
    }

    private static void unparse_interface_methods(string access_mode, source_builder source, method_unparser[] inteface_methods)
    {
      int i = 0;
      foreach (method_unparser m in inteface_methods)
      {
        if (i > 0) source.new_line();
        m(access_mode == "" ? "" : access_mode + " ", source);
        if (access_mode == "") source.write(";");
        source.new_line();
        if (access_mode != "")
        {
          source.write("{");
          source.new_line();
          source.write("}");
          source.new_line();
        }
        i = i + 1;
      }
    }
    
    private static void unparse_class(bool partial, production p, string interface_name, source_builder source, method_unparser[] inteface_methods)
    {
      source.write((partial ? "partial " : "") + "class " + p.t.Name + ((interface_name == "") ? "" : ": " + interface_name));
      source.new_line();
      source.write("{");
      source.new_line();
        source.indent();
          Type   definition = p.t;
          Type[] types      = definition.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public);
          int    i          = 0;
          List<Type> subtypes = types.Where((t) => t.IsSubclassOf(definition)).ToList();
          source.separate_lines(subtypes, (t) => unparse_nested_class(partial, p.t, t, source, inteface_methods));
          source.new_line();
          if (inteface_methods.Length > 0)
            unparse_interface_methods(subtypes.Count > 0 ? "public virtual" : "public", source, inteface_methods);
        source.outdent();
      source.write("}");
      source.new_line();
    }
    
    private static void unparse_nested_class(bool partial, Type parent_type, Type t, source_builder source, method_unparser[] inteface_methods)
    {
      source.write((partial ? "partial " : "") + "class " + t.Name + ": " + parent_type.Name);
      source.new_line();
      source.write("{");
        source.indent();
          source.new_line();
          if (inteface_methods.Length > 0)
            unparse_interface_methods("public override", source, inteface_methods);
          else source.new_line();
        source.outdent();
      source.write("}");
      source.new_line();
    }
    
    public virtual void parsed()
    {
    }
    
    public virtual void parsed(source_reader source)
    {
    }
    
    internal static void analyse(Type definition)
    {
      if (!productions.Keys.Contains(definition.Name))
      {
        add_production(definition);
        if      (has_parsable_subtypes(definition)          ) analyse_parsable_subtypes(definition);
        else if (has_parsable_fields(definition, definition)) anaylse_parsable_fields(definition, definition); 
        else grammar_error("Class " + definition.Name + " must have parsable fields or parsable subtypes");
        productions[definition.Name].analyse();
      }
    }
    
    private static bool has_parsable_subtypes(Type definition)
    {
      bool   result = false;
      Type[] types  = definition.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public);
      foreach (Type t in types)
        if (t.IsSubclassOf(definition)) result = true;
      return result;
    }
    
    private static void analyse_parsable_subtypes(Type definition)
    {
      Type[] types  = definition.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public);
      foreach (Type t in types)
        if (t.IsSubclassOf(definition))
        {
          //////
          //if (!t.IsNestedPrivate) grammar_error("Class " + t.Name + ": parsable subtypes must be private");
          if (has_parsable_fields(definition, t)) anaylse_parsable_fields(definition, t);
          else grammar_error("Class " + t.Name + " must define at least one parsable field");
          productions[definition.Name].add_alternative(new List<component>{non_terminal_component_of_type(t)}, t);  
          productions[t.Name].analyse();
        }
    }
    
    private static bool has_parsable_fields(Type definition, Type alternative_type)
    {
      return get_parsable_fields(alternative_type).Count > 0;
    }
    
    private static void anaylse_parsable_fields(Type definition, Type alternative_type)
    {
      List<FieldInfo> fields = get_parsable_fields(alternative_type);
      if (fields.Count > 0)
      {
        List<component> components = new List<component>();
        foreach (FieldInfo field in fields) 
          if      (is_iteration(field.FieldType)           ) components.Add(create_iteration_component(field)    );
          else if (is_parsable(field.FieldType)            ) components.Add(non_terminal_component_of_type(field.FieldType)              );
          else if (field.FieldType == typeof(int)          ) components.Add(new number_component()               );
          else if (field.FieldType == typeof(double)       ) components.Add(new real_component()                 );
          else if (field.FieldType == typeof(char)         ) components.Add(new char_literal_component()         );
          else if (field.FieldType == typeof(string)       ) components.Add(new string_literal_component()       );
          else if (field.FieldType == typeof(identifier)   ) components.Add(new identifier_component()           );
          else if (field.FieldType.IsEnum                  ) components.Add(create_alternative_component(field)  ); 
          else if (field.FieldType == typeof(punctuation)  ) components.Add(create_punctuation_component(field)  );
          else if (field.FieldType == typeof(reserved_word)) components.Add(create_reserved_word_component(field));
          else if (is_parsable_list(field.FieldType)       ) components.Add(create_list_component(field)         );
          else grammar_error("Parse attribute can only be applied to fields with parsable types");
        add_production(alternative_type);
        productions[alternative_type.Name].add_alternative(components, alternative_type);
      }
    }
    
    private static component new_lexeme_component(Type lexeme_type)
    {
      if      (lexeme_type == typeof(int)       ) return new number_component        ();
      else if (lexeme_type == typeof(double)    ) return new real_component          ();
      else if (lexeme_type == typeof(char)      ) return new char_literal_component  ();
      else if (lexeme_type == typeof(string)    ) return new string_literal_component();
      else if (lexeme_type == typeof(identifier)) return new identifier_component    ();
      else 
      {
        grammar_error(lexeme_type.Name + " is not a lexeme");
        return null;
      }
    }
    
    //private static void anaylse_parsable_fields(Type definition, Type alternative_type)
    //{
    //  List<FieldInfo> fields = get_parsable_fields(alternative_type);
    //  if (fields.Count > 0)
    //  {
    //    List<component> components = new List<component>();
    //    foreach (FieldInfo field in fields) 
    //      if      (is_iteration(field.FieldType)           ) components.Add(create_iteration_component(field)    );
    //      else if (is_parsable(field.FieldType)            ) components.Add(non_terminal_component_of_type(field.FieldType)              );
    //      else if (field.FieldType == typeof(int)          ) components.Add(new number_component()               );
    //      else if (field.FieldType == typeof(double)       ) components.Add(new real_component()                 );
    //      else if (field.FieldType == typeof(char)         ) components.Add(new char_literal_component()         );
    //      else if (field.FieldType == typeof(string)       ) components.Add(new string_literal_component()       );
    //      else if (field.FieldType == typeof(identifier)   ) components.Add(new identifier_component()           );
    //      else if (field.FieldType.IsEnum                  ) components.Add(create_alternative_component(field)  ); 
    //      else if (field.FieldType == typeof(punctuation)  ) components.Add(create_punctuation_component(field)  );
    //      else if (field.FieldType == typeof(reserved_word)) components.Add(create_reserved_word_component(field));
    //      else if (is_parsable_list(field.FieldType)       ) components.Add(create_list_component(field)         );
    //      else grammar_error("Parse attribute can only be applied to fields with parsable types");
    //    add_production(alternative_type);
    //    productions[alternative_type.Name].add_alternative(components, alternative_type);
    //  }
    //}
    
    internal static void add_production(Type definition)
    {
      if (!productions.Keys.Contains(definition.Name)) productions.Add(definition.Name, new production(definition));
    }
    
    internal static void start_checking_for_left_recursion()
    {
      checking_left_recursion_state.Push(checking_left_recursion);
      checking_left_recursion = true;
    }
    
    internal static void end_checking_for_left_recursion()
    {
      checking_left_recursion = checking_left_recursion_state.Pop();
    }
    
    internal virtual void parsed(Stack<object> elements)
    {
    }
    
    internal virtual void parsed(Stack<object> elements, int alternative)
    {
    }
    
    internal virtual void parsed(int alternative)
    {
    }
    
    internal static bool IsIteration(Type type)
    {
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Iteration<>);
    }
    
    internal static bool IsOptional(Type type)
    {
      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Optional<>);
    }
    
    //NOTE: Because they rely on calling methods via reflection, the following four methods must all have the same name.
    
    private static component non_terminal_component_of_type(Type t)
    {
      string this_method_name = MethodBase.GetCurrentMethod().Name;
      Type   parsable_type    = typeof(parsable);
      MethodInfo m = parsable_type.GetMethod(this_method_name, BindingFlags.NonPublic | BindingFlags.Static, null, new Type[]{}, new ParameterModifier[]{});
      MethodInfo g = m.MakeGenericMethod(t);
      return (component)g.Invoke(null, new object[]{});
    }
    
    internal static component non_terminal_component_of_type<T>() where T: parsable, new()
    {
      Type t = typeof(T);
      if (IsIteration(t) && t.GetGenericArguments()[0].BaseType == typeof(parsable))
        return new iterated_component<T>(new non_terminal_component(t.GetGenericArguments()[0]));
      else if (IsOptional(t) && t.GetGenericArguments()[0].BaseType == typeof(parsable))
        return new optional_component<T>(new non_terminal_component(t.GetGenericArguments()[0]));
      else if (is_parsable(t))
        return new non_terminal_component(t);
      else  
        grammar_error("Type parameter of single must be parsable");
      return new non_terminal_component(t);
    }
    
    private static component non_terminal_component_of_type(Type t, string parameter)
    {
      string this_method_name = MethodBase.GetCurrentMethod().Name;
      Type   parsable_type    = typeof(parsable);
      MethodInfo m = parsable_type.GetMethod(this_method_name, BindingFlags.NonPublic | BindingFlags.Static, null, new Type[]{typeof(string)}, new ParameterModifier[]{});
      MethodInfo g = m.MakeGenericMethod(t);
      return (component)g.Invoke(null, new object[]{parameter});
    }
    
    internal static component non_terminal_component_of_type<T>(string parameter) where T: parsable, new()
    {
      Type t = typeof(T);
      if (IsIteration(t) && t.GetGenericArguments()[0].BaseType == typeof(parsable))
        return new iterated_component<T>(new non_terminal_component(t.GetGenericArguments()[0]), parameter);
      else if (IsOptional(t) && t.GetGenericArguments()[0].BaseType == typeof(parsable))
        return new optional_component<T>(new non_terminal_component(t.GetGenericArguments()[0]));
      else if (is_parsable(t))
        return new non_terminal_component(t);
      else  
        grammar_error("Type parameter of single must be parsable");
      return new non_terminal_component(t);
    }
    
    private static bool is_parsable(Type t)
    {
      if (t == typeof(object)) return false; else if (t == typeof(parsable)) return true; else return is_parsable(t.BaseType);
    }
    
    internal static component sym(string spelling)
    {
      return new terminal_component(spelling);
    }
    
    internal static component number()
    {
      return new number_component();
    }
    
    internal static component real()
    {
      return new real_component();
    }
    
    internal static component string_literal()
    {
      return new string_literal_component();
    }
    
    internal static component char_literal()
    {
      return new char_literal_component();
    }
    
    internal static component id()
    {
      return new identifier_component();
    }
    
    internal static component word(string spelling)
    { 
      return new reserved_word_component(spelling);
    }
       
    internal static component optional<T>(component c) where T: parsable, new()
    {
      return new optional_component<T>(c);
    }
    
    internal static component iteration<T>(component c) where T: parsable, new()
    {
      return new iterated_component<T>(c);
    }
    
    private static component alt(string[] alternatives, Type t)
    {
      return new alt_component(alternatives, t);
    }
    
    internal static void add_symbol(string s)
    {
      if (s != "" && !symbols.Contains(s)) symbols.Add(s);
    }
    
    internal static void add_word(string w)
    {
      if (w != "" && !words.Contains(w)) words.Add(w);
    }

    private static component create_iteration_component(FieldInfo f)
    {
      object[] attributes = f.GetCustomAttributes(typeof(Parse), false);
      Parse attribute = (Parse)attributes[0];
      if (attribute.spelling != null)
        return non_terminal_component_of_type(f.FieldType, attribute.spelling);
      else
        return non_terminal_component_of_type(f.FieldType);
    }

    private static component create_reserved_word_component(FieldInfo f)
    {
      object[] attributes = f.GetCustomAttributes(typeof(Parse), false);
      if (attributes.Length != 1) grammar_error("Reserved word field must have a single attribute of type Parse(int, string)");
      Parse attribute = (Parse)attributes[0];
      if (attribute.spelling == null || attribute.spelling == "") grammar_error("Reserved word field must have a single attribute of type Parse(int, string)");
      return word(attribute.spelling);
    }

    private static component create_punctuation_component(FieldInfo f)
    {
      object[] attributes = f.GetCustomAttributes(typeof(Parse), false);
      if (attributes.Length != 1) grammar_error("Punctuation field must have a single attribute of type Parse(int, string)");
      Parse attribute = (Parse)attributes[0];
      if (attribute.spelling == null) grammar_error("Punctuation field must have a single attribute of type Parse(int, string)");
      return sym(attribute.spelling);
    }

    private static component create_list_component(FieldInfo f)
    {
      object[] attributes             = f.GetCustomAttributes(typeof(Parse), false);
      Parse    attribute              = (Parse)attributes[0];
      Type     element_type           = f.FieldType.GetGenericArguments()[0];
      Type     any_optional_type      = typeof(Iteration<>);
      if (!element_type.IsSubclassOf(typeof(parsable))) grammar_error("List element " + element_type.Name + " is not a subtype of parsible");
      Type     required_optional_type = any_optional_type.MakeGenericType(new Type[] { element_type });
      if (attribute.spelling != null)
        return non_terminal_component_of_type(required_optional_type, attribute.spelling);
      else
        return non_terminal_component_of_type(required_optional_type);
    }

    private static component create_alternative_component(FieldInfo f)
    {
      object[] attributes = f.GetCustomAttributes(typeof(Parse), false);
      if (attributes.Length != 1) grammar_error("Reserved word field must have a single attribute of type Parse(int, string)");
      Parse attribute = (Parse)attributes[0];
      if (attribute.spelling == null && attribute.alternatives == null) grammar_error("Enumerated field must have a single attribute of type Parse(int, string[])");
      string[] alternatives;
      if (attribute.spelling != null && attribute.spelling != "") alternatives = new string[] { attribute.spelling };
      else alternatives = attribute.alternatives;
      if (alternatives.Length == 0) grammar_error("Enumerated field must specify at least one alternative symbol");
      Array values = Enum.GetValues(f.FieldType);
      if (alternatives.Length != values.Length) grammar_error("Enumerated field: number of symbols does not match number of values in enumerated type. " + f.FieldType.Name);
      int min = (int)values.GetValue(0);
      if (min != 0) grammar_error("Enumerated field: enumerated values must be zero based and contiguous. " + f.FieldType.Name);
      int last = -1;
      foreach (var v in values)
      {
        int n = (int)v;
        if (n != last + 1) grammar_error("Enumerated field: enumerated values must be zero based and contiguous. " + f.FieldType.Name);
        last = n;
      }
      return alt(alternatives, f.FieldType);
    }
    
    internal static bool is_parsable_list(Type t)
    {
      bool   is_generic = t.IsGenericType;
      string name       = t.Name;
      return is_generic && name.StartsWith("List`");
    }
    
    internal static bool is_iteration(Type t)
    {
      bool   is_generic = t.IsGenericType;
      string name       = t.Name;
      return is_generic && name.StartsWith("Iteration`");
    }
    
    internal static List<FieldInfo> get_parsable_fields(Type t)
    {
      FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.GetCustomAttributes(typeof(Parse), false).Length > 0).ToArray();
      return fields.OrderBy(f =>((Parse)f.GetCustomAttributes(typeof(Parse), false)[0]).order).ToList();
    }
    
    internal static List<FieldInfo> get_marked_fields(Type t)
    {
      FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.GetCustomAttributes(typeof(Mark), false).Length > 0 && f.FieldType == typeof(source_position)).ToArray();
      return fields.ToList();
    }
    
    internal static void error(string message)
    {
      throw new parse_error(message, parse_error.error_kind.parser);
    }
    
    internal static void grammar_error(string message)
    {
      throw new parse_error(message, parse_error.error_kind.grammar);
    }
    
    internal static void source_builder_error(string message)
    {
      throw new parse_error(message, parse_error.error_kind.source_builder);
    }
  }
}
