using System;

namespace CybersecurityChatbot
{
    internal class Chatbot
    {
        public void StartChat(string userName)
        {
            Console.WriteLine("You can ask me about passwords, phishing, or safe browsing.");
            Console.WriteLine("Type 'exit' to quit.\n");

            while (true)
            {
                Console.Write($"{userName}: ");
                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Bot: Please enter something.");
                    continue;
                }

                input = input.ToLower().Trim();

                if (input == "exit")
                {
                    Console.WriteLine("Bot: Goodbye! Stay safe online.");
                    break;
                }
                else if (input.Contains("how are you"))
                {
                    Console.WriteLine("Bot: I'm just code, but I'm here to help you stay safe online.");
                }
                else if (input.Contains("purpose"))
                {
                    Console.WriteLine("Bot: My purpose is to teach users about cybersecurity awareness.");
                }
                else if (input.Contains("password"))
                {
                    Console.WriteLine("Bot: Use strong passwords with a mix of letters, numbers, and symbols.");
                }
                else if (input.Contains("phishing"))
                {
                    Console.WriteLine("Bot: Be careful of suspicious emails and links.");
                }
                else if (input.Contains("safe browsing") || input.Contains("browsing"))
                {
                    Console.WriteLine("Bot: Only visit trusted websites and avoid downloading files from unknown sources.");
                }
                else
                {
                    Console.WriteLine("Bot: I didn't quite understand that. Try asking about passwords, phishing, or safe browsing.");
                }
            }
        }
    }
}