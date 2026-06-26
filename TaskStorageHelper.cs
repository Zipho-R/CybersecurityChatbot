using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CybersecurityChatbot
{
    public class TaskStorageHelper
    {
        private readonly string _filePath;

        public string LastError { get; private set; } = string.Empty;

        public TaskStorageHelper(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "tasks.json");
        }

        public List<CyberTask> LoadTasks()
        {
            LastError = string.Empty;

            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<CyberTask>();
                }

                string json = File.ReadAllText(_filePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<CyberTask>();
                }

                return JsonConvert.DeserializeObject<List<CyberTask>>(json)
                       ?? new List<CyberTask>();
            }
            catch (Exception ex)
            {
                LastError = $"Tasks could not be loaded: {ex.Message}";
                return new List<CyberTask>();
            }
        }

        public bool SaveTasks(IEnumerable<CyberTask> tasks)
        {
            LastError = string.Empty;

            try
            {
                string? directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonConvert.SerializeObject(tasks, Formatting.Indented);
                File.WriteAllText(_filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                LastError = $"Tasks could not be saved: {ex.Message}";
                return false;
            }
        }

        public CyberTask? AddTask(string title, string description, string reminder, DateTime? reminderDate = null)
        {
            List<CyberTask> tasks = LoadTasks();
            int nextId = tasks.Count == 0 ? 1 : tasks.Max(task => task.Id) + 1;

            CyberTask newTask = new CyberTask
            {
                Id = nextId,
                Title = title.Trim(),
                Description = description.Trim(),
                Reminder = reminder.Trim(),
                ReminderDate = reminderDate,
                IsComplete = false,
                CreatedAt = DateTime.Now
            };

            tasks.Add(newTask);
            return SaveTasks(tasks) ? newTask : null;
        }

        public bool MarkAsComplete(int id)
        {
            List<CyberTask> tasks = LoadTasks();
            CyberTask? task = tasks.FirstOrDefault(item => item.Id == id);

            if (task == null)
            {
                LastError = "The selected task could not be found.";
                return false;
            }

            task.IsComplete = true;
            return SaveTasks(tasks);
        }

        public bool UpdateReminder(int id, string reminder, DateTime? reminderDate)
        {
            List<CyberTask> tasks = LoadTasks();
            CyberTask? task = tasks.FirstOrDefault(item => item.Id == id);

            if (task == null)
            {
                LastError = "The selected task could not be found.";
                return false;
            }

            task.Reminder = reminder.Trim();
            task.ReminderDate = reminderDate;
            return SaveTasks(tasks);
        }

        public bool DeleteTask(int id)
        {
            List<CyberTask> tasks = LoadTasks();
            int removedCount = tasks.RemoveAll(item => item.Id == id);

            if (removedCount == 0)
            {
                LastError = "The selected task could not be found.";
                return false;
            }

            return SaveTasks(tasks);
        }
    }
}
