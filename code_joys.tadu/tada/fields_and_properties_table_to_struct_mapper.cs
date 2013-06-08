using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace tada
{
   // map table to struct
   // first attempt match domain member to column name else match based on table mappings
   // if object field does not have a matching column name then throw an exception
   public class fields_and_properties_table_to_struct_mapper : i_table_to_object_mapper
   {
      List<table_mapping> _table_mappings;

      public fields_and_properties_table_to_struct_mapper(List<table_mapping> table_mappings) {
         _table_mappings = table_mappings;
      }

      public List<table_mapping> table_mappings { get { return _table_mappings; } }

      public List<t> map<t>(DataTable table)
      {
         var is_single_value = false;
         var items = new List<t>();
         if (typeof(t) == typeof(int) || typeof(t) == typeof(double)
         || typeof(t) == typeof(DateTime) || typeof(t) == typeof(string))
            is_single_value = true;

         var fields = typeof(t).GetFields(BindingFlags.Public | BindingFlags.Instance);
         foreach (DataRow row in table.Rows)
         {
            if (is_single_value)
            {
               items.Add((t)row[0]);
               continue;
            }
            var item = default(t);
            //if (item == null)
            //  item = (t)typeof(t).GetConstructor(null).Invoke(null);
            foreach (var field in fields)
            {
               if (table.Columns.Contains(field.Name))
                  field.SetValueDirect(__makeref(item), row[field.Name]);
               else
               {
                  var table_mapping = table_mappings.First(m => m.type == typeof(t));
                  field.SetValueDirect(__makeref(item), row[table_mapping.get_column_name(field.Name)]);
               }
            }
            items.Add(item);
         }
         return items;
      }
   }
}