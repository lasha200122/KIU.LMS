using System.Text.Json;
using KIU.LMS.Domain.Common.Enums.Question;
using KIU.LMS.Domain.Entities.NoSQL;
using Microsoft.Extensions.Logging;

namespace KIU.LMS.Application.Features.AIProcessing;

public sealed record GradeQuizCommand(Guid ExamResultId) : IRequest<AIProcessingResult>;

public sealed class GradeQuizCommandHandler(
    IUnitOfWork _unitOfWork,
    IMongoRepository<ExamSession> _sessionRepository,
    IMongoRepository<StudentAnswer> _answerRepository,
    IMongoRepository<Question> _questionRepository,
    IAiGradingService _aiService,
    ILogger<GradeQuizCommandHandler> _logger)
    : IRequestHandler<GradeQuizCommand, AIProcessingResult>
{
    public async Task<AIProcessingResult> Handle(GradeQuizCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var examResult = await _unitOfWork.ExamResultRepository.FirstOrDefaultWithTrackingAsync(
                x => x.Id == request.ExamResultId, cancellationToken);

            if (examResult == null)
            {
                _logger.LogWarning("ExamResult {ExamResultId} not found", request.ExamResultId);
                return new AIProcessingResult(false, "{}", "ExamResult not found");
            }
            
            var session = await _sessionRepository.GetByIdAsync(examResult.SessionId);
            if (session == null)
            {
                _logger.LogWarning("Session {SessionId} not found", examResult.SessionId);
                return new AIProcessingResult(false, "{}", "Session not found");
            }

            var answers = await _answerRepository.FindAsync(a => a.SessionId == session.Id);
            var answersList = answers.ToList();

            decimal totalScore = 0;
            int correctCount = 0;
            
            var ipeqQuestions = session.Questions.Where(q => q.Type == QuestionType.IPEQ).ToList();
            
            foreach (var ipeqQuestion in ipeqQuestions)
            {
                var answer = answersList.FirstOrDefault(a => a.QuestionId == ipeqQuestion.QuestionId);
                
                if (answer?.StudentCode == null)  
                {
                    _logger.LogWarning("StudentCode missing for IPEQ question {QuestionId}", ipeqQuestion.QuestionId);
                    continue;
                }

                var question = await _questionRepository.FindOneAsync(q => q.Id == ipeqQuestion.QuestionId);
                if (question == null)
                {
                    _logger.LogWarning("Question {QuestionId} not found in DB", ipeqQuestion.QuestionId);
                    continue;
                }

                _logger.LogInformation("Grading IPEQ question {QuestionId}", ipeqQuestion.QuestionId);
                
                var gradeResult = await _aiService.GradeAsync(
                    question.TaskDescription ?? "",
                    question.ReferenceSolution ?? "",
                    answer.StudentCode,  
                    question.CodeGradingPrompt ?? "",
                    10);

                if (gradeResult != null && TryParseGrade(gradeResult, out int grade))
                {
                    _logger.LogInformation("IPEQ question {QuestionId} graded: {Grade}/10", ipeqQuestion.QuestionId, grade);
                    totalScore += grade;
                    if (grade >= 5) correctCount++;
                }
                else
                {
                    _logger.LogWarning("Failed to parse grade for IPEQ question {QuestionId}", ipeqQuestion.QuestionId);
                }
            }
            
            var c2rsQuestions = session.Questions.Where(q => q.Type == QuestionType.C2RS).ToList();
            
            foreach (var c2rsQuestion in c2rsQuestions)
            {
                var answer = answersList.FirstOrDefault(a => a.QuestionId == c2rsQuestion.QuestionId);
                
                if (answer?.StudentPrompt == null)  
                {
                    _logger.LogWarning("StudentPrompt missing for C2RS question {QuestionId}", c2rsQuestion.QuestionId);
                    continue;
                }

                var question = await _questionRepository.FindOneAsync(q => q.Id == c2rsQuestion.QuestionId);
                if (question == null)
                {
                    _logger.LogWarning("Question {QuestionId} not found in DB", c2rsQuestion.QuestionId);
                    continue;
                }

                _logger.LogInformation("Generating code from prompt for C2RS question {QuestionId}", c2rsQuestion.QuestionId);
                
                var generatedCode = await _aiService.GenerateCodeFromPromptAsync(answer.StudentPrompt);
                
                if (generatedCode == null)
                {
                    _logger.LogWarning("Failed to generate code for C2RS question {QuestionId}", c2rsQuestion.QuestionId);
                    continue;
                }

                answer.SetGeneratedCode(generatedCode);
                await _answerRepository.UpdateAsync(answer);
                
                _logger.LogInformation("Grading generated code for C2RS question {QuestionId}", c2rsQuestion.QuestionId);

                var gradeResult = await _aiService.GradeAsync(
                    question.TaskDescription ?? "",
                    question.ReferenceSolution ?? "",
                    generatedCode, 
                    question.CodeGradingPrompt ?? "",
                    10);

                if (gradeResult != null && TryParseGrade(gradeResult, out int grade))
                {
                    _logger.LogInformation("C2RS question {QuestionId} graded: {Grade}/10", c2rsQuestion.QuestionId, grade);
                    totalScore += grade;
                    if (grade >= 5) correctCount++;
                }
                else
                {
                    _logger.LogWarning("Failed to parse grade for C2RS question {QuestionId}", c2rsQuestion.QuestionId);
                }
            }
            
            _logger.LogInformation("Quiz grading completed. Total score: {TotalScore}, Correct: {CorrectCount}", totalScore, correctCount);
            
            examResult.UpdateScore(totalScore, correctCount);
            await _unitOfWork.SaveChangesAsync();

            return new AIProcessingResult(true, $"{{\"score\": {totalScore}, \"correctCount\": {correctCount}}}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to grade quiz {ExamResultId}", request.ExamResultId);
            return new AIProcessingResult(false, "{}", ex.Message);
        }
    }

    private bool TryParseGrade(string gradeResultJson, out int grade)
    {
        grade = 0;
        try
        {
            var json = JsonDocument.Parse(gradeResultJson).RootElement;
            
            if (!json.TryGetProperty("grade", out var gradeProp))
                return false;

            return gradeProp.ValueKind == JsonValueKind.Number
                ? (grade = gradeProp.GetInt32()) >= 0
                : (gradeProp.ValueKind == JsonValueKind.String
                   && int.TryParse(gradeProp.GetString(), out grade));
        }
        catch
        {
            return false;
        }
    }
}
