using System;

namespace CybersecurityChatbot
{
    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Reminder { get; set; } = string.Empty;
        public DateTime? ReminderDate { get; set; }
        public bool IsComplete { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status => IsComplete ? "Complete" : "Pending";

        public string ReminderDisplay
        {
            get
            {
                if (ReminderDate.HasValue)
                {
                    return $"{Reminder} ({ReminderDate.Value:dd MMM yyyy})";
                }

                return string.IsNullOrWhiteSpace(Reminder) ? "None" : Reminder;
            }
        }
    }
}
