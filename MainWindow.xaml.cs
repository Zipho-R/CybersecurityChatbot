using System;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace CybersecurityChatbot
{
    public partial class MainWindow : Window
    {
        private ChatBot _chatBot;

        public MainWindow()
        {
            InitializeComponent();

            _chatBot = new ChatBot();

            PlayVoiceGreeting();
            LoadAsciiArt();

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