using code_joys;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using tada;

namespace tada.tests {
[TestClass]
public class session_tests : base_tests {
  [TestMethod, Ignore] public void execute() {
    var session = new session();
    var rows_affected = session.execute("insert into users (email, password) values ('casey@codejoys.com', 'password')");
    assert(rows_affected == 1);
  }

  [TestMethod] public void all() {
    var session = new session();
    var users = session.all<user>("select * from users");
    assert(users.Count > 0);
    assert(users[0].email != null);
  }

  [TestMethod] public void one() {
    var session = new session();
    var email = "casey@codejoys.com";
    var user = session.one<user>("select email, password from users where email='{0}'".plug(email));
    assert(user.email == email);
  }

  [TestMethod, Ignore] public void share_connection() {
    var session = new session().open_connection();
    session.execute("insert into users (email, password) values ('{0}', '{1}')".plug("c@b.com", "ca"));
    var user = session.one<user>("select * from users where email='c@j.com'");
    session.close_connection();
  }

  [TestMethod, Ignore] public void dispose_connection() {
    user user;
    using (var session = new session().open_connection()) {
      session.execute("insert into users (email, password) values ('{0}', '{1}')".plug("c@b.com", "ca"));
      user = session.one<user>("select * from users where email='c@j.com'");
    }
    assert(user.email == "c@j.com");
  }
  
  [TestMethod] public void domain_member_and_column_name_differ_but_are_mapped() {
    var session = new session();
    var user = session.one<user>("select email as email_address, password from users where email = 'casey@codejoys.com'");
    assert(user.email == "casey@codejoys.com");
  }

  [TestMethod] public void get_users_with_same_email_using_plug() {
    var email = "casey@codejoys.com";
    var session = new session();
    var users = session.all<user>("where email = '{0}'".plug(email));
    assert(users.Count == 1);
  }

  [TestMethod] public void get_users_with_same_email_using_parameter() {
    var email = "casey@codejoys.com";
    var id = 2;

    var users = new session()
      .param("@email", email)
      .param("@id", id)
      .all<user>("where email = @email and id = @id");
    assert(users.Count == 1);
  }

  [TestMethod] public void get_users_with_same_email_using_int_parameter() {
    var id = 8;
    var session = new session();
    var users = session
      .param("@id", id)
      .all<user>("where id = @id");
    assert(users.Count == 1);
  }

  [TestMethod]
  public void rollback_uncommitted_transaction() {
    using (var db = new session().start_transaction())
      db.execute("update users set email = 'clown@yahoo.com' where id = 5");
    
    var user = new session()
      .param("@id", 5)
      .one<user>("where id=@id");

    assert(user.email == "c@b.com");
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