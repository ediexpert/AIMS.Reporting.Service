using System;
using System.ComponentModel.DataAnnotations;

namespace AIMS.Dashboards.Data
{
    public class HazriNigraniData
    {
        [Key]
        public int Id { get; set; }
        public int EventId { get; set; }

        public Days Days { get; set; }

        [MaxLength(200)]
        public string Designation { get; set; }

        public int Total { get; set; }
        public int Attended { get; set; }
    }
}

