using System;
using System.ComponentModel.DataAnnotations;

namespace AIMS.Dashboards.Data
{
    public class Hazri : AttendanceTanzeem
    {
        [Key]
        public int Id { get; set; }
        public Days Days { get; set; }
        public int EventId { get; set; }
        public EventSection Eventsection { get; set; }
    }
}

