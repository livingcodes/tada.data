using System;
using System.Collections.Generic;

namespace code_joys
{
public static class list_extensions
{
   public static void add<t>(this List<t> list, params t[] items) {
      list.AddRange(items);
   }
}
}