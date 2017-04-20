namespace Kala.Models
{
    public class Events
    {
        public string topic { get; set; }
        public string payload { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Payload
    {
        public string type { get; set; }
        public string value { get; set; }
    }
}