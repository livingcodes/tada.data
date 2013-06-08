using System.Collections.Generic;
using System.Data;

namespace tada
{
public interface i_table_to_object_mapper
{
   List<t> map<t>(DataTable table);
   List<table_mapping> table_mappings { get; }
}
}
