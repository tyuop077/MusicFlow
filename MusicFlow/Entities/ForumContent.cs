namespace MusicFlow.Entities
{
    public class ForumContent
    {
        public int Id { get; set; } // message id
        public int Tid { get; set; } // thread id
        public int Oid { get; set; } // message owner id
        public string Content { get; set; }
        public int Rid { get; set; } // reply id
        public User Owner { get; set; }
    }
}
