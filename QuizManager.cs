using System;
using System.Collections.Generic;

namespace CybersecurityChatbot
{
    public class QuizManager
    {
        private readonly List<QuizQuestion> _questions;
        private int _currentIndex;
        private int _score;

        public int CurrentQuestionNumber => _currentIndex + 1;
        public int QuestionCount => _questions.Count;
        public int Score => _score;
        public bool IsFinished => _currentIndex >= _questions.Count;

        public QuizManager()
        {
            _questions = CreateQuestions();
            ResetQuiz();
        }

        public QuizQuestion? GetCurrentQuestion()
        {
            return IsFinished ? null : _questions[_currentIndex];
        }

        public QuizAnswerResult SubmitAnswer(string answer)
        {
            QuizQuestion? question = GetCurrentQuestion();

            if (question == null)
            {
                return new QuizAnswerResult
                {
                    IsCorrect = false,
                    Feedback = "The quiz has already finished."
                };
            }

            bool correct = string.Equals(
                answer?.Trim(),
                question.CorrectAnswer.Trim(),
                StringComparison.OrdinalIgnoreCase);

            if (correct)
            {
                _score++;
            }

            return new QuizAnswerResult
            {
                IsCorrect = correct,
                Feedback = correct
                    ? $"Correct! {question.Explanation}"
                    : $"Incorrect. The correct answer is '{question.CorrectAnswer}'. {question.Explanation}"
            };
        }

        public void MoveNext()
        {
            if (!IsFinished)
            {
                _currentIndex++;
            }
        }

        public void ResetQuiz()
        {
            _currentIndex = 0;
            _score = 0;
        }

        public string GetFinalScore()
        {
            return $"{_score} out of {_questions.Count}";
        }

        public string GetFinalMessage()
        {
            double percentage = _questions.Count == 0
                ? 0
                : (double)_score / _questions.Count * 100;

            if (percentage >= 80)
            {
                return "Great job! You're a cybersecurity pro!";
            }

            if (percentage >= 60)
            {
                return "Good work! Review the explanations and keep strengthening your knowledge.";
            }

            return "Keep learning to stay safe online. Try the quiz again after reviewing the chatbot tips.";
        }

        private List<QuizQuestion> CreateQuestions()
        {
            return new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if an unexpected email asks for your password?",
                    Options = new List<string>
                    {
                        "Reply with the password",
                        "Click the link and sign in",
                        "Report the email as phishing",
                        "Forward it to friends"
                    },
                    CorrectAnswer = "Report the email as phishing",
                    Explanation = "Legitimate organisations should not ask you to send passwords by email. Reporting the message helps block the scam."
                },
                new QuizQuestion
                {
                    Question = "Which password is the strongest?",
                    Options = new List<string>
                    {
                        "Password123",
                        "Zipho2005",
                        "Blue-River!Train-92",
                        "12345678"
                    },
                    CorrectAnswer = "Blue-River!Train-92",
                    Explanation = "A long, unique passphrase is harder to guess or crack than short predictable passwords."
                },
                new QuizQuestion
                {
                    Question = "True or False: It is safe to reuse one strong password on every account.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "False",
                    Explanation = "Password reuse allows one breached account to put all your other accounts at risk.",
                    IsTrueFalse = true
                },
                new QuizQuestion
                {
                    Question = "What does two-factor authentication (2FA) add to an account?",
                    Options = new List<string>
                    {
                        "A second verification step",
                        "A second username",
                        "A public password",
                        "A faster internet connection"
                    },
                    CorrectAnswer = "A second verification step",
                    Explanation = "2FA requires another proof of identity, such as an authenticator code, in addition to the password."
                },
                new QuizQuestion
                {
                    Question = "Which action is safest on public Wi-Fi?",
                    Options = new List<string>
                    {
                        "Do online banking without protection",
                        "Share files with everyone",
                        "Use trusted sites and avoid sensitive transactions",
                        "Turn off your device password"
                    },
                    CorrectAnswer = "Use trusted sites and avoid sensitive transactions",
                    Explanation = "Public networks can be monitored, so avoid sensitive activity and use secure connections."
                },
                new QuizQuestion
                {
                    Question = "True or False: HTTPS guarantees that every website is honest and safe.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "False",
                    Explanation = "HTTPS encrypts the connection, but scammers can also obtain HTTPS certificates for fake websites."
                },
                new QuizQuestion
                {
                    Question = "What is social engineering?",
                    Options = new List<string>
                    {
                        "Building social media apps",
                        "Manipulating people into revealing information",
                        "Repairing computer hardware",
                        "Encrypting a database"
                    },
                    CorrectAnswer = "Manipulating people into revealing information",
                    Explanation = "Social engineering attacks human trust, fear, urgency, or curiosity rather than only attacking technology."
                },
                new QuizQuestion
                {
                    Question = "What should you do before installing a software update?",
                    Options = new List<string>
                    {
                        "Use an official source",
                        "Download it from a random pop-up",
                        "Disable all security tools permanently",
                        "Send your password to the developer"
                    },
                    CorrectAnswer = "Use an official source",
                    Explanation = "Updates should come from the operating system, official app store, or verified vendor website."
                },
                new QuizQuestion
                {
                    Question = "Which statement best describes ransomware?",
                    Options = new List<string>
                    {
                        "Software that improves battery life",
                        "Malware that locks or encrypts data and demands payment",
                        "A secure backup tool",
                        "A type of strong password"
                    },
                    CorrectAnswer = "Malware that locks or encrypts data and demands payment",
                    Explanation = "Ransomware blocks access to data, often through encryption, and demands money from the victim."
                },
                new QuizQuestion
                {
                    Question = "True or False: You should review app permissions and remove access the app does not need.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = "True",
                    Explanation = "Limiting permissions reduces unnecessary access to your location, camera, contacts, and other data."
                },
                new QuizQuestion
                {
                    Question = "Why are backups important?",
                    Options = new List<string>
                    {
                        "They make phishing emails disappear",
                        "They help recover files after loss, damage, or ransomware",
                        "They replace antivirus software",
                        "They automatically create stronger passwords"
                    },
                    CorrectAnswer = "They help recover files after loss, damage, or ransomware",
                    Explanation = "A tested backup provides a separate copy of important data when the original becomes unavailable."
                },
                new QuizQuestion
                {
                    Question = "What is the safest response to a message creating urgent pressure to send money?",
                    Options = new List<string>
                    {
                        "Act immediately",
                        "Send a small amount first",
                        "Pause and verify through a trusted contact method",
                        "Share your banking PIN"
                    },
                    CorrectAnswer = "Pause and verify through a trusted contact method",
                    Explanation = "Scammers use urgency to stop people from thinking. Verify independently before taking action."
                }
            };
        }
    }
}
