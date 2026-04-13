using System;

namespace CybersecurityChatbot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            User user = new User();
            Chatbot bot = new Chatbot();

            user.GetUserName();
            bot.StartChat(user.Name);
        }
    }
}