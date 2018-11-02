namespace Kala.Models
{
    public class Events
    {
        public string Topic { get; set; }
        public string Payload { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class Payload
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}