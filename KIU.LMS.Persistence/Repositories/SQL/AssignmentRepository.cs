using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class AssignmentRepository(LmsDbContext _context) : BaseRepository<Assignment>(_context), IAssignmentRepository
{
    public async Task<Assignment?> GetByIdAsync(Guid id)
    {
        return await _context.Assignments
            .Include(a => a.Course)
            .Include(a => a.Topic)
            .Include(a => a.Solutions)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Assignment?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.Assignments
            .Include(a => a.Course)
                .ThenInclude(c => c.UserCourses)
            .Include(a => a.Topic)
            .Include(a => a.Prompt)
            .Include(a => a.Solutions)
                .ThenInclude(s => s.User)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Assignment>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Assignments
            .Include(a => a.Topic)
            .Include(a => a.Solutions)
            .Where(a => a.CourseId == courseId)
            .OrderBy(a => a.Order)
            .ToListAsync();
    }

    public async Task<IEnumerable<Assignment>> GetByTopicIdAsync(Guid topicId)
    {
        return await _context.Assignments
            .Include(a => a.Course)
            .Include(a => a.Solutions)
            .Where(a => a.TopicId == topicId)
            .OrderBy(a => a.Order)
            .ToListAsync();
    }
}
