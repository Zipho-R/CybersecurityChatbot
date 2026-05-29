using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    public enum Sentiment
    {
        Neutral,
        Worried,
        Curious,
        Frustrated,
        Happy
    }

    public class SentimentDetector
    {
        private readonly Dictionary<Sentiment, List<string>> _triggers;

        public SentimentDetector()
        {
            _triggers = new Dictionary<Sentiment, List<string>>
            {
                {
                    Sentiment.Worried,
                    new List<string>
                    {
                        "worried",
                        "scared",
                        "afraid",
                        "anxious",
                        "nervous",
                        "unsafe",
                        "panic",
                        "stressed"
                    }
                },
                {
                    Sentiment.Curious,
                    new List<string>
                    {
                        "curious",
                        "wondering",
                        "interested",
                        "want to know",
                        "how does",
                        "how do",
                        "explain",
                        "teach me"
                    }
                },
                {
                    Sentiment.Frustrated,
                    new List<string>
                    {
                        "frustrated",
                        "annoyed",
                        "confused",
                        "don't understand",
                        "dont understand",
                        "do not understand",
                        "dont get",
                        "hard",
                        "difficult"
                    }
                },
                {
                    Sentiment.Happy,
                    new List<string>
                    {
                        "great",
                        "thanks",
                        "thank you",
                        "helpful",
                        "awesome",
                        "love it",
                        "nice",
                        "good"
                    }
                }
            };
        }

        public Sentiment Detect(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return Sentiment.Neutral;
            }

            string lowerInput = input.ToLowerInvariant();

            foreach (var sentimentGroup in _triggers)
            {
                foreach (string triggerWord in sentimentGroup.Value)
                {
                    if (lowerInput.Contains(triggerWord))
                    {
                        return sentimentGroup.Key;
                    }
                }
            }

            return Sentiment.Neutral;
        }

        public string GetSentimentResponse(Sentiment sentiment)
        {
            switch (sentiment)
            {
                case Sentiment.Worried:
                    return "I can understand why you're worried. Online threats can be stressful, but there are ways to stay safe.";

                case Sentiment.Curious:
                    return "That's a great question. Learning about cybersecurity is one of the best ways to stay protected.";

                case Sentiment.Frustrated:
                    return "I understand this can be frustrating. Cybersecurity topics can feel complicated at first.";

                case Sentiment.Happy:
                    return "I'm glad you're finding this useful. I have more tips.";

                default:
                    return string.Empty;
            }
        }
    }
}