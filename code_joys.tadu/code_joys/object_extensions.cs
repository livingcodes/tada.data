namespace code_joys
{
public static class object_extensions 
{
   public static bool equals_any(this object obj, params object[] items) {
      foreach (var item in items)
         if (obj.Equals(item))
            return true;
      return false;
   }
   public static bool is_set(this object obj) {
      return !(obj.not_set());
   }
   public static bool not_set(this object obj) {
      return (obj == null 
         || obj == System.DBNull.Value 
         || obj.ToString().Trim() == "");
   }
}
}