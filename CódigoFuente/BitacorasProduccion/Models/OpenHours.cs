using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class OpenHours
    {
        public OpenHours(string openHours)
        {
            var openClose = openHours.Split(new[] { ':', ';' });
            StartHour = int.Parse(openClose[0]);
            StartMinute = int.Parse(openClose[1]);
            EndHour = int.Parse(openClose[2]);
            EndMinute = int.Parse(openClose[3]);
        }

        public int StartHour
        {
            get;
            set;
        }

        public int StartMinute { get; set; }

        public int EndHour
        {
            get;
            set;
        }

        public int EndMinute
        {
            get;
            set;
        }

    }
}