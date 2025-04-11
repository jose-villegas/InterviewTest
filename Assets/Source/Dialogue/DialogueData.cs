using System.Collections.Generic;

namespace Dialogue
{
    public class Avatar
    {
        public string name { get; set; }
        public string url { get; set; }
        public string position { get; set; }
    }

    public class Dialogue
    {
        public string name { get; set; }
        public string text { get; set; }
    }

    public class Emojy
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    /// <summary>
    /// Represents the data structure for the dialogue data obtained from the game API.
    /// </summary>
    public class DialogueData
    {
        public List<Dialogue> dialogue { get; set; }
        public List<Emojy> emojies { get; set; }
        public List<Avatar> avatars { get; set; }
    }
}