namespace code_joys.tadu
{
public class sql {
  string _sql, _select, _from, _where, _order;

  public sql select(string select_sql) {
    _select = "select " + select_sql;
    return this;
  }
  public sql from(string from_sql) {
    _from = "from " + from_sql;
    return this;
  }
  public sql where(string where_sql) {
    _where = "where " + where_sql;
    return this;
  }
  public sql order_by(string order_sql) {
    _order = "order by " + order_sql;
    return this;
  }
  public override string ToString() {
    _sql = "{0} {1} {2} {3}".plug(_select, _from, _where, _order);
    return _sql;
  }
}
}

namespace code_joys
{
public static class string_extensions {
  public static string plug(this string format, params object[] values) {
    return string.Format(format, values);
  }
}
}