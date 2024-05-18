namespace EventSchedulePro.Data
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int GroupID { get; set; }
        public string RoleUser { get; set; } 

        public string PasswordHash { get; set; }
    }
}
