using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_joys;

namespace tada.tests
{
[TestClass]
public class struct_tests : base_test
{
   session db;

   public struct_tests() {
      db = new session();      
   }
   
   [TestMethod] public void insert_update_select_delete() {
      var user = new user();
      user.email = "insert@update.com";
      user.password = "password";

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
      var user = new user() { email="user@domain.com", password="pw" };
      db.insert<user>(user);
   }

   [TestMethod] public void sql_select_statement() {
      var users = db.all<user>("select * from users");
      assert(users.Count > 0);
   }

   [TestMethod] public void select_all_shorthand() {
      var users = db.all<user>("where email='user@domain.com'");
      assert(users.Count > 0);
   }

   [TestMethod] public void select_one_shorthand() {
      var user = db.one<user>("where email='user@domain.com'");
      assert(user.id > 0);
   }

   [TestMethod] public void select_all_with_no_arg() {
      var users = db.all<user>();
      assert(users.Count > 0);
   }

   [TestMethod] public void field_set() {
      var user = db.all<user>()[0];
      assert(user.email.is_set());
   }

   [TestMethod] public void property_set() {
      var user = db.all<user>()[0];
      assert(user.password.is_set());
   }

}
}