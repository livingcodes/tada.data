using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace code_joys.tadu.tests {
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

  [TestMethod] public void share_connection() {
    var session = new session().open_connection();
    session.execute("insert into users (email, password) values ('{0}', '{1}')".plug("c@b.com", "ca"));
    var user = session.one<user>("select * from users where email='c@j.com'");
    session.close_connection();
  }

  [TestMethod] public void dispose_connection() {
    user user;
    using (var session = new session().open_connection()) {
      session.execute("insert into users (email, password) values ('{0}', '{1}')".plug("c@b.com", "ca"));
      user = session.one<user>("select * from users where email='c@j.com'");
    }
    assert(user.email == "c@j.com");
  }
}

struct user {
  public string email, password;
}
}