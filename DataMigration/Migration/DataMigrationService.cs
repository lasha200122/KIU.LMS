using System.Text;
using KIU.LMS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace DataMigration.Migration;

public class DataMigrationService
{
    private readonly KiuLmsContext _old;
    private readonly LmsDbContext _new;

    public DataMigrationService(KiuLmsContext oldDb, LmsDbContext newDb)
    {
        _old = oldDb;
        _new = newDb;
    }

    public async Task RunAsync()
    {
        await MigrateUsers();
        await Task.Delay(3);
        await MigrateCourses();
        await Task.Delay(3);
        await MigrateCourseMaterials();
        await Task.Delay(3);
        await MigrateTopics();
        await Task.Delay(3);
        await MigrateModules();
        await Task.Delay(3);
        await MigrateModuleBanks();
        await Task.Delay(3);
        await MigrateSubModules();
        await Task.Delay(3);
        await MigrateQuizzes();
        await Task.Delay(3);
        await MigrateExamResults();
        await Task.Delay(3);
        await MigrateQuestionBanks();
        await Task.Delay(3);
        await MigrateQuizBanks();
        await Task.Delay(3);
        await MigrateUserCourses();
        await Task.Delay(3);
        await MigrateAssignments();
        await Task.Delay(3);
        await MigrateSolutions();

        // await MigrateUserDevices();
        // await MigrateFileRecords();
        // await MigrateLoginAttempts();
    }

    private const int BatchSize = 1000;

    private async Task<List<T>> LoadBatch<T>(IQueryable<T> query, int skip)
    {
        return await query
            .Skip(skip)
            .Take(BatchSize)
            .ToListAsync();
    }

    private async Task MigrateUsers()
    {
        Console.WriteLine("Migrating Users...");
        int offset = 0;

        while (true)
        {
            var batch = await LoadBatch(_old.Users.OrderBy(x => x.Id), offset);
            if (batch.Count == 0) break;

            var sql = new StringBuilder();


            foreach (var u in batch)
            {
                if (u.Email == null) continue;

                var firstName = u.FirstName?.Replace("'", "''") ?? "";
                var lastName = u.LastName?.Replace("'", "''") ?? "";

                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""User"" 
                (
                    ""Id"", ""Email"", ""FirstName"", ""LastName"",
                    ""PasswordHash"", ""PasswordSalt"", ""Institution"",
                    ""Role"", ""EmailVerified"", ""CreateUserId"",
                    ""CreateDate"", ""LastUpdateDate"", ""DeleteDate"", ""IsDeleted""
                )
                VALUES 
                (
                    {u.Id}, {u.Email}, {firstName}, {lastName},
                    {u.PasswordHash}, {u.PasswordSalt}, {u.Institution},
                    {1}, {false}, {u.CreateUserId}, 
                    {u.CreateDate.UtcDateTime}, {u.LastUpdateDate}, {u.DeleteDate}, {u.IsDeleted}
                )
                ON CONFLICT (""Email"") DO NOTHING;
            ");
            }

            await _new.Database.ExecuteSqlRawAsync(sql.ToString());
            offset += BatchSize;
        }
    }

