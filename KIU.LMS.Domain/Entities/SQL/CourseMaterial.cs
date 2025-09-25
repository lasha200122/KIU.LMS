namespace KIU.LMS.Domain.Entities.SQL;

public class CourseMaterial : Aggregate
{
    public Guid CourseId { get; private set; }
    public Guid? CourseMaterialParentId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public int Order { get; private set; }

    public virtual Course Course { get; private set; } = null!;
    public virtual CourseMaterial? Parent { get; private set; }

    private List<CourseMaterial> _children = new();
    public IReadOnlyCollection<CourseMaterial> Children => _children;

    public CourseMaterial() { }

    public CourseMaterial(
        Guid id,
        Guid courseId,
        Guid? parentId,
        string name,
        string url,
        int order,
        Guid createUserId) : base(id, DateTimeOffset.UtcNow, createUserId)
    {
        CourseId = courseId;
        CourseMaterialParentId = parentId;
        Name = name;
        Url = url;
        Order = order;
        Validate(this);
    }

    private void Validate(CourseMaterial material)
    {
        if (material.CourseId == default)
            throw new Exception("კურსის ID სავალდებულოა");
        if (string.IsNullOrEmpty(material.Name))
            throw new Exception("სახელი სავალდებულოა");
        if (material.Order < 0)
            throw new Exception("სორტირების ინდექსი არ შეიძლება იყოს უარყოფითი");
    }
}