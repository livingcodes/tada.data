using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tada.tests 
{
[TestClass]
public class employee_session_tests : base_test
{
  [TestMethod]
  public void get_employee() {
    var db = new employee_db();
    var employee = db.one<employee>("where email='casey@codejoys.com'");
    assert(employee != null);
  }
}
}