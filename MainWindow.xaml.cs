using System;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        private readonly ChatBot _chatBot;
        private readonly TaskManager _taskManager;
        private readonly QuizManager _quizManager;
        private readonly ActivityLogger _activityLogger;
        private bool _quizAnswerSubmitted;
        private bool _showingAllActivities;

        public MainWindow()
        {
            InitializeComponent();

            _activityLogger = new ActivityLogger();
            _taskManager = new TaskManager();
            _quizManager = new QuizManager();
            _chatBot = new ChatBot(_taskManager, _activityLogger);
            _chatBot.TasksChanged += HandleChatTasksChanged;
            _chatBot.TasksViewRequested += HandleChatTasksViewRequested;
            _chatBot.QuizRequested += HandleChatQuizRequested;
            _chatBot.ActivityLogRequested += HandleChatActivityLogRequested;

            PlayVoiceGreeting();
            LoadAsciiArt();
            RefreshTaskGrid();
            _activityLogger.Log("Application launched and saved tasks loaded.");
            RefreshActivityLog();

            AppendBotMessage(_chatBot.GetGreeting());
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string input = UserInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                AppendBotMessage("Please type something first.");
                return;
            }

            AppendUserMessage(input);

            string response = _chatBot.ProcessInput(input);
            AppendBotMessage(response);

            UserInput.Clear();
            UserInput.Focus();
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            string title = TaskTitleInput.Text.Trim();
            string description = TaskDescriptionInput.Text.Trim();
            string reminder = TaskReminderInput.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                TaskStatusText.Text = "Please enter a task title.";
                TaskTitleInput.Focus();
                return;
            }

            CyberTask? task = _taskManager.AddTask(title, description, reminder);

            if (task == null)
            {
                TaskStatusText.Text = string.IsNullOrWhiteSpace(_taskManager.LastError)
                    ? "The task could not be added."
                    : _taskManager.LastError;
                return;
            }

            TaskTitleInput.Clear();
            TaskDescriptionInput.Clear();
            TaskReminderInput.Clear();
            TaskStatusText.Text = $"Task #{task.Id} added successfully.";
            _activityLogger.Log($"Task added: {task.Title}.");
            if (!string.IsNullOrWhiteSpace(task.Reminder))
            {
                _activityLogger.Log($"Reminder set for task '{task.Title}': {task.Reminder}.");
            }
            RefreshTaskGrid();
            RefreshActivityLog();
            AppendBotMessage($"Task added: '{task.Title}'.");
        }

        private void CompleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskGrid.SelectedItem is not CyberTask selectedTask)
            {
                TaskStatusText.Text = "Select a task to mark as complete.";
                return;
            }

            if (_taskManager.MarkAsComplete(selectedTask.Id))
            {
                TaskStatusText.Text = $"'{selectedTask.Title}' marked complete.";
                _activityLogger.Log($"Task completed: {selectedTask.Title}.");
                RefreshTaskGrid();
                RefreshActivityLog();
                AppendBotMessage($"Great work! The task '{selectedTask.Title}' is complete.");
            }
            else
            {
                TaskStatusText.Text = _taskManager.LastError;
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            if (TaskGrid.SelectedItem is not CyberTask selectedTask)
            {
                TaskStatusText.Text = "Select a task to delete.";
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show(
                $"Delete the task '{selectedTask.Title}'?",
                "Confirm deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
            {
                return;
            }

            if (_taskManager.DeleteTask(selectedTask.Id))
            {
                TaskStatusText.Text = $"'{selectedTask.Title}' deleted.";
                _activityLogger.Log($"Task deleted: {selectedTask.Title}.");
                RefreshTaskGrid();
                RefreshActivityLog();
                AppendBotMessage($"The task '{selectedTask.Title}' was deleted.");
            }
            else
            {
                TaskStatusText.Text = _taskManager.LastError;
            }
        }

        private void RefreshTasksButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshTaskGrid();
            TaskStatusText.Text = "Task list refreshed from tasks.json.";
        }

        private void RefreshTaskGrid()
        {
            TaskGrid.ItemsSource = null;
            TaskGrid.ItemsSource = _taskManager.GetAllTasks();

            if (!string.IsNullOrWhiteSpace(_taskManager.LastError))
            {
                TaskStatusText.Text = _taskManager.LastError;
            }
        }



        private void StartQuizButton_Click(object sender, RoutedEventArgs e)
        {
            StartQuiz();
        }

        private void StartQuiz()
        {
            _quizManager.ResetQuiz();
            _quizAnswerSubmitted = false;
            _activityLogger.Log("Cybersecurity quiz started.");
            RefreshActivityLog();
            StartQuizButton.Content = "Restart Quiz";
            LoadCurrentQuizQuestion();
        }

        private void SubmitAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            if (_quizAnswerSubmitted)
            {
                QuizFeedbackText.Text = "Click Next Question to continue.";
                return;
            }

            if (QuizOptionsList.SelectedItem is not string selectedAnswer)
            {
                QuizFeedbackText.Text = "Select an answer before submitting.";
                return;
            }

            QuizAnswerResult result = _quizManager.SubmitAnswer(selectedAnswer);
            _quizAnswerSubmitted = true;
            _activityLogger.Log($"Quiz question {_quizManager.CurrentQuestionNumber} answered {(result.IsCorrect ? "correctly" : "incorrectly")}.");
            RefreshActivityLog();
            QuizFeedbackText.Text = result.Feedback;
            QuizFeedbackText.Foreground = result.IsCorrect
                ? System.Windows.Media.Brushes.DarkGreen
                : System.Windows.Media.Brushes.DarkRed;

            QuizOptionsList.IsEnabled = false;
            SubmitAnswerButton.IsEnabled = false;
            NextQuestionButton.Content = _quizManager.CurrentQuestionNumber == _quizManager.QuestionCount
                ? "View Score"
                : "Next Question";
            NextQuestionButton.IsEnabled = true;
        }

        private void NextQuestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_quizAnswerSubmitted)
            {
                QuizFeedbackText.Text = "Submit an answer first.";
                return;
            }

            _quizManager.MoveNext();

            if (_quizManager.IsFinished)
            {
                ShowQuizResult();
                return;
            }

            _quizAnswerSubmitted = false;
            LoadCurrentQuizQuestion();
        }

        private void LoadCurrentQuizQuestion()
        {
            QuizQuestion? question = _quizManager.GetCurrentQuestion();

            if (question == null)
            {
                ShowQuizResult();
                return;
            }

            QuizProgressText.Text = $"Question {_quizManager.CurrentQuestionNumber} of {_quizManager.QuestionCount} | Score: {_quizManager.Score}";
            QuizQuestionText.Text = question.Question;
            QuizOptionsList.ItemsSource = question.Options;
            QuizOptionsList.SelectedItem = null;
            QuizOptionsList.IsEnabled = true;
            QuizFeedbackText.Text = string.Empty;
            QuizFeedbackText.Foreground = System.Windows.Media.Brushes.Black;
            SubmitAnswerButton.IsEnabled = true;
            NextQuestionButton.IsEnabled = false;
            NextQuestionButton.Content = "Next Question";
        }

        private void ShowQuizResult()
        {
            QuizProgressText.Text = $"Final score: {_quizManager.GetFinalScore()}";
            QuizQuestionText.Text = "Quiz complete!";
            QuizOptionsList.ItemsSource = null;
            QuizOptionsList.IsEnabled = false;
            QuizFeedbackText.Foreground = System.Windows.Media.Brushes.DarkBlue;
            QuizFeedbackText.Text = _quizManager.GetFinalMessage();
            SubmitAnswerButton.IsEnabled = false;
            NextQuestionButton.IsEnabled = false;
            StartQuizButton.Content = "Try Again";
            _activityLogger.Log($"Cybersecurity quiz completed with a score of {_quizManager.GetFinalScore()}.");
            RefreshActivityLog();
        }





        private void HandleChatTasksChanged()
        {
            RefreshTaskGrid();
            RefreshActivityLog();
        }

        private void HandleChatTasksViewRequested()
        {
            MainTabs.SelectedItem = TasksTab;
        }

        private void HandleChatQuizRequested()
        {
            MainTabs.SelectedItem = QuizTab;
            StartQuiz();
        }

        private void HandleChatActivityLogRequested()
        {
            _showingAllActivities = false;
            RefreshActivityLog();
            MainTabs.SelectedItem = ActivityTab;
        }

        private void ShowRecentActivityButton_Click(object sender, RoutedEventArgs e)
        {
            _showingAllActivities = false;
            RefreshActivityLog();
        }

        private void ShowMoreActivityButton_Click(object sender, RoutedEventArgs e)
        {
            _showingAllActivities = true;
            RefreshActivityLog();
        }

        private void RefreshActivityLog()
        {
            var entries = _showingAllActivities
                ? _activityLogger.GetAllEntries()
                : _activityLogger.GetRecentEntries(10);

            ActivityLogList.ItemsSource = null;
            ActivityLogList.ItemsSource = entries.Count == 0
                ? new[] { "No activity has been recorded yet." }
                : entries;

            ActivitySummaryText.Text = _showingAllActivities
                ? $"Showing all {_activityLogger.Count} recorded actions."
                : $"Showing the latest {Math.Min(10, _activityLogger.Count)} of {_activityLogger.Count} recorded actions.";

            ShowMoreActivityButton.IsEnabled = !_showingAllActivities && _activityLogger.Count > 10;
        }

        private void AppendUserMessage(string message)
        {
            ChatDisplay.Text += $"\nYou: {message}\n";
            ChatScrollViewer.ScrollToEnd();
        }

        private void AppendBotMessage(string message)
        {
            ChatDisplay.Text += $"\nBot: {message}\n";
            ChatScrollViewer.ScrollToEnd();
        }

        private void LoadAsciiArt()
        {
            AsciiArt.Text =
@"   ____      _               ____        _   
  / ___|   _| |__   ___ _ __| __ )  ___ | |_ 
 | |  | | | | '_ \ / _ \ '__|  _ \ / _ \| __|
 | |__| |_| | |_) |  __/ |  | |_) | (_) | |_ 
  \____\__, |_.__/ \___|_|  |____/ \___/ \__|
       |___/      Cybersecurity Bot";
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("greeting.wav");
                player.Play();
            }
            catch
            {
                AppendBotMessage("Voice greeting could not be played. Make sure greeting.wav is in the project folder.");
            }
        }
    }
}
