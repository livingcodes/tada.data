using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tada.tests
{
[TestClass]
public class db_id_tests : base_test
{
   [TestMethod] public void insert_update_delete() {
      var user = new user();
      user.email = "insert@update.com";
      user.password = "password";

      var db = new session();

      user.id = db.insert<user>(user);
      assert(user.id > 0);

      var rows_affected = db.update(user);
      assert(rows_affected == 1);

      rows_affected = db.delete<user>(user.id);
      assert(rows_affected == 1);
   }

   [TestMethod] public void save() {
      var user = new user();
      user.email = "save@test.com";
      user.password = "save - insert";

      var db = new session();
      user.id = db.save(user); // insert
      assert(user.id > 0);

      user.password = "save - update";
      db.save(user); // update
      assert(user.password == "save - update");

      db.delete<user>(user.id);
   }
}
}