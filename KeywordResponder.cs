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
                ["password"] = new List<string>
                {
                    "Use a long password or passphrase with uppercase letters, lowercase letters, numbers, and symbols.",
                    "Avoid reusing the same password on different accounts. If one account is hacked, the others become risky too.",
                    "Use a password manager to safely store strong passwords instead of trying to remember them all."
                },
                ["phishing"] = new List<string>
                {
                    "Be careful of emails or messages asking you to click urgent links. Always check the sender address first.",
                    "Phishing messages often create panic, such as saying your account will be closed. Pause and verify before clicking.",
                    "Never enter your password through a link sent in an unexpected email or SMS."
                },
                ["privacy"] = new List<string>
                {
                    "Check your privacy settings on social media and limit who can see your posts and personal details.",
                    "Avoid sharing sensitive information like your address, school, phone number, or passwords online.",
                    "Only give apps the permissions they really need, such as camera, location, or contacts."
                },
                ["scam"] = new List<string>
                {
                    "If an offer sounds too good to be true, it is probably a scam.",
                    "Do not send money or personal information to someone you only met online.",
                    "Scammers often pressure you to act quickly. Slow down and verify the message first."
                },
                ["malware"] = new List<string>
                {
                    "Malware is harmful software. Avoid downloading files from websites or people you do not trust.",
                    "Keep your antivirus, operating system, and apps updated to protect against malware.",
                    "Do not open suspicious attachments, especially from unknown emails."
                },
                ["ransomware"] = new List<string>
                {
                    "Ransomware can encrypt files and demand payment. Keep tested backups that are separate from your device.",
                    "Avoid unexpected attachments and keep software updated to reduce ransomware risk.",
                    "If ransomware is suspected, disconnect the device from the network and report it to a trusted IT or security professional."
                },
                ["vpn"] = new List<string>
                {
                    "A VPN can help protect your connection on public Wi-Fi, but it does not replace safe browsing habits.",
                    "Use a trusted VPN provider and avoid random free VPNs that may collect your data.",
                    "A VPN hides some browsing activity from the network, but you still need strong passwords and safe websites."
                },
                ["browsing"] = new List<string>
                {
                    "Use websites that start with HTTPS, especially when logging in or entering personal information.",
                    "Avoid clicking pop-ups that claim your device is infected. They are often fake warnings.",
                    "Keep your browser updated and remove extensions you do not use or trust."
                },
                ["2fa"] = new List<string>
                {
                    "Enable two-factor authentication on important accounts so a stolen password is not enough to sign in.",
                    "Authenticator apps are generally safer than SMS codes when an account supports them.",
                    "Never approve an unexpected login prompt. It may be an attacker trying to access your account."
                },
                ["two-factor"] = new List<string>
                {
                    "Two-factor authentication adds a second verification step, such as an authenticator code.",
                    "Use two-factor authentication for email, banking, social media, and cloud storage accounts.",
                    "Store recovery codes somewhere secure in case you lose access to your authentication device."
                },
                ["social engineering"] = new List<string>
                {
                    "Social engineers manipulate trust, fear, urgency, or curiosity to make people reveal information.",
                    "Verify unusual requests through a separate trusted contact method before acting.",
                    "Do not let urgency stop you from checking who is really making a request."
                },
                ["backup"] = new List<string>
                {
                    "Keep backups of important files and test that you can restore them.",
                    "Use the 3-2-1 approach: three copies, on two types of storage, with one copy stored separately.",
                    "A backup that remains permanently connected may also be damaged by ransomware, so keep a separate copy."
                },
                ["public wi-fi"] = new List<string>
                {
                    "Avoid sensitive transactions on public Wi-Fi unless you have a trusted secure connection.",
                    "Turn off automatic Wi-Fi connection and file sharing when using a public network.",
                    "Confirm the correct network name with staff because attackers can create convincing fake hotspots."
                }
            };
        }

        public string? GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            foreach (string keyword in _responses.Keys.OrderByDescending(key => key.Length))
            {
                if (input.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return GetRandomResponseForKey(keyword);
                }
            }

            return null;
        }

        public string? GetRandomResponseForKey(string keyword)
        {
            if (!_responses.TryGetValue(keyword, out List<string>? possibleResponses))
            {
                return null;
            }

            return possibleResponses[_random.Next(possibleResponses.Count)];
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
