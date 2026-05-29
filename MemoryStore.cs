using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    public class MemoryStore
    {
        private readonly Dictionary<string, string> _store =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string UserName { get; set; }
        public string FavouriteTopic { get; set; }

        public void Store(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            value = value.Trim();
            _store[key] = value;

            if (key.Equals("name", StringComparison.OrdinalIgnoreCase))
            {
                UserName = value;
            }

            if (key.Equals("favourite", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("favouriteTopic", StringComparison.OrdinalIgnoreCase))
            {
                FavouriteTopic = value;
            }
        }

        public string Recall(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            _store.TryGetValue(key, out string value);
            return value;
        }

        public bool HasMemory()
        {
            return !string.IsNullOrWhiteSpace(UserName) ||
                   !string.IsNullOrWhiteSpace(FavouriteTopic);
        }

        public string GetPersonalisedOpener()
        {
            if (!string.IsNullOrWhiteSpace(UserName) &&
                !string.IsNullOrWhiteSpace(FavouriteTopic))
            {
                return $"{UserName}, as someone interested in {FavouriteTopic}, here's a cybersecurity tip:";
            }

            if (!string.IsNullOrWhiteSpace(FavouriteTopic))
            {
                return $"As someone interested in {FavouriteTopic}, here's a cybersecurity tip:";
            }

            if (!string.IsNullOrWhiteSpace(UserName))
            {
                return $"{UserName}, here's a cybersecurity tip:";
            }

            return string.Empty;
        }
    }
}