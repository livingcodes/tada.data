using System;
using System.Collections.Generic;
using System.Data;

namespace tada 
{
public class table_to_object_mapper : i_table_to_object_mapper
{
  public table_to_object_mapper(List<table_mapping> table_mappings) {
    _table_mappings = table_mappings;    
  }
  public List<t> map<t>(DataTable table) {
    if (typeof(t).IsClass)
      return new table_to_class_mapper(table_mappings).map<t>(table);
    else
      return new table_to_struct_mapper(table_mappings).map<t>(table);
  }

  public List<table_mapping> table_mappings {
    get { return _table_mappings; }
  }
  List<table_mapping> _table_mappings;
}
}