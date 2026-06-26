using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    public class ActivityLogger
    {
        private readonly List<string> _entries = new List<string>();

        public int Count => _entries.Count;

        public void Log(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return;
            }

            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {description.Trim()}";
            _entries.Add(entry);
        }

        public List<string> GetRecentEntries(int count = 10)
        {
            return _entries
                .TakeLast(Math.Max(1, count))
                .Reverse()
                .ToList();
        }

        public List<string> GetAllEntries()
        {
            return _entries
                .AsEnumerable()
                .Reverse()
                .ToList();
        }
    }
}
