using code_joys;

namespace tada 
{
// these methods require the item to have an id
public partial class session_base 
{
   /// <summary>Gets one item by id</summary>
   public t one<t>(int id) {
      return one<t>("where id={0}".plug(id));
   }
}
}
