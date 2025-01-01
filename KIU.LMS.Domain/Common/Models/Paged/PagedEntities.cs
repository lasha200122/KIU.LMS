namespace KIU.LMS.Domain.Common.Models.Paged;

public record PagedEntities<T>(ICollection<T> Records, int TotalCount);