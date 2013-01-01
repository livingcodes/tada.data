using System;

namespace code_joys
{
public static class string_extensions
{
   public static string plug(this string format, params object[] values)
   {
      return string.Format(format, values);
   }
}
}