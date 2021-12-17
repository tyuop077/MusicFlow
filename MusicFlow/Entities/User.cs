namespace MusicFlow.Entities
{
    // View "/init.sql" on how to initialize SQL Database for this entity
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public string Avatar { get; set; }
        public UserRole Role { get; set; } = 0;
    }
    public enum UserRole
    {
        User,
        Staff
    }
}
