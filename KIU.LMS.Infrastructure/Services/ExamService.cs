using KIU.LMS.Domain.Common.Enums.Question;
using KIU.LMS.Domain.Common.Interfaces.Repositories.SQL.Base;
using KIU.LMS.Domain.Entities.NoSQL;

namespace KIU.LMS.Infrastructure.Services;

public class ExamService(
    IUnitOfWork _unitOfWork,
    IMongoRepository<ExamSession> _sessionRepository,
    IMongoRepository<StudentAnswer> _answerRepository,
    IMongoRepository<Question> _questionsRepository) : IExamService
{
    public async Task<ExamSession> StartExamAsync(Guid studentId, Guid quizId)
    {
        var quiz = await _unitOfWork.QuizRepository.SingleOrDefaultAsync(x => x.Id == quizId, x => x.QuizBanks);

        if (quiz == null)
            throw new Exception("Quiz not found");

        var activeSession = await _sessionRepository.FindOneAsync(s =>
            s.StudentId == studentId.ToString() &&
            s.QuizId == quizId.ToString() &&
            s.Status == ExamStatus.InProgress);

        if (activeSession is not null)
            return activeSession;

        var questions = new List<ExamQuestion>();
        foreach (var quizBank in quiz.QuizBanks)
        {
            var randomQuestions = await _questionsRepository.FindAsync(x => x.QuestionBankId == quizBank.QuestionBankId.ToString());

            if (randomQuestions != null) 
            {
                var questionsToAdd = randomQuestions
                    .OrderBy(x => Guid.NewGuid())
                    .Take(quizBank.Amount)
                    .Select(x => MapToExamQuestion(x, quiz.TimePerQuestion));

                questions.AddRange(questionsToAdd);
            }
        }

        int? totalDuration = quiz.TimePerQuestion.HasValue
            ? quiz.TimePerQuestion.Value * questions.Count
            : null;

        var session = new ExamSession(
            studentId.ToString(),
            quizId.ToString(),
            questions,
            totalDuration);

        await _sessionRepository.CreateAsync(session);
        return session;
    }
    public async Task<ExamQuestion?> GetCurrentQuestionAsync(string sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);

        if (session is not { Status: ExamStatus.InProgress })
            return null;
        
        if (session.IsExamTimeExpired)
        {
            await FinishExamAsync(sessionId);
            return null;
        }

        var currentQuestion = session.CurrentQuestion;
        if (currentQuestion?.IsTimeExpired == true)
        {
            session.MoveToNextQuestion();
            session.StartQuestion();
            await _sessionRepository.UpdateAsync(session);
            return session.CurrentQuestion;
        }

        if (currentQuestion != null && !currentQuestion.StartedAt.HasValue)
        {
            session.StartQuestion();
            await _sessionRepository.UpdateAsync(session);
        }

        return currentQuestion;
    }

    public async Task<bool> SubmitAnswerAsync(
        string sessionId, 
        string questionId, 
        List<string>? selectedOptions = null,
        string? studentCode = null,
        string? studentPrompt = null)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || !session.CanAnswerCurrentQuestion() ||
            session.CurrentQuestion?.QuestionId != questionId)
            return false;

        var currentQuestion = session.CurrentQuestion;
        StudentAnswer answer;
    
        if (currentQuestion.Type == QuestionType.Multiple || currentQuestion.Type == QuestionType.Single)
        {
            if (selectedOptions == null || !selectedOptions.Any())
                return false;
            
            answer = new StudentAnswer(sessionId, questionId, selectedOptions);
        }
        else if (currentQuestion.Type == QuestionType.IPEQ)
        {
            if (string.IsNullOrWhiteSpace(studentCode))
                return false;
        
            answer = new StudentAnswer(sessionId, questionId, studentCode);
        }
        else if (currentQuestion.Type == QuestionType.C2RS)
        {
            if (string.IsNullOrWhiteSpace(studentPrompt))
                return false;
        
            answer = new StudentAnswer(sessionId, questionId, studentPrompt);
        }
        else
        {
            return false;
        }

        await _answerRepository.CreateAsync(answer);

        session.MoveToNextQuestion();
        session.UpdateExamStatus();
        await _sessionRepository.UpdateAsync(session);

        if (session.Status == ExamStatus.Completed) 
        {
            await SaveResult(session);
        }

        return true;
    }


    public async Task<List<string>> GetStudentAnswer(string sessionId, string questionId) 
    {
        return (await _answerRepository.FindOneAsync(x => x.SessionId == sessionId && x.QuestionId == questionId)).SelectedOptions;
    }

    public async Task<ExamSession?> GetSessionByIdAsync(string sessionId)
    {
        return await _sessionRepository.GetByIdAsync(sessionId);
    }

    public async Task<bool> PauseExamAsync(string sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || session.Status != ExamStatus.InProgress)
            return false;

        session.Paused();
        await _sessionRepository.UpdateAsync(session);
        return true;
    }

    public async Task<bool> ResumeExamAsync(string sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || session.Status != ExamStatus.Paused)
            return false;

        session.InProggress();

        await _sessionRepository.UpdateAsync(session);
        return true;
    }

    public async Task<bool> FinishExamAsync(string sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || session.Status == ExamStatus.Completed)
            return false;

        session.FinishExam();

        var answers = await _answerRepository.FindAsync(a => a.SessionId == sessionId);
        var quiz = await _unitOfWork.QuizRepository.SingleOrDefaultAsync(x => x.Id == new Guid(session.QuizId));

        if (quiz != null)
        {
            var examResult = new ExamResult(
                Guid.NewGuid(),
                new Guid(session.StudentId),
                new Guid(session.QuizId),
                session.StartedAt,
                DateTimeOffset.UtcNow,
                CalculateScore(answers.ToList(), session.Questions, quiz.Type, quiz.MinusScore),
                session.Questions.Count,
                CountAnswers(answers.ToList(), session.Questions).CorrectCount,
                session.Id,
                new Guid(session.StudentId));

            await _unitOfWork.ExamResultRepository.AddAsync(examResult);
            await _unitOfWork.SaveChangesAsync();
        }

        await _sessionRepository.UpdateAsync(session);
        return true;
    }

    private async Task SaveResult(ExamSession session) 
    {
        var answers = await _answerRepository.FindAsync(a => a.SessionId == session.Id);
        var quiz = await _unitOfWork.QuizRepository.SingleOrDefaultAsync(x => x.Id == new Guid(session.QuizId));
        
        if (quiz != null)
        {
            var examResult = new ExamResult(
                Guid.NewGuid(),
                new Guid(session.StudentId),
                new Guid(session.QuizId),
                session.StartedAt,
                DateTimeOffset.UtcNow,
                CalculateScore(answers.ToList(), session.Questions, quiz.Type, quiz.MinusScore),
                session.Questions.Count,
                CountAnswers(answers.ToList(), session.Questions).CorrectCount,
                session.Id,
                new Guid(session.StudentId));

            await _unitOfWork.ExamResultRepository.AddAsync(examResult);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private decimal CalculateScore(List<StudentAnswer> answers, List<ExamQuestion> questions, QuizType type, decimal? penaltyPerWrongAnswer = null)
    {
        if (questions.Count <= 0)
            return 0;
        
        if (type is QuizType.C2RS or QuizType.IPEQ)
            return -1;
        
        var (correctCount, wrongCount) = CountAnswers(answers, questions);

        decimal baseScore = correctCount;

        if (penaltyPerWrongAnswer.HasValue && penaltyPerWrongAnswer.Value > 0)
        {
            decimal penalty = wrongCount * penaltyPerWrongAnswer.Value;
            return baseScore - penalty;
        }

        return baseScore;
    }

    private (int CorrectCount, int WrongCount) CountAnswers(List<StudentAnswer> answers, List<ExamQuestion> questions)
    {
        int correctCount = 0;
        int wrongCount = 0;
        
        var mcqQuestions = questions
            .Where(q => q.Type == QuestionType.Multiple || q.Type == QuestionType.Single)
            .ToList();

        if (mcqQuestions.Count == 0)
            return (-1, 0);
        
        foreach (var answer in answers)
        {
            var question = mcqQuestions.FirstOrDefault(q => q.QuestionId == answer.QuestionId);
            if (question?.Options != null)
            {
                var correctOptions = question.Options
                    .Where(o => o.IsCorrect)
                    .Select(o => o.Id)
                    .ToList();

                if (answer.SelectedOptions != null &&
                    answer.SelectedOptions.Count == correctOptions.Count &&
                    answer.SelectedOptions.All(correctOptions.Contains))
                {
                    correctCount++;
                }
                else
                {
                    wrongCount++;
                }
            }
        }

        return (correctCount, wrongCount);
    }

    
    private ExamQuestion MapToExamQuestion(Question question, int? timePerQuestion)
    {
        return question.Type switch
        {
            QuestionType.Multiple or QuestionType.Single => new ExamQuestion(
                question.Id,
                question.Text!,
                question.ExplanationCorrectAnswer!,
                question.ExplanationIncorrectAnswer!,
                question.Type,
                question.Options!.OrderBy(x => Guid.NewGuid()).ToList(),
                timePerQuestion,
                null),
            
            QuestionType.IPEQ or QuestionType.C2RS => new ExamQuestion(
                question.Id,
                question.Type,
                question.TaskDescription!,
                question.ReferenceSolution!,
                question.CodeGenerationPrompt!,
                question.CodeGradingPrompt!,
                timePerQuestion,
                null),
            
            _ => throw new NotSupportedException($"Question type {question.Type} is not supported")
        };
    }
}