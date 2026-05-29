using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    public class ChatBot
    {
        private readonly KeywordResponder _keywords;
        private readonly SentimentDetector _sentiment;
        private readonly MemoryStore _memory;

        private bool _awaitingName = true;
        private string _lastTopic;
        private readonly Random _random = new Random();

        private readonly List<string> _fallbacks = new List<string>
        {
            "I didn't quite understand that. You can ask me about passwords, phishing, privacy, scams, malware, VPNs, or safe browsing.",
            "I'm mainly here to help with cybersecurity awareness. Try asking about password safety, phishing, or online scams.",
            "I'm not sure about that yet, but I can help with topics like malware, privacy, scams, and safe browsing."
        };

        public ChatBot()
        {
            _keywords = new KeywordResponder();
            _sentiment = new SentimentDetector();
            _memory = new MemoryStore();
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

            // 1. Capture user's name first
            if (_awaitingName)
            {
                _awaitingName = false;
                _memory.Store("name", input);

                return $"Nice to meet you, {input}! You can ask me about passwords, phishing, privacy, scams, malware, VPNs, or safe browsing.";
            }

            // 2. Handle follow-up phrases before anything else
            if (IsFollowUp(lowerInput))
            {
                if (!string.IsNullOrWhiteSpace(_lastTopic))
                {
                    string followUpTip = _keywords.GetRandomResponseForKey(_lastTopic);
                    string opener = _memory.GetPersonalisedOpener();

                    if (!string.IsNullOrWhiteSpace(opener))
                    {
                        return $"{opener} {followUpTip}";
                    }

                    return $"Here is more about {_lastTopic}: {followUpTip}";
                }

                return "I can explain more, but first ask me about a topic like phishing, passwords, malware, privacy, scams, VPNs, or safe browsing.";
            }

            // Store favourite topic if the user tells the bot
            string favouriteTopic = ExtractFavouriteTopic(lowerInput);

            if (!string.IsNullOrWhiteSpace(favouriteTopic))
            {
                _memory.Store("favouriteTopic", favouriteTopic);
                return $"Got it! I'll remember that you're interested in {favouriteTopic}.";
            }

            // 3. Detect sentiment
            Sentiment detectedSentiment = _sentiment.Detect(input);
            string sentimentOpener = _sentiment.GetSentimentResponse(detectedSentiment);

            // 4. Detect keyword and return matching response
            string keywordResponse = _keywords.GetResponse(input);

            if (!string.IsNullOrWhiteSpace(keywordResponse))
            {
                _lastTopic = FindMatchedKeyword(input);

                string personalisedOpener = _memory.GetPersonalisedOpener();

                if (!string.IsNullOrWhiteSpace(sentimentOpener))
                {
                    if (!string.IsNullOrWhiteSpace(personalisedOpener))
                    {
                        return $"{sentimentOpener} {personalisedOpener} {keywordResponse}";
                    }

                    return $"{sentimentOpener} {keywordResponse}";
                }

                if (!string.IsNullOrWhiteSpace(personalisedOpener))
                {
                    return $"{personalisedOpener} {keywordResponse}";
                }

                return keywordResponse;
            }

            // 5. Handle special phrases
            if (lowerInput.Contains("how are you"))
            {
                string name = _memory.Recall("name");

                if (!string.IsNullOrWhiteSpace(name))
                {
                    return $"I'm doing well, {name}! I'm ready to help you stay safe online.";
                }

                return "I'm doing well and ready to help you stay safe online.";
            }

            if (lowerInput.Contains("what can you do") ||
                lowerInput.Contains("what can i ask") ||
                lowerInput.Contains("help"))
            {
                string topics = string.Join(", ", _keywords.GetAllKeywords());
                return $"I can help you with these cybersecurity topics: {topics}. You can also tell me if you're worried, curious, or frustrated.";
            }

            if (lowerInput.Contains("purpose"))
            {
                return "My purpose is to teach cybersecurity awareness by giving helpful tips about online safety.";
            }

            // If sentiment exists but no keyword was found, still give a tip automatically
            if (!string.IsNullOrWhiteSpace(sentimentOpener))
            {
                string randomTopic = GetRandomKeyword();
                string randomTip = _keywords.GetRandomResponseForKey(randomTopic);

                return $"{sentimentOpener} {randomTip}";
            }

            // 6. Fallback
            return _fallbacks[_random.Next(_fallbacks.Count)];
        }

        private bool IsFollowUp(string lowerInput)
        {
            return lowerInput.Contains("tell me more") ||
                   lowerInput.Contains("explain more") ||
                   lowerInput.Contains("more info") ||
                   lowerInput.Contains("continue") ||
                   lowerInput.Contains("go on");
        }

        private string ExtractFavouriteTopic(string lowerInput)
        {
            foreach (string keyword in _keywords.GetAllKeywords())
            {
                if ((lowerInput.Contains("interested in") ||
                     lowerInput.Contains("i like") ||
                     lowerInput.Contains("favourite topic") ||
                     lowerInput.Contains("favorite topic")) &&
                    lowerInput.Contains(keyword.ToLowerInvariant()))
                {
                    return keyword;
                }
            }

            return null;
        }

        private string FindMatchedKeyword(string input)
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