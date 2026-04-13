using System;
namespace CybersecurityChatbot
{
    internal class User
    {
        public string Name { get; private set; }

        public void GetUserName()
        {
            Console.Write("Enter your name: ");
            string input = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(input))
            {
                Console.Write("Name cannot be empty. Please enter your name: ");
                input = Console.ReadLine();
            }

            Name = input.Trim();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nHello, {Name}! Welcome to the Cybersecurity Awareness Bot.\n");
            Console.ResetColor();
        }
    }
}