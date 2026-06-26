using System.Collections.Generic;

namespace CybersecurityChatbot
{
    public class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
        public bool IsTrueFalse { get; set; }
    }

    public class QuizAnswerResult
    {
        public bool IsCorrect { get; set; }
        public string Feedback { get; set; } = string.Empty;
    }
}
