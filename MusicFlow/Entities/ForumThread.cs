﻿namespace MusicFlow.Entities
{
    public class ForumThread
    {
        public int Tid { get; set; } // thread id
        public string Topic { get; set; }
        public int Oid { get; set; } // owner id
        public bool Pinned { get; set; }
    }
}
