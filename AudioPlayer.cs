using System;
using System.IO;
using System.Media;

namespace CybersecurityChatbot
{
    internal class AudioPlayer
    {
        public void PlayGreeting()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav");

                SoundPlayer player = new SoundPlayer(path);
                player.PlaySync();
            }
            catch
            {
                Console.WriteLine("Audio file not found or failed to play.");
            }
        }
    }
}