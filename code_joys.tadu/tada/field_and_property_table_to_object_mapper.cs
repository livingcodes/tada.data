using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using code_joys;

namespace tada
{
// sets public fields and properties
public class field_and_property_table_to_class_mapper : i_table_to_object_mapper
{
   public field_and_property_table_to_class_mapper(List<table_mapping> table_mappings) {
      _table_mappings = table_mappings;
   }

   public List<t> map<t>(DataTable table) {
      var items = new List<t>();

      var fields = typeof(t).GetFields(BindingFlags.Public | BindingFlags.Instance);
      var properties = typeof(t).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      foreach (DataRow row in table.Rows) {
         var item = default(t);
         item = (t)typeof(t).GetConstructor(System.Type.EmptyTypes).Invoke(null);
         foreach (var field in fields) {
            if (table.Columns.Contains(field.Name))
               field.SetValueDirect(__makeref(item), row[field.Name]);
            else {
               var table_mapping = table_mappings.FirstOrDefault(m => m.type == typeof(t));
               if (table_mapping != null)
                  field.SetValueDirect(__makeref(item), row[table_mapping.get_column_name(field.Name)]);
            }
         }
         foreach (var property in properties) {
            if (table.Columns.Contains(property.Name))
               property.SetValue(item, row[property.Name]);
            else {
               var table_mapping = table_mappings.FirstOrDefault(m => m.type == typeof(t));
               if (table_mapping != null) {
                  var column_mapping = table_mapping.column_mappings
                     .FirstOrDefault(c => c.domain_member == property.Name);
                  var column_name = table_mapping.get_column_name(property.Name);
                  if (column_name != null) {
                     property.SetValue(item, row[column_name]);
                  }
               }
            }
         }
         items.Add(item);
      }
      
      return items;
   }

   public List<table_mapping> table_mappings
   {
      get { return _table_mappings; }
   }
   List<table_mapping> _table_mappings;
}
}