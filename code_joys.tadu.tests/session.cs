using System;
using tada;

namespace tada.tests
{
public class session : session_base
{
   public session() : 
   base(new sql_server_connection_factory(System.Configuration.ConfigurationManager.ConnectionStrings["default"].ToString()))
   {
      
   }
}
}