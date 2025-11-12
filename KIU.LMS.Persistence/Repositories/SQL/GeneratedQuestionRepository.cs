namespace KIU.LMS.Persistence.Repositories.SQL;

public class GeneratedQuestionRepository(LmsDbContext db)
    : BaseRepository<GeneratedQuestion>(db), IGeneratedQuestionRepository 
{
    public async Task AddSafetyAsync(GeneratedQuestion q, int maxAllowed, Guid generatedAssigmentId)
    {
        var currentCount = await db.GeneratedQuestions.CountAsync(x => x.GeneratedAssignmentId == generatedAssigmentId);
        if (currentCount < maxAllowed)
            await db.GeneratedQuestions.AddAsync(q);
    }
}