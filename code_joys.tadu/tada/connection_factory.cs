using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;

namespace tada {
public interface i_connection_factory {
  IDbConnection create();
}

public class sql_server_connection_factory : i_connection_factory {
  string connection_string;

  public sql_server_connection_factory(string connection_string) {
    this.connection_string = connection_string;
  }
  public IDbConnection create() {
    return new SqlConnection(connection_string);
  }
}

public class sql_ce_connection_factory : i_connection_factory {
   string connection_string;

   public sql_ce_connection_factory(string connection_string) {
      this.connection_string = connection_string;
   }
   public IDbConnection create() {
      // must add reference to System.Data.SqlServerCe from Program Files\Microsoft SQL Server Compact Edition\v4.0\Private
      return new SqlCeConnection(connection_string);
   }
}
}