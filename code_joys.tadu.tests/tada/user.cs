namespace tada.tests
{
struct user
{
   public int id;
   public string email;
   public string password { get; set; }
   public int age;
}

class user_mapping : table_mapping
{
   public user_mapping()
   {
      table = "users";
      type = typeof(user);
      map("age", "age_in_years");
   }
}
}