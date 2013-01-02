using tada;

namespace tada.tests
{
class user_mapping : table_mapping
{
   public user_mapping()
   {
      table = "users";
      type = typeof(user);
      map("id", "user_id");
   }
}
}