using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tada.tests {
public class base_test {
  public void assert(bool condition) {
    Assert.IsTrue(condition);
  }
}
}