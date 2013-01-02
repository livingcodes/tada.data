using System;
using System.Collections.Generic;
using System.Configuration;
using tada;

namespace tada.tests
{
public class session : session_base
{
   public session() : 
   base(new sql_server_connection_factory(ConfigurationManager.ConnectionStrings["default"].ToString()),
        new table_to_struct_mapper(
            new app_table_mappings()
        )
   )
   { }
}

// application
class app_table_mappings : List<table_mapping>
{
   public app_table_mappings() {
      Add(new user_mapping());
   }
}
}