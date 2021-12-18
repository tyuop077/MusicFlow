using MusicFlow.Utils;

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
        public string AvatarURL
        {
            get
            {
                return Avatar is not null ? $"https://i.imgur.com/{Avatar}" : $"https://www.gravatar.com/avatar/{AvatarUtil.CreateMD5(Email).ToLower()}?d=https%3A%2F%2Fui-avatars.com%2Fapi%2F/{Username}/128/20B2AA";
            }
        }
    }
    public enum UserRole
    {
        User,
        Staff
    }
}
