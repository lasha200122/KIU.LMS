using Microsoft.EntityFrameworkCore;

namespace KIU.LMS.Persistence.Repositories.SQL;

public sealed class TopicRepository(LmsDbContext _context) : BaseRepository<Topic>(_context) , ITopicRepository
{
    public async Task<IEnumerable<Topic>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Topics
            .Include(t => t.Assignments)
                .ThenInclude(a => a.Solutions)
            .Where(t => t.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<Topic?> GetByIdAsync(Guid id)
    {
        return await _context.Topics
            .Include(t => t.Course)
            .Include(t => t.Assignments)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}
