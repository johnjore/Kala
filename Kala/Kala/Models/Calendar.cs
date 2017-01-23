using System;
using Xamarin.Forms;

namespace Kala.Models
{
    public class calendar
    {
        public string Day { get; set; }
        public string DayOfWeek { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Hours { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class calItems
    {
        public string Label { get; set; }
        public string State { get; set; }
        public string Link { get; set; }
        
        public Grid grid { get; set; }
        public int px { get; set; }
        public int py { get; set; }
        public int sx { get; set; }
        public int sy { get; set; }
    }
}
