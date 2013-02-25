using code_joys;
using System;
using System.Linq;
using System.Reflection;

namespace tada 
{
// these methods require the item to have an id
public partial class session_base 
{
   /// <summary>gets one item by id</summary>
   public t one<t>(int id) {
      return one<t>("where id={0}".plug(id));
   }

   /// <summary>inserts item and returns id</summary>
   public int insert<t>(t item) {
      //insert into table (col1, col2) values (val1, val2)

      // generate sql
      var mapping = get_table_mapping<t>();
      if (mapping == null)
         throw new Exception("No mapping exists for type '{0}'".plug(typeof(t).ToString()));
      string column_sql = "";
      string value_sql = "";
      string quote = "";

      var fields = typeof(t).GetFields(BindingFlags.Public | BindingFlags.Instance);

      foreach (var field in fields) {
         if (field.FieldType.equals_any(typeof(string), typeof(DateTime)))
               quote = "'";
         
         var column_mapping = mapping.column_mappings.FirstOrDefault(m => m.domain_member == field.Name);

         if (column_mapping != null)
            column_sql += column_mapping.column_name + ", ";
         else
            column_sql += field.Name + ", ";
         value_sql += "{0}{1}{0}, ".plug(quote, field.GetValue(item));
      }

      column_sql = column_sql.Remove(column_sql.Length-2);
      value_sql = value_sql.Remove(value_sql.Length-2);

      var sql = "insert into {0} ({1}) values ({2})"
         .plug(mapping.table, column_sql, value_sql);

      // insert item and get id
      var id = one<int>(sql + @"
         declare @id int
         set @id = @@identity
         select @id");

      return id;
   }

   /// <summary>update item based on id</summary>
   public int update<t>(t item) {
      //update table set col1 = val1, col2 = val2
      var mapping = get_table_mapping<t>();
      string sql = "";
      string id = "";

      var fields = typeof(t).GetFields(BindingFlags.Public | BindingFlags.Instance);
      foreach (var field in fields) {
         if (field.Name.ToLower() == "id") {
            id = field.GetValue(item).ToString();
            continue;
         }
         var column_mapping = mapping.column_mappings
            .FirstOrDefault(m => m.domain_member == field.Name);
         var quote = "";
         if (field.FieldType.equals_any(typeof(string), typeof(DateTime)))
            quote = "'";
         if (column_mapping != null)
            sql += "{0}={1}{2}{1}, ".plug(column_mapping.column_name, quote, field.GetValue(item));
         else
            sql += "{0}={1}{2}{1}, ".plug(field.Name, quote, field.GetValue(item));
      }
      sql = sql.Remove(sql.Length-2);
      sql = "update {0} set {1} where id={2}"
         .plug(mapping.table, sql, id);
      return execute(sql);
   }

   public int delete<t>(int id) {
      var mapping = get_table_mapping<t>();
      return execute("delete from {0} where id={1}"
         .plug(mapping.table, id));
   }

   table_mapping get_table_mapping<t>() {
      return mapper.table_mappings.FirstOrDefault(m => m.type.Equals(typeof(t)));
   }
}
}