using System;

namespace ConferenceBot.Model
{
    [Serializable]
    public class Session
    {
        public string Title { get; set; }
        public string Speaker { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public string Room { get; set; }
    }
}
