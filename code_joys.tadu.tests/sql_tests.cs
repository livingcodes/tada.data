using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace code_joys.tadu.tests {

[TestClass]
public class sql_tests : base_tests {
  [TestMethod]
  public void select_from_where_order() {
    var sql = new sql()
      .select("*").from("users")
      .where("last_name='@last_name'")
      .order_by("first_name desc").ToString();
    assert(sql == "select * from users where last_name='@last_name' order by first_name desc");
  }
}
}