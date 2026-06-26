using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CybersecurityChatbot
{
    public class TaskManager
    {
        private readonly TaskStorageHelper _storage;

        public string LastError => _storage.LastError;

        public TaskManager(TaskStorageHelper? storage = null)
        {
            _storage = storage ?? new TaskStorageHelper();
        }

        public List<CyberTask> GetAllTasks()
        {
            return _storage.LoadTasks();
        }

        public CyberTask? AddTask(string title, string description, string reminder)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return null;
            }

            string safeDescription = string.IsNullOrWhiteSpace(description)
                ? CreateDescription(title)
                : description.Trim();

            DateTime? reminderDate = ParseReminderDate(reminder);
            return _storage.AddTask(title, safeDescription, reminder ?? string.Empty, reminderDate);
        }

        public bool MarkAsComplete(int id)
        {
            return _storage.MarkAsComplete(id);
        }

        public bool UpdateReminder(int id, string reminder)
        {
            return _storage.UpdateReminder(id, reminder, ParseReminderDate(reminder));
        }

        public bool DeleteTask(int id)
        {
            return _storage.DeleteTask(id);
        }

        public string CreateDescription(string title)
        {
            string lowerTitle = title.ToLowerInvariant();

            if (lowerTitle.Contains("password"))
            {
                return "Review password security and make sure each account uses a strong, unique password.";
            }

            if (lowerTitle.Contains("privacy"))
            {
                return "Review account privacy settings to ensure personal information is protected.";
            }

            if (lowerTitle.Contains("2fa") || lowerTitle.Contains("two-factor") || lowerTitle.Contains("two factor"))
            {
                return "Enable two-factor authentication on important accounts for additional protection.";
            }

            if (lowerTitle.Contains("update") || lowerTitle.Contains("patch"))
            {
                return "Install security updates so known vulnerabilities are fixed.";
            }

            if (lowerTitle.Contains("backup"))
            {
                return "Create and verify a secure backup of important files.";
            }

            return $"Complete the cybersecurity task: {title.Trim()}.";
        }

        private DateTime? ParseReminderDate(string? reminder)
        {
            if (string.IsNullOrWhiteSpace(reminder))
            {
                return null;
            }

            string lower = reminder.ToLowerInvariant().Trim();

            Match daysMatch = Regex.Match(lower, @"(?:in\s+)?(\d+)\s+day");
            if (daysMatch.Success && int.TryParse(daysMatch.Groups[1].Value, out int days))
            {
                return DateTime.Today.AddDays(days);
            }

            Match weeksMatch = Regex.Match(lower, @"(?:in\s+)?(\d+)\s+week");
            if (weeksMatch.Success && int.TryParse(weeksMatch.Groups[1].Value, out int weeks))
            {
                return DateTime.Today.AddDays(weeks * 7);
            }

            if (lower.Contains("tomorrow"))
            {
                return DateTime.Today.AddDays(1);
            }

            if (DateTime.TryParse(reminder, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }

            return null;
        }
    }
}
