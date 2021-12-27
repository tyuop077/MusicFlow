using System;

namespace MusicFlow.Entities
{
    public class MusicLibrary
    {
        public Guid Id { get; set; } // music content id
        public int Oid { get; set; } // owner id
        public string Name { get; set; }
        public string Artists { get; set; }
    }
}
