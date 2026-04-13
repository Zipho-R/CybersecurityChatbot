using System;

namespace CybersecurityChatbot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(@"
  ____       _               ____        _   
 / ___| ___ | |__   ___ _ __| __ )  ___ | |_ 
| |    / _ \| '_ \ / _ \ '__|  _ \ / _ \| __|
| |___| (_) | |_) |  __/ |  | |_) | (_) | |_ 
 \____|\___/|_.__/ \___|_|  |____/ \___/ \__|

   CYBERSECURITY AWARENESS BOT
");

            Console.ResetColor();

            User user = new User();
            Chatbot bot = new Chatbot();

            AudioPlayer audio = new AudioPlayer();
            audio.PlayGreeting();

            user.GetUserName();
            bot.StartChat(user.Name);
        }
    }
}