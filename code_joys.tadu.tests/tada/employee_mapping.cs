namespace tada.tests 
{
class employee_mapping : table_mapping
{
  public employee_mapping() {
    table = "users";
    type = typeof(employee);
  }
}
}