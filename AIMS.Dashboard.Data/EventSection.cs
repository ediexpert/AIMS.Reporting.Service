using System;
using System.ComponentModel.DataAnnotations;

namespace AIMS.Dashboards.Data
{
    public class EventSection
    {
        [Key]
        public int Id { get; set; }



        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public string CssClass { get; set; } = "btn btn-primary m-2";
        public int EventId { get; set; }
        public Event Event { get; set; }
    }
}
