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
      var id = db.insert(user);
      assert(id > 0);

      var rows_affected = db.delete<user>(id);
      assert(rows_affected == 1);
   }

   [TestMethod] public void update() {
      var user = new user();
      user.id = 11;
      user.email = "update@test.com";
      user.password = "update";

      var db = new session();
      var rows_affected = db.update(user);
      assert(rows_affected == 1);
   }
}
}