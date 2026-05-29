using System;
using System.Collections.Generic;
using System.Linq;

namespace CybersecurityChatbot
{
    public class KeywordResponder
    {
        private readonly Dictionary<string, List<string>> _responses;
        private readonly Random _random = new Random();

        public KeywordResponder()
        {
            _responses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "password",
                    new List<string>
                    {
                        "Use a long password or passphrase with uppercase letters, lowercase letters, numbers, and symbols.",
                        "Avoid reusing the same password on different accounts. If one account is hacked, the others become risky too.",
                        "Use a password manager to safely store strong passwords instead of trying to remember them all."
                    }
                },
                {
                    "phishing",
                    new List<string>
                    {
                        "Be careful of emails or messages asking you to click urgent links. Always check the sender address first.",
                        "Phishing messages often create panic, such as saying your account will be closed. Pause and verify before clicking.",
                        "Never enter your password through a link sent in an unexpected email or SMS."
                    }
                },
                {
                    "privacy",
                    new List<string>
                    {
                        "Check your privacy settings on social media and limit who can see your posts and personal details.",
                        "Avoid sharing sensitive information like your address, school, phone number, or passwords online.",
                        "Only give apps the permissions they really need, such as camera, location, or contacts."
                    }
                },
                {
                    "scam",
                    new List<string>
                    {
                        "If an offer sounds too good to be true, it is probably a scam.",
                        "Do not send money or personal information to someone you only met online.",
                        "Scammers often pressure you to act quickly. Slow down and verify the message first."
                    }
                },
                {
                    "malware",
                    new List<string>
                    {
                        "Malware is harmful software. Avoid downloading files from websites or people you do not trust.",
                        "Keep your antivirus, operating system, and apps updated to protect against malware.",
                        "Do not open suspicious attachments, especially from unknown emails."
                    }
                },
                {
                    "vpn",
                    new List<string>
                    {
                        "A VPN can help protect your connection on public Wi-Fi, but it does not replace safe browsing habits.",
                        "Use a trusted VPN provider and avoid random free VPNs that may collect your data.",
                        "A VPN hides some browsing activity from the network, but you still need strong passwords and safe websites."
                    }
                },
                {
                    "browsing",
                    new List<string>
                    {
                        "Use websites that start with HTTPS, especially when logging in or entering personal information.",
                        "Avoid clicking pop-ups that claim your device is infected. They are often fake warnings.",
                        "Keep your browser updated and remove extensions you do not use or trust."
                    }
                }
            };
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            foreach (var keyword in _responses.Keys)
            {
                if (input.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return GetRandomResponseForKey(keyword);
                }
            }

            return null;
        }

        public string GetRandomResponseForKey(string keyword)
        {
            if (!_responses.ContainsKey(keyword))
            {
                return null;
            }

            List<string> possibleResponses = _responses[keyword];
            int index = _random.Next(possibleResponses.Count);

            return possibleResponses[index];
        }

        public List<string> GetAllKeywords()
        {
            return _responses.Keys.ToList();
        }

        public bool IsKnownKeyword(string keyword)
        {
            return _responses.ContainsKey(keyword);
        }
    }
}