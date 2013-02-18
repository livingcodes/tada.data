using System.Collections.Generic;
using System.Configuration;

namespace tada.tests
{
public class employee_db : session_base 
{
  public employee_db() :
    base(
      new sql_server_connection_factory(ConfigurationManager.ConnectionStrings["default"].ToString()),
      new table_to_class_mapper(
        new List<table_mapping>() {new employee_mapping()}
      ),
      new in_memory_cache()
    ) { }
}
}