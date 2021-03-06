﻿using System;
using Xamarin.Forms;

namespace Kala.Models
{
    public class Calendar
    {
        public string Day { get; set; }
        public string DayOfWeek { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Hours { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class CalItems
    {
        public string Label { get; set; }
        public string State { get; set; }
        public string Name { get; set; }
        public Grid Grid { get; set; }
    }
}
