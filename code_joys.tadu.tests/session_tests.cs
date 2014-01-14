using code_joys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using tada;

namespace tada.tests {
[TestClass]
public class session_tests : base_test {
   session session;
   user user;

   [TestInitialize]
   public void initialize() {
      session = new session();
      user = new user();
      user.email = "casey@code-joys.com";
      user.password = "password";
      user.age = 33;
   }
   [TestCleanup]
   public void cleanup() {
      session.delete<user>();
   }

   [TestMethod] public void execute_and_get_rows_affected() {
      var rows_affected = session.execute("insert into users (email, password) values ('{0}', '{1}')"
         .plug(user.email, user.password));
      assert(rows_affected == 1);
   }

   int insert() {
      return session.insert<user>(user);   
   }

   [TestMethod] public void all() {
      insert();
      var users = session.all<user>("select * from users");
      assert(users.Count > 0);
      assert(users[0].email != null);
   }

   [TestMethod] public void one() {
      insert();
      var _user = session.one<user>("select id, email, password from users where email='{0}'".plug(user.email));
      assert(user.email == _user.email);
   }

   [TestMethod] public void share_connection() {
      var email = user.email;
      var session = new session().open_connection();
         session.insert(user);
         user = session
            .param("@email", email)
            .one<user>("where email=@email");
      session.close_connection();
      assert(user.email == email);
   }

   [TestMethod] public void dispose_connection() {
      using (session.open_connection()) {
         session.insert(user);
         user = session
            .param("@email", user.email)
            .one<user>("where email=@email");
      }
      assert(user.email != null);
   }
  
   [TestMethod] public void domain_member_and_column_name_differ_but_are_mapped() {
      var age = user.age;
      session.insert(user);
      user = session
         .param("@email", user.email)
         .one<user>("where email = @email");
      assert(user.age == age);
   }

   [TestMethod] public void get_users_with_same_email_using_plug() {
      session.insert(user);
      var users = session.all<user>("where email = '{0}'".plug(user.email));
      assert(users.Count == 1);
   }

   [TestMethod] public void get_users_with_same_email_using_string_parameter() {
      session.insert(user);
      var users = session
         .param("@email", user.email)
         .all<user>("where email = @email");
      assert(users.Count == 1);
   }

   [TestMethod] public void get_users_with_same_id_using_int_parameter() {
      var id = session.insert(user);
      var users = session
         .param("@id", id)
         .all<user>("where id = @id");
      assert(users.Count == 1);
   }

   [TestMethod] public void rollback_uncommitted_transaction() {
      var id = session.insert(user);
      var email = user.email;

      var updated_email = "clown@yahoo.com";
      using (session.start_transaction())
         session
            .execute("update users set email = '{0}'"
            .plug(updated_email));
    
      user = session.one<user>(id);
      assert(user.email == email);
      assert(user.email != updated_email);
  }

  [TestMethod]
  public void commit_transaction() {
    using (var db = new session().start_transaction()) {
      db.execute("update users set email = 'clown1@yahoo.com' where id = 4");
      db.commit();
    }

    var user = new session()
      .param("@id", 4)
      .one<user>("where id=@id");

    assert(user.email == "clown1@yahoo.com");
  }

  [TestMethod]
  public void rollback_transaction_after_exception() {
    using (var db = new session().start_transaction()) {
      db.execute("update users set email = 'exception@domain.com' where id = 5");
      throw new Exception("This exception should cause rollback");
      db.commit();
    }
    
    var user = new session()
      .one<user>("where id = 5");

    assert(user.email == "c@b.com");
  }

  [TestMethod] public void cache() {
    var db = new session();
    var count = db
      .cache("count")
      .one<int>("select count(email) from users");
    
    db.execute("insert into users (email, password) values ('count3@dracula.com', 'blood')");

    var new_count = db
      .cache("count")
      .one<int>("select count(email) from users");

    assert(count == new_count);

    var diff = db.one<int>("select count(email) from users");

    assert(diff > count);
  }

  [TestMethod] public void get_email() {
    var db = new session();
    var emails = db.all<string>("select email from users");
    assert(emails.Count > 1);

    var email = db.one<string>("select email from users where id = 2");
    assert(email.Contains("@"));
  }
}
}