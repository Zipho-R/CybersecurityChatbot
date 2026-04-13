using System;

namespace CybersecurityChatbot
{
    internal class Chatbot
    {
        public void StartChat(string userName)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Bot: You can ask me about passwords, phishing, malware, VPN, or safe browsing.");
            Console.WriteLine("Bot: Type 'exit' to quit.\n");
            Console.ResetColor();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{userName}: ");
                Console.ResetColor();

                string input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Bot: Please enter something.");
                    continue;
                }

                input = input.ToLower().Trim();

                Console.ForegroundColor = ConsoleColor.Green;

                if (input == "exit")
                {
                    Console.WriteLine("Bot: Goodbye! Stay safe online.");
                    break;
                }
                else if (input.Contains("how are you"))
                {
                    Console.WriteLine("Bot: I'm running perfectly and ready to help!");
                }
                else if (input.Contains("purpose"))
                {
                    Console.WriteLine("Bot: I help users learn cybersecurity awareness.");
                }
                else if (input.Contains("password"))
                {
                    Console.WriteLine("Bot: Use strong passwords with letters, numbers, and symbols.");
                }
                else if (input.Contains("phishing"))
                {
                    Console.WriteLine("Bot: Avoid clicking suspicious links or emails.");
                }
                else if (input.Contains("malware"))
                {
                    Console.WriteLine("Bot: Malware is harmful software. Use antivirus protection.");
                }
                else if (input.Contains("vpn"))
                {
                    Console.WriteLine("Bot: A VPN protects your internet privacy.");
                }
                else if (input.Contains("browsing"))
                {
                    Console.WriteLine("Bot: Only visit trusted websites.");
                }
                else
                {
                    Console.WriteLine("Bot: I didn’t understand. Ask about cybersecurity topics.");
                }

                Console.ResetColor();
            }
        }
    }
}
        