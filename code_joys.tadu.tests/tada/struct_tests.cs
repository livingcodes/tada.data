using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tada.tests
{
[TestClass]
public class struct_tests : base_test
{
   [TestMethod] public void insert_update_select_delete() {
      var user = new user();
      user.email = "insert@update.com";
      user.password = "password";

      var db = new session();

      user.id = db.insert(user);
      assert(user.id > 0);

      user.password = "new";
      var rows_affected = db.update(user);
      assert(rows_affected == 1);

      user = db.one<user>(user.id);
      assert(user.password == "new");

      rows_affected = db.delete<user>(user.id);
      assert(rows_affected == 1);

      // throws exception since user does not exist
      //user = db.one<user>(user.id);
      //assert(user.id == 0);
   }

   [TestMethod] public void prepare_database() {
      
   }
}
}