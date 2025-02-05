using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KIU.LMS.Domain.Common.Models.Excel
{
    public sealed class QuestionExcelDto
    {
        public string Question { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public List<string> IncorrectAnswers { get; set; } = new();

        public QuestionExcelDto(string question, string correctAnswer, List<string> incorrectAnswers)
        {
            Question = question;
            CorrectAnswer = correctAnswer;
            IncorrectAnswers = incorrectAnswers;
        }
    }

}
