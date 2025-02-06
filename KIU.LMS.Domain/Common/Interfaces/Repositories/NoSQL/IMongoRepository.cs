namespace KIU.LMS.Domain.Common.Interfaces.Repositories.NoSQL;

public interface IMongoRepository<TDocument> where TDocument : IDocument
{
    Task<IEnumerable<TDocument>> GetAllAsync();
    Task<TDocument?> GetByIdAsync(string id);
    Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression);
    Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> filterExpression);
    Task<IEnumerable<TProjected>> FindAsync<TProjected>(
        Expression<Func<TDocument, bool>> filterExpression,
        Expression<Func<TDocument, TProjected>> projectionExpression);
    Task InsertOneAsync(TDocument document);
    Task InsertManyAsync(ICollection<TDocument> documents);
    Task ReplaceOneAsync(TDocument document);
    Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression);
    Task DeleteByIdAsync(string id);
    Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression);
    Task<bool> ExistsAsync(Expression<Func<TDocument, bool>> filterExpression);
    Task<PagedEntities<TDocument>> GetPagedDataAsync(
        Expression<Func<TDocument, bool>>? filterExpression = null,
        int pageNumber = 1,
        int pageSize = 10);

    Task<long> CountAsync(Expression<Func<TDocument, bool>>? filterExpression = null);
    Task CreateAsync(TDocument document);
    Task UpdateAsync(TDocument document);
}