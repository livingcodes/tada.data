using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tada.tests 
{
[TestClass]
public class employee_session_tests : base_test
{
  [TestMethod] public void insert_employee() {
    var db = new employee_db();
    var count = db.delete<employee>();
    assert(count > 0);
    
    var employee = new employee();
    employee.email = "casey@code-joys.com";
    employee.password = "password";
    var id = db.insert(employee);
    assert(id > 1);

    var employees = db.all<employee>();
    assert(employees.Count > 0);
    assert(employee.password == "password");

    employee.email = "changed@code-joys.com";
    db.update(employee);

    employee = db.one<employee>("where email='changed@code-joys.com'");
    assert(employee.email == "changed@code-joys.com");
  }

  [TestMethod] public void get_employee() {
    var db = new employee_db();
    var employee = db.one<employee>("where email='casey@code-joys.com'");
    assert(employee != null);
  }
}
}