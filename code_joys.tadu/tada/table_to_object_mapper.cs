using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace tada {

class table_to_object_mapper : i_table_to_object_mapper {
  public List<t> map<t>(DataTable table) {
    var items = new List<t>();
    var fields = typeof(t).GetFields(BindingFlags.Public | BindingFlags.Instance);
    foreach (DataRow row in table.Rows) {
      var item = default(t);
      //if (item == null)
      //  item = (t)typeof(t).GetConstructor(null).Invoke(null);
      foreach (var field in fields)
        field.SetValueDirect(__makeref(item), row[field.Name]);
      items.Add(item);
    }
    return items;
  }
}

public interface i_table_to_object_mapper {
  List<t> map<t>(DataTable table);
}

}