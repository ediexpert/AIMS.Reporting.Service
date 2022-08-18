using System;
using System.ComponentModel.DataAnnotations;

namespace AIMS.Reporting.WorkerService.Models
{
    public enum Days
    {
        FirstDay = 1,
        SecondDay = 2,
        ThirdDay = 3,
        AllDays = 4
    }
    public class Hazri : AttendanceTanzeem
    {
        [Key]
        public int Id { get; set; }
        public Days Days { get; set; }
    }
}

