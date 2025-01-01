namespace KIU.LMS.Domain.Entities.Base;

public class Aggregate : Entity
{
    public DateTimeOffset CreateDate { get; protected set; }
    public Guid CreateUserId { get; protected set; }
    public DateTimeOffset? LastUpdateDate { get; protected set; }
    public Guid? LastUpdateUserId { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTimeOffset? DeleteDate { get; protected set; }
    public Guid? DeleteUserId { get; protected set; }

    protected Aggregate() { }

    protected Aggregate(Guid id, DateTimeOffset createDate, Guid userId)
    {
        if (id == default)
        {
            throw new Exception($"ობიექტის იდენტიფიკატორი არავალიდურია {GetType().Name}");
        }
        Id = id;

        if (userId == default)
        {
            throw new Exception($"მომხმარებლის იდენტიფიკატორი არავალიდურია {GetType().Name}");
        }
        CreateUserId = userId;
        CreateDate = createDate;
    }

    protected void Update(Guid userId, DateTimeOffset updateDate)
    {
        if (IsDeleted)
        {
            throw new Exception($"წაშლილი ობიექტის განახლება შეუძლებელია {GetType().Name}");
        }

        if (userId == default)
        {
            throw new Exception($"მომხმარებლის იდენტიფიკატორი არავალიდურია {GetType().Name}");
        }
        LastUpdateUserId = userId;
        LastUpdateDate = updateDate;
    }

    public void Delete(Guid userId, DateTimeOffset deleteDate)
    {
        if (IsDeleted)
        {
            throw new Exception($"წაშლილი ობიექტის ხელმეორედ წაშლა შეუძლებელია {GetType().Name}");
        }

        if (userId == default)
        {
            throw new Exception($"მომხმარებლის იდენტიფიკატორი არავალიდურია {GetType().Name}");
        }

        DeleteUserId = userId;
        DeleteDate = deleteDate;
        IsDeleted = true;
    }
}
