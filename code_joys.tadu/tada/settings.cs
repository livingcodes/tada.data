namespace tada {
public interface i_settings {
  string connection_string { get; }
}

public class default_settings : i_settings {
  string _connection_string;
  public default_settings(string connection_string) { _connection_string = connection_string; }
  public string connection_string { get { return _connection_string; } }
}
}