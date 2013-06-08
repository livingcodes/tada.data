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
    
    var fields = typeof(t).GetFields(BindingFlags.Public | BindingFlags.Instance);
    foreach (DataRow row in table.Rows) {
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