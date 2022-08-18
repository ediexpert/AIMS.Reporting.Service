using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AIMS.Reporting.WorkerService.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationInDays { get; set; }
        public bool IsActive { get; set; } // only one event can be active at a time for now


        public static Event AddEvent(AppDbContext dbContext, string eventName, DateTime startDateOfTheEvent, int durationOfTheEventInDays, bool setActive = true)
        {
            Event e = new Event { Name = eventName, DurationInDays = durationOfTheEventInDays, StartDate = startDateOfTheEvent.Date, IsActive = setActive };
            dbContext.Events.Add(e);
            dbContext.SaveChanges();
            if (setActive)
            {
                // Only one event can be active at a time
                var previouslyActiveEvents = dbContext.Events.Where(x => x != e && x.IsActive == true);
                if (previouslyActiveEvents.Count() != 0)
                {
                    foreach (var item in previouslyActiveEvents)
                    {
                        item.IsActive = false;
                    }
                    dbContext.SaveChanges();
                }

            }
            return e;
        }
    }
}

