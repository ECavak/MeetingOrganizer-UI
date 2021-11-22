using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingOrganizer.Models
{
    public class Meeting
    {
        public int id { get; set; }
        public string subject { get; set; }
        public string startDate { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string participants { get; set; }
    }
}