    private async Task MigrateCourses()
    {
        Console.WriteLine("Migrating Courses...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.Courses
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(500)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var c in batch)
            {
                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""Course""
                (
                    ""Id"",
                    ""Name"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId""
                )
                VALUES
                (
                    {c.Id},
                    {c.Name},
                    {c.CreateDate.UtcDateTime},
                    {c.CreateUserId},
                    {c.LastUpdateDate},
                    {c.LastUpdateUserId},
                    {c.IsDeleted},
                    {c.DeleteDate},
                    {c.DeleteUserId}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 500;
        }
    }

    private async Task MigrateCourseMaterials()
    {
        Console.WriteLine("Migrating CourseMaterials...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.CourseMaterials
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(500)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var m in batch)
            {
                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""CourseMaterial""
                (
                    ""Id"",
                    ""CourseId"",
                    ""CourseMaterialParentId"",
                    ""Name"",
                    ""Url"",
                    ""Order"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId""
                )
                VALUES
                (
                    {m.Id},
                    {m.CourseId},
                    {m.CourseMaterialParentId},
                    {m.Name},
                    {m.Url},
                    {m.Order},
                    {m.CreateDate.UtcDateTime},
                    {m.CreateUserId},
                    {m.LastUpdateDate},
                    {m.LastUpdateUserId},
                    {m.IsDeleted},
                    {m.DeleteDate},
                    {m.DeleteUserId}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 500;
        }
    }

    private async Task MigrateTopics()
    {
        Console.WriteLine("Migrating Topics...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.Topics
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(500)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var t in batch)
            {
                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""Topic""
                (
                    ""Id"",
                    ""CourseId"",
                    ""Name"",
                    ""StartDateTime"",
                    ""EndDateTime"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId""
                )
                VALUES
                (
                    {t.Id},
                    {t.CourseId},
                    {t.Name},
                    {t.StartDateTime},
                    {t.EndDateTime},
                    {t.CreateDate.UtcDateTime},
                    {t.CreateUserId},
                    {t.LastUpdateDate},
                    {t.LastUpdateUserId},
                    {t.IsDeleted},
                    {t.DeleteDate},
                    {t.DeleteUserId}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 500;
        }
    }

    private async Task MigrateModules()
    {
        Console.WriteLine("Migrating Modules...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.Modules
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(500)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var m in batch)
            {
                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""Module""
                (
                    ""Id"",
                    ""CourseId"",
                    ""Name"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId""
                )
                VALUES
                (
                    {m.Id},
                    {m.CourseId},
                    {m.Name},
                    {m.CreateDate.UtcDateTime},
                    {m.CreateUserId},
                    {m.LastUpdateDate},
                    {m.LastUpdateUserId},
                    {m.IsDeleted},
                    {m.DeleteDate},
                    {m.DeleteUserId}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 500;
        }
    }

    private async Task MigrateModuleBanks()
    {
        Console.WriteLine("Migrating ModuleBanks...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.ModuleBanks
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(500)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var mb in batch)
            {
                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""ModuleBank""
                (
                    ""Id"",
                    ""Name"",
                    ""ModuleId"",
                    ""Type"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId""
                )
                VALUES
                (
                    {mb.Id},
                    {mb.Name},
                    {mb.ModuleId},
                    {mb.Type},   
                    {mb.CreateDate.UtcDateTime},
                    {mb.CreateUserId},
                    {mb.LastUpdateDate},
                    {mb.LastUpdateUserId},
                    {mb.IsDeleted},
                    {mb.DeleteDate},
                    {mb.DeleteUserId}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 500;
        }
    }

    private async Task MigrateSubModules()
    {
        Console.WriteLine("Migrating SubModules...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.SubModules
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(500)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var s in batch)
            {
                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""SubModule""
                (
                    ""Id"",
                    ""ModuleBankId"",
                    ""TaskDescription"",
                    ""CodeSolution"",
                    ""CodeGenerationPrompt"",
                    ""CodeGraidingPrompt"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId"",
                    ""Solution"",
                    ""Difficulty""
                )
                VALUES
                (
                    {s.Id},
                    {s.ModuleBankId},
                    {s.TaskDescription},
                    {s.CodeSolution},
                    {s.CodeGenerationPrompt},
                    {s.CodeGraidingPrompt},
                    {s.CreateDate.UtcDateTime},
                    {s.CreateUserId},
                    {s.LastUpdateDate},
                    {s.LastUpdateUserId},
                    {s.IsDeleted},
                    {s.DeleteDate},
                    {s.DeleteUserId},
                    {s.Solution},
                    {s.Difficulty}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 500;
        }
    }

    private async Task MigrateQuizzes()
    {
        Console.WriteLine("Migrating Quizzes...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.Quizzes
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(500)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var q in batch)
            {
                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""Quiz""
                (
                    ""Id"",
                    ""CourseId"",
                    ""TopicId"",
                    ""Title"",
                    ""Type"",
                    ""Order"",
                    ""Attempts"",
                    ""StartDateTime"",
                    ""EndDateTime"",
                    ""Score"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId"",
                    ""Explanation"",
                    ""TimePerQuestion"",
                    ""MinusScore"",
                    ""PublicTill"",
                    ""IsTraining""
                )
                VALUES
                (
                    {q.Id},
                    {q.CourseId},
                    {q.TopicId},
                    {q.Title},
                    {q.Type},
                    {q.Order},
                    {q.Attempts},
                    {q.StartDateTime.UtcDateTime},
                    {q.EndDateTime},
                    {q.Score},
                    {q.CreateDate.UtcDateTime},
                    {q.CreateUserId},
                    {q.LastUpdateDate},
                    {q.LastUpdateUserId},
                    {q.IsDeleted},
                    {q.DeleteDate},
                    {q.DeleteUserId},
                    {q.Explanation},
                    {q.TimePerQuestion},
                    {q.MinusScore},
                    {q.PublicTill},
                    {q.IsTraining}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 500;
        }
    }

    private async Task MigrateExamResults()
    {
        Console.WriteLine("Migrating ExamResults...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.ExamResults
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(500)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var e in batch)
            {
                if (!await _new.Users.AnyAsync(x => x.Id == e.StudentId))
                    continue;

                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""ExamResult""
                (
                    ""Id"", ""StudentId"", ""QuizId"",
                    ""StartedAt"", ""FinishedAt"", ""Score"",
                    ""TotalQuestions"", ""CorrectAnswers"", ""Duration"",
                    ""SessionId"", ""CreateDate"", ""CreateUserId"",
                    ""LastUpdateDate"", ""LastUpdateUserId"",
                    ""IsDeleted"", ""DeleteDate"", ""DeleteUserId""
                )
                VALUES
                (
                    {e.Id}, {e.StudentId}, {e.QuizId},
                    {e.StartedAt.UtcDateTime}, {e.FinishedAt.UtcDateTime}, {e.Score},
                    {e.TotalQuestions}, {e.CorrectAnswers}, {e.Duration.ToTimeSpan()},
                    {e.SessionId}, {e.CreateDate.UtcDateTime}, {e.CreateUserId},
                    {e.LastUpdateDate},
                    {e.LastUpdateUserId}, {e.IsDeleted},
                    {e.DeleteDate},
                    {e.DeleteUserId}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 500;
        }
    }

    private async Task MigrateQuestionBanks()
    {
        Console.WriteLine("Migrating QuestionBanks...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.QuestionBanks
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(500)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var qb in batch)
            {
                if (!await _new.Modules.AnyAsync(x => x.Id == qb.ModuleId))
                    continue;

                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""QuestionBank""
                (
                    ""Id"",
                    ""Name"",
                    ""ModuleId"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId""
                )
                VALUES
                (
                    {qb.Id},
                    {qb.Name},
                    {qb.ModuleId},
                    {qb.CreateDate.UtcDateTime},
                    {qb.CreateUserId},
                    {qb.LastUpdateDate},
                    {qb.LastUpdateUserId},
                    {qb.IsDeleted},
                    {qb.DeleteDate},
                    {qb.DeleteUserId}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 500;
        }
    }

    private async Task MigrateQuizBanks()
    {
        Console.WriteLine("Migrating QuizBank...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.QuizBanks
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(500)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var qb in batch)
            {
                if (!await _new.Quizzes.AnyAsync(x => x.Id == qb.QuizId))
                    continue;

                if (!await _new.QuestionBanks.AnyAsync(x => x.Id == qb.QuestionBankId))
                    continue;

                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""QuizBank""
                (
                    ""Id"",
                    ""QuizId"",
                    ""QuestionBankId"",
                    ""Amount"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId""
                )
                VALUES
                (
                    {qb.Id},
                    {qb.QuizId},
                    {qb.QuestionBankId},
                    {qb.Amount},
                    {qb.CreateDate.UtcDateTime},
                    {qb.CreateUserId},
                    {qb.LastUpdateDate},
                    {qb.LastUpdateUserId},
                    {qb.IsDeleted},
                    {qb.DeleteDate},
                    {qb.DeleteUserId}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 500;
        }
    }

    private async Task MigrateUserCourses()
    {
        Console.WriteLine("Migrating UserCourse...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.UserCourses
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(500)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var uc in batch)
            {
                if (!await _new.Users.AnyAsync(x => x.Id == uc.UserId))
                    continue;

                if (!await _new.Courses.AnyAsync(x => x.Id == uc.CourseId))
                    continue;

                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""UserCourse""
                (
                    ""Id"",
                    ""UserId"",
                    ""CourseId"",
                    ""CanAccessTill"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId""
                )
                VALUES
                (
                    {uc.Id},
                    {uc.UserId},
                    {uc.CourseId},
                    {uc.CanAccessTill.UtcDateTime},
                    {uc.CreateDate.UtcDateTime},
                    {uc.CreateUserId},
                    {uc.LastUpdateDate},
                    {uc.LastUpdateUserId},
                    {uc.IsDeleted},
                    {uc.DeleteDate},
                    {uc.DeleteUserId}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 500;
        }
    }

    private async Task MigrateAssignments()
    {
        Console.WriteLine("Migrating Assignments...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.Assignments
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(300)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var a in batch)
            {
                if (!await _new.Courses.AnyAsync(x => x.Id == a.CourseId))
                    continue;

                if (!await _new.Topics.AnyAsync(x => x.Id == a.TopicId))
                    continue;

                if (a.PromptId.HasValue)
                {
                    if (!await _new.Prompts.AnyAsync(x => x.Id == a.PromptId))
                        continue;
                }

                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""Assignment""
                (
                    ""Id"",
                    ""CourseId"",
                    ""TopicId"",
                    ""Type"",
                    ""Name"",
                    ""Order"",
                    ""StartDateTime"",
                    ""EndDateTime"",
                    ""Score"",
                    ""Problem"",
                    ""Code"",
                    ""FileName"",
                    ""IsPublic"",
                    ""AIGrader"",
                    ""SolutionType"",
                    ""PromptId"",
                    ""FullScreen"",
                    ""RuntimeAttempt"",
                    ""IsTraining"",
                    ""PromptText"",
                    ""CodeSolution"",
                    ""ValidationsCount"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId""
                )
                VALUES
                (
                    {a.Id},
                    {a.CourseId},
                    {a.TopicId},
                    {a.Type},
                    {a.Name},
                    {a.Order},
                    {a.StartDateTime},
                    {a.EndDateTime},
                    {a.Score},
                    {a.Problem},
                    {a.Code},
                    {a.FileName},
                    {a.IsPublic},
                    {a.Aigrader},
                    {a.SolutionType},
                    {a.PromptId},
                    {a.FullScreen},
                    {a.RuntimeAttempt},
                    {a.IsTraining},
                    {a.GraidingPrompt ?? a.PromptText},
                    {a.CodeSolution},
                    {0}, 
                    {a.CreateDate.UtcDateTime},
                    {a.CreateUserId},
                    {a.LastUpdateDate},
                    {a.LastUpdateUserId},
                    {a.IsDeleted},
                    {a.DeleteDate},
                    {a.DeleteUserId}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 300;
        }
    }

    private async Task MigrateSolutions()
    {
        Console.WriteLine("Migrating Solutions...");
        int offset = 0;

        while (true)
        {
            var batch = await _old.Solutions
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(300)
                .AsNoTracking()
                .ToListAsync();

            if (batch.Count == 0)
                break;

            foreach (var s in batch)
            {
                if (!await _new.Assignments.AnyAsync(x => x.Id == s.AssignmentId))
                    continue;

                if (!await _new.Users.AnyAsync(x => x.Id == s.UserId))
                    continue;

                await _new.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO ""Solution""
                (
                    ""Id"",
                    ""AssignmentId"",
                    ""UserId"",
                    ""Value"",
                    ""Grade"",
                    ""FeedBack"",
                    ""GradingStatus"",
                    ""CreateDate"",
                    ""CreateUserId"",
                    ""LastUpdateDate"",
                    ""LastUpdateUserId"",
                    ""IsDeleted"",
                    ""DeleteDate"",
                    ""DeleteUserId""
                )
                VALUES
                (
                    {s.Id},
                    {s.AssignmentId},
                    {s.UserId},
                    {s.Value},
                    {s.Grade},
                    {s.FeedBack},
                    {s.GradingStatus},
                    {s.CreateDate.UtcDateTime},
                    {s.CreateUserId},
                    {s.LastUpdateDate},
                    {s.LastUpdateUserId},
                    {s.IsDeleted},
                    {s.DeleteDate},
                    {s.DeleteUserId}
                )
                ON CONFLICT (""Id"") DO NOTHING;
            ");
            }

            offset += 300;
        }
    }
}