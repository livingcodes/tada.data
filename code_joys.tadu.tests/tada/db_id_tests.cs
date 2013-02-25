using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tada.tests 
{
[TestClass]
public class db_id_tests : base_test
{
   [TestMethod] public void insert() {
      var user = new user();
      user.email = "insert@test.com";
      user.password = "insert";

      var db = new session();
      var rows_affected = db.insert<user>(user);
      assert(rows_affected == 1);
   }
}
}
