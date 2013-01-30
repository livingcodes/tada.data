using System;
using System.Web.Caching;

namespace tada 
{
public interface i_cache {
  void set(string key, object value, DateTime? expiration = null);
  object get(string key);
}
public class web_cache : i_cache {
  Cache c = new Cache();

  public void set(string key, object value, DateTime? expiration = null) {
    DateTime exp = DateTime.Now.AddMinutes(1);
    if (expiration.HasValue)
      exp = expiration.Value;
    c.Insert(key, value, null, exp, Cache.NoSlidingExpiration);
  }
  public object get(string key) {
    return c.Get(key);
  }
}

public class in_memory_cache : i_cache {
  System.Runtime.Caching.MemoryCache cache = new System.Runtime.Caching.MemoryCache("debuggin");

  public void set(string key, object value, DateTime? expiration = null) {
    DateTime exp = expiration.HasValue 
                 ? expiration.Value 
                 : DateTime.Now.AddMinutes(1);
    cache.Set(key, value, exp);
  }
  public object get(string key) {
    return cache.Get(key);
  }
}
}