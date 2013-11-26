using System;
using System.Collections.Generic;

namespace code_joys
{
public static class list_extensions
{
   public static void add<t>(this List<t> list, params t[] items) {
      list.AddRange(items);
   }

   public static List<t> duplicates<t>(this List<t> list,
      Func<t, t, bool> comparison
   ) {
      var duplicates = new List<t>();
      for (var i=0; i<list.Count-1; i++)
         for (var j=i+1; j<list.Count; j++)
            if (i != j)
               if (comparison(list[i], list[j])) {
                  duplicates.Add(list[j]);
                  break;
               }
      return duplicates;
   }
}
}