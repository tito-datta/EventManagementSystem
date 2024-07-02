namespace user_service
{
    public class UserService
    {
        public User[] Get() => new User[]
        {
            new(){ Id="1", Name="Shana", Email="shana@bana.com"},
            new(){ Id="2", Name="Bal", Email="bal@bana.com"},
            new(){ Id="3", Name="Goru", Email="goru@bana.com"},
            new(){ Id="4", Name="Choom", Email="choom@bana.com"},
        };

        public User GetById(string id) => Get().SingleOrDefault(u => u.Id == id);
    }

    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
