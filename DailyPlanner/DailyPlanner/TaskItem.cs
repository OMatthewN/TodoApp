using System;

namespace DailyPlanner
{
    public class TaskItem
    {
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string Category { get; set; }

        public TaskItem(string description, DateTime dateTime, string category = "Другое")
        {
            Description = description;
            DateTime = dateTime;
            Category = category;
        }
    }
}
