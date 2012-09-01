using System.Data;
using System.Data.SqlClient;

namespace code_joys.tadu {
public interface i_connection_factory {
  IDbConnection create();
}

public class connection_factory : i_connection_factory {
  string connection_string;

  public connection_factory(string connection_string) {
    this.connection_string = connection_string;
  }
  public IDbConnection create() {
    return new SqlConnection(connection_string);
  }
}
}