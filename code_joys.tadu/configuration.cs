using System.Configuration;
namespace code_joys.tadu {
public static class configuration {
  static i_settings _settings = null;
  static i_connection_factory _connection_factory = null;

  public static i_settings settings { 
    get { 
      if (_settings == null) {
        var connection_string = ConfigurationManager.ConnectionStrings["default"].ConnectionString;      
        _settings = new default_settings(connection_string); 
      }
      return _settings;
    }
    set { _settings = value; }
  }
  public static i_connection_factory connection_factory { 
    get {
      if (_connection_factory == null)
        _connection_factory = new connection_factory(settings.connection_string);
      return _connection_factory;
    } 
    set { _connection_factory = value; }
  }
}
}