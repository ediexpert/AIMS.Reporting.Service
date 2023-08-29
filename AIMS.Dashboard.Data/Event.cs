using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace AIMS.Dashboards.Data
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
        public string UrduName { get; set; }
        public DateTime StartDate { get; set; }
        public int DurationInDays { get; set; }
        public bool IsActive { get; set; } // only one event can be active at a time for now

        public ICollection<EventSection> EventSections { get; set; }

        //public static Event AddEvent(ApplicationDbContext dbContext, string eventName, DateTime startDateOfTheEvent, int durationOfTheEventInDays, bool setActive = true)
        //{
        //    Event e = new Event { Name = eventName, DurationInDays = durationOfTheEventInDays, StartDate = startDateOfTheEvent.Date, IsActive = setActive };
        //    dbContext.Events.Add(e);
        //    dbContext.SaveChanges();
        //    if (setActive)
        //    {
        //        // Only one event can be active at a time
        //        var previouslyActiveEvents = dbContext.Events.Where(x => x != e && x.IsActive == true);
        //        if (previouslyActiveEvents.Count() != 0)
        //        {
        //            foreach (var item in previouslyActiveEvents)
        //            {
        //                item.IsActive = false;
        //            }
        //            dbContext.SaveChanges();
        //        }

        //    }
        //    return e;
        //}
    }
}

