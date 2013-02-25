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
}
}