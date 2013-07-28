using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace tada 
{
// sets public fields
public class table_to_class_mapper : i_table_to_object_mapper
{
  public table_to_class_mapper(List<table_mapping> table_mappings) {
    _table_mappings = table_mappings;
  }

  public List<t> map<t>(DataTable table) {
    var items = new List<t>();
    var is_single_value = false;
    if (typeof(t) == typeof(int) || typeof(t) == typeof(double) 
    ||  typeof(t) == typeof(DateTime) || typeof(t) == typeof(string))
      is_single_value = true;
    
    var fields = typeof(t).GetFields(BindingFlags.Public | BindingFlags.Instance);
    var properties = typeof(t).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                              .Where(p => p.CanWrite && p.CanRead);
    foreach (DataRow row in table.Rows) {
      if (is_single_value) {
        items.Add((t)row[0]);
        continue;
      }
      
      var item = default(t);
      item = (t)typeof(t).GetConstructor(System.Type.EmptyTypes).Invoke(null);
      foreach (var field in fields) {
        if (table.Columns.Contains(field.Name))
          field.SetValueDirect(__makeref(item), row[field.Name]);
        else {
          var table_mapping = table_mappings.First(m => m.type == typeof(t));
          field.SetValueDirect(__makeref(item), row[table_mapping.get_column_name(field.Name)]);
        }
      }
      foreach (var property in properties) {
        if (table.Columns.Contains(property.Name))
          property.SetValue(item, row[property.Name]);
        else {
          var table_mapping = table_mappings.First(m => m.type == typeof(t));
          property.SetValue(item, row[table_mapping.get_column_name(property.Name)]);
        }
      }
      items.Add(item);
    }
    return items;
  }

  public List<table_mapping> table_mappings {
    get { return _table_mappings; }
  }
  List<table_mapping> _table_mappings;
}
}