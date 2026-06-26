using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CybersecurityChatbot
{
    public class ChatBot
    {
        private readonly KeywordResponder _keywords;
        private readonly SentimentDetector _sentiment;
        private readonly MemoryStore _memory;
        private readonly TaskManager? _taskManager;
        private readonly ActivityLogger? _activityLogger;

        private bool _awaitingName = true;
        private string? _lastTopic;
        private int? _pendingReminderTaskId;
        private string? _pendingReminderTaskTitle;
        private readonly Random _random = new Random();

        public event Action? TasksChanged;
        public event Action? TasksViewRequested;
        public event Action? QuizRequested;
        public event Action? ActivityLogRequested;

        private readonly List<string> _fallbacks = new List<string>
        {
            "I didn't quite understand that. Try asking about a cybersecurity topic, adding a task, starting the quiz, or showing the activity log.",
            "Could you rephrase that? For example: 'add a task to enable 2FA', 'start quiz', or 'show activity log'.",
            "I'm not sure about that yet, but I can help with cybersecurity tips, tasks, reminders, the quiz, and your activity log."
        };

        public ChatBot(TaskManager? taskManager = null, ActivityLogger? activityLogger = null)
        {
            _keywords = new KeywordResponder();
            _sentiment = new SentimentDetector();
            _memory = new MemoryStore();
            _taskManager = taskManager;
            _activityLogger = activityLogger;
        }

        public string GetGreeting()
        {
            _awaitingName = true;
            return "Hello! Welcome to the Cybersecurity Awareness Bot. What is your name?";
        }

        public string ProcessInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "Please type something so I can help you.";
            }

            input = input.Trim();
            string lowerInput = input.ToLowerInvariant();

            if (_awaitingName)
            {
                _awaitingName = false;
                _memory.Store("name", input);
                _activityLogger?.Log($"User introduced themselves as {input}.");

                return $"Nice to meet you, {input}! You can ask for cybersecurity advice, add tasks and reminders, start the quiz, or view the activity log.";
            }

            if (_pendingReminderTaskId.HasValue)
            {
                return HandlePendingReminder(input, lowerInput);
            }

            if (IsActivityLogIntent(lowerInput))
            {
                _activityLogger?.Log("Activity log requested through the chatbot.");
                ActivityLogRequested?.Invoke();
                return "I opened the Activity Log tab. It shows the latest 10 actions, and you can choose Show More for the full history.";
            }

            if (IsQuizIntent(lowerInput))
            {
                QuizRequested?.Invoke();
                return "I opened the Cyber Quiz and started a new round. Answer one question at a time and read the feedback after each answer.";
            }

            if (IsViewTasksIntent(lowerInput))
            {
                TasksChanged?.Invoke();
                TasksViewRequested?.Invoke();
                int taskCount = _taskManager?.GetAllTasks().Count ?? 0;
                return taskCount == 0
                    ? "Your task list is empty. Try saying 'add a task to enable 2FA'."
                    : $"I opened the Task Assistant. You currently have {taskCount} saved task{(taskCount == 1 ? string.Empty : "s")}.";
            }

            if (IsTaskCreationIntent(lowerInput))
            {
                return HandleTaskCreation(input);
            }

            if (IsFollowUp(lowerInput))
            {
                if (!string.IsNullOrWhiteSpace(_lastTopic))
                {
                    string? followUpTip = _keywords.GetRandomResponseForKey(_lastTopic);
                    string opener = _memory.GetPersonalisedOpener();

                    if (!string.IsNullOrWhiteSpace(opener))
                    {
                        return $"{opener} {followUpTip}";
                    }

                    return $"Here is more about {_lastTopic}: {followUpTip}";
                }

                return "I can explain more, but first ask about a topic such as phishing, passwords, malware, privacy, scams, 2FA, or safe browsing.";
            }

            string? favouriteTopic = ExtractFavouriteTopic(lowerInput);

            if (!string.IsNullOrWhiteSpace(favouriteTopic))
            {
                _memory.Store("favouriteTopic", favouriteTopic);
                _activityLogger?.Log($"Favourite cybersecurity topic remembered: {favouriteTopic}.");
                return $"Got it! I'll remember that you're interested in {favouriteTopic}.";
            }

            Sentiment detectedSentiment = _sentiment.Detect(input);
            string sentimentOpener = _sentiment.GetSentimentResponse(detectedSentiment);
            string? keywordResponse = _keywords.GetResponse(input);

            if (!string.IsNullOrWhiteSpace(keywordResponse))
            {
                _lastTopic = FindMatchedKeyword(input);
                string personalisedOpener = _memory.GetPersonalisedOpener();

                if (!string.IsNullOrWhiteSpace(sentimentOpener))
                {
                    return !string.IsNullOrWhiteSpace(personalisedOpener)
                        ? $"{sentimentOpener} {personalisedOpener} {keywordResponse}"
                        : $"{sentimentOpener} {keywordResponse}";
                }

                return !string.IsNullOrWhiteSpace(personalisedOpener)
                    ? $"{personalisedOpener} {keywordResponse}"
                    : keywordResponse;
            }

            if (lowerInput.Contains("how are you"))
            {
                string? name = _memory.Recall("name");
                return !string.IsNullOrWhiteSpace(name)
                    ? $"I'm doing well, {name}! I'm ready to help you stay safe online."
                    : "I'm doing well and ready to help you stay safe online.";
            }

            if (lowerInput.Contains("what can you do") ||
                lowerInput.Contains("what can i ask") ||
                lowerInput == "help" ||
                lowerInput.Contains("show help"))
            {
                string topics = string.Join(", ", _keywords.GetAllKeywords());
                return $"I can explain these topics: {topics}. I can also add cybersecurity tasks, set reminders, start a quiz, and show your activity log.";
            }

            if (lowerInput.Contains("purpose"))
            {
                return "My purpose is to teach cybersecurity awareness and help you practise safe habits through tips, tasks, reminders, and a quiz.";
            }

            if (!string.IsNullOrWhiteSpace(sentimentOpener))
            {
                string randomTopic = GetRandomKeyword();
                string? randomTip = _keywords.GetRandomResponseForKey(randomTopic);
                _lastTopic = randomTopic;
                return $"{sentimentOpener} {randomTip}";
            }

            return _fallbacks[_random.Next(_fallbacks.Count)];
        }

        private string HandleTaskCreation(string input)
        {
            if (_taskManager == null)
            {
                return "The task assistant is not available right now.";
            }

            string title = ExtractTaskTitle(input);
            if (string.IsNullOrWhiteSpace(title))
            {
                return "What cybersecurity task should I add? For example: 'add a task to review my privacy settings'.";
            }

            string inlineReminder = ExtractInlineReminder(input);
            CyberTask? task = _taskManager.AddTask(title, string.Empty, inlineReminder);

            if (task == null)
            {
                return string.IsNullOrWhiteSpace(_taskManager.LastError)
                    ? "I could not add that task. Please try again."
                    : _taskManager.LastError;
            }

            _activityLogger?.Log($"Task added through chat: {task.Title}.");
            TasksChanged?.Invoke();

            if (!string.IsNullOrWhiteSpace(inlineReminder))
            {
                _activityLogger?.Log($"Reminder set for task '{task.Title}': {inlineReminder}.");
                return $"Task added: '{task.Title}' with the description '{task.Description}'. I also set the reminder '{inlineReminder}'.";
            }

            _pendingReminderTaskId = task.Id;
            _pendingReminderTaskTitle = task.Title;
            return $"Task added with the description '{task.Description}' Would you like a reminder?";
        }

        private string HandlePendingReminder(string input, string lowerInput)
        {
            if (_taskManager == null || !_pendingReminderTaskId.HasValue)
            {
                ClearPendingReminder();
                return "The reminder could not be connected to a task. Please try again.";
            }

            if (lowerInput == "no" || lowerInput.Contains("no reminder") || lowerInput.Contains("not now"))
            {
                string title = _pendingReminderTaskTitle ?? "the task";
                ClearPendingReminder();
                return $"No problem. '{title}' was saved without a reminder.";
            }

            string reminder = ExtractReminderReply(input);

            if (string.IsNullOrWhiteSpace(reminder) ||
                lowerInput == "yes" ||
                lowerInput == "sure" ||
                lowerInput == "okay" ||
                lowerInput == "ok")
            {
                return "Please tell me when to remind you, for example: 'remind me in 5 days' or 'tomorrow'.";
            }

            int taskId = _pendingReminderTaskId.Value;
            string titleForMessage = _pendingReminderTaskTitle ?? "the task";

            if (!_taskManager.UpdateReminder(taskId, reminder))
            {
                return string.IsNullOrWhiteSpace(_taskManager.LastError)
                    ? "I could not save that reminder. Please try again."
                    : _taskManager.LastError;
            }

            _activityLogger?.Log($"Reminder set for task '{titleForMessage}': {reminder}.");
            ClearPendingReminder();
            TasksChanged?.Invoke();
            return $"Got it! I'll remind you {reminder}.";
        }

        private void ClearPendingReminder()
        {
            _pendingReminderTaskId = null;
            _pendingReminderTaskTitle = null;
        }

        private bool IsTaskCreationIntent(string lowerInput)
        {
            return ContainsAny(lowerInput,
                "add task",
                "add a task",
                "create task",
                "create a task",
                "new task",
                "set a task",
                "remind me to",
                "add reminder",
                "set reminder");
        }

        private bool IsViewTasksIntent(string lowerInput)
        {
            return ContainsAny(lowerInput,
                "show tasks",
                "show my tasks",
                "view tasks",
                "view my tasks",
                "task list",
                "what tasks");
        }

        private bool IsQuizIntent(string lowerInput)
        {
            return ContainsAny(lowerInput,
                "start quiz",
                "start the quiz",
                "take quiz",
                "take the quiz",
                "play quiz",
                "cyber quiz",
                "test my knowledge",
                "quiz me");
        }

        private bool IsActivityLogIntent(string lowerInput)
        {
            return ContainsAny(lowerInput,
                "show activity log",
                "view activity log",
                "open activity log",
                "show my activity",
                "what have you done for me",
                "recent actions",
                "activity history");
        }

        private bool ContainsAny(string input, params string[] phrases)
        {
            return phrases.Any(input.Contains);
        }

        private string ExtractTaskTitle(string input)
        {
            string title = input.Trim();

            int dashIndex = title.IndexOf('-');
            if (dashIndex >= 0 && dashIndex < title.Length - 1)
            {
                title = title[(dashIndex + 1)..];
            }
            else
            {
                title = Regex.Replace(
                    title,
                    @"^.*?\b(?:add|create|set|make)\s+(?:a\s+)?(?:new\s+)?(?:cybersecurity\s+)?task\s*(?:to|for|about)?\s*",
                    string.Empty,
                    RegexOptions.IgnoreCase);

                title = Regex.Replace(
                    title,
                    @"^.*?\bremind\s+me\s+to\s+",
                    string.Empty,
                    RegexOptions.IgnoreCase);

                title = Regex.Replace(
                    title,
                    @"^.*?\b(?:add|set)\s+(?:a\s+)?reminder\s+(?:to|for)?\s*",
                    string.Empty,
                    RegexOptions.IgnoreCase);
            }

            title = Regex.Replace(title, @"\s+(?:in\s+\d+\s+(?:day|days|week|weeks)|tomorrow)\s*[.!?]*$", string.Empty, RegexOptions.IgnoreCase);
            return title.Trim(' ', '.', ',', '!', '?', ':', ';');
        }

        private string ExtractInlineReminder(string input)
        {
            Match relativeMatch = Regex.Match(
                input,
                @"\b(?:in\s+\d+\s+(?:day|days|week|weeks)|tomorrow)\b",
                RegexOptions.IgnoreCase);

            return relativeMatch.Success ? relativeMatch.Value.ToLowerInvariant() : string.Empty;
        }

        private string ExtractReminderReply(string input)
        {
            string reminder = input.Trim();
            reminder = Regex.Replace(reminder, @"^(?:yes[, ]*)?(?:please[, ]*)?", string.Empty, RegexOptions.IgnoreCase);
            reminder = Regex.Replace(reminder, @"^(?:remind\s+me\s*)", string.Empty, RegexOptions.IgnoreCase);
            return reminder.Trim(' ', '.', ',', '!', '?');
        }

        private bool IsFollowUp(string lowerInput)
        {
            return ContainsAny(lowerInput,
                "tell me more",
                "explain more",
                "more info",
                "continue",
                "go on",
                "another tip");
        }

        private string? ExtractFavouriteTopic(string lowerInput)
        {
            foreach (string keyword in _keywords.GetAllKeywords())
            {
                if (ContainsAny(lowerInput, "interested in", "i like", "favourite topic", "favorite topic") &&
                    lowerInput.Contains(keyword.ToLowerInvariant()))
                {
                    return keyword;
                }
            }

            return null;
        }

        private string? FindMatchedKeyword(string input)
        {
            foreach (string keyword in _keywords.GetAllKeywords())
            {
                if (input.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return keyword;
                }
            }

            return null;
        }

        private string GetRandomKeyword()
        {
            List<string> keywords = _keywords.GetAllKeywords();
            return keywords[_random.Next(keywords.Count)];
        }
    }
}
