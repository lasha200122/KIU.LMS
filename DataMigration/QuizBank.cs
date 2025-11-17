using System;
using System.Collections.Generic;

namespace DataMigration;

public partial class QuizBank
{
    public Guid Id { get; set; }

    public Guid QuizId { get; set; }

    public Guid QuestionBankId { get; set; }

    public int Amount { get; set; }

    public DateTimeOffset CreateDate { get; set; }

    public Guid CreateUserId { get; set; }

    public DateTimeOffset? LastUpdateDate { get; set; }

    public Guid? LastUpdateUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeleteDate { get; set; }

    public Guid? DeleteUserId { get; set; }

    public virtual QuestionBank QuestionBank { get; set; } = null!;

    public virtual Quiz Quiz { get; set; } = null!;
}
