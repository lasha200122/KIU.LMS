using System.Text.Json;
using System.Text.RegularExpressions;
using KIU.LMS.Domain.Common.Enums.Assignment;
using KIU.LMS.Domain.Common.Interfaces.Services;

namespace KIU.LMS.Domain.Common.Utils;

public static class TaskParser
{
    public static List<GeneratedTaskDraft>? Parse(string raw, GeneratedAssignmentType type)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var tasks = new List<GeneratedTaskDraft>();
        
        var pattern = @"(\d+)\.\s*Task Description:\s*(.+?)\s*Code Solution:\s*(.+?)\s*Code Generation Prompt:\s*(.+?)\s*Code Grading Prompt:\s*(.+?)(?=\d+\.\s*Task Description:|\z)";
        var matches = Regex.Matches(raw, pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

        if (matches.Count == 0)
        {
            return ParseAlternativeFormat(raw, type);
        }

        foreach (Match match in matches)
        {
            if (match.Groups.Count < 6)
                continue;

            var taskDescription = match.Groups[2].Value.Trim();
            var codeSolution = match.Groups[3].Value.Trim();
            var codeGenerationPrompt = match.Groups[4].Value.Trim();
            var codeGradingPrompt = match.Groups[5].Value.Trim();

            if (type == GeneratedAssignmentType.C2RS)
            {
                if (codeGenerationPrompt.Equals("LEAVE_EMPTY", StringComparison.OrdinalIgnoreCase) ||
                    codeGenerationPrompt.Equals("empty", StringComparison.OrdinalIgnoreCase))
                {
                    codeGenerationPrompt = string.Empty;
                }
            }

            if (string.IsNullOrWhiteSpace(taskDescription) ||
                string.IsNullOrWhiteSpace(codeSolution) ||
                string.IsNullOrWhiteSpace(codeGradingPrompt))
                continue;

            if (type == GeneratedAssignmentType.IPEQ && string.IsNullOrWhiteSpace(codeGenerationPrompt))
                continue;

            var draft = new GeneratedTaskDraft(
                taskDescription,
                codeSolution,
                string.IsNullOrWhiteSpace(codeGenerationPrompt) ? null : codeGenerationPrompt,
                codeGradingPrompt
            );

            tasks.Add(draft);
        }

        return tasks.Count > 0 ? tasks : null;
    }

    private static List<GeneratedTaskDraft>? ParseAlternativeFormat(string raw, GeneratedAssignmentType type)
    {
        
        var tasks = new List<GeneratedTaskDraft>();
        var lines = raw.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        string? taskDesc = null;
        string? codeSol = null;
        string? codeGenPrompt = null;
        string? codeGradPrompt = null;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            if (trimmed.StartsWith("Task Description:", StringComparison.OrdinalIgnoreCase))
            {
                taskDesc = trimmed.Substring("Task Description:".Length).Trim();
            }
            else if (trimmed.StartsWith("Code Solution:", StringComparison.OrdinalIgnoreCase))
            {
                codeSol = trimmed.Substring("Code Solution:".Length).Trim();
            }
            else if (trimmed.StartsWith("Code Generation Prompt:", StringComparison.OrdinalIgnoreCase))
            {
                codeGenPrompt = trimmed.Substring("Code Generation Prompt:".Length).Trim();
                if (codeGenPrompt.Equals("LEAVE_EMPTY", StringComparison.OrdinalIgnoreCase) ||
                    codeGenPrompt.Equals("empty", StringComparison.OrdinalIgnoreCase))
                {
                    codeGenPrompt = string.Empty;
                }
            }
            else if (trimmed.StartsWith("Code Grading Prompt:", StringComparison.OrdinalIgnoreCase))
            {
                codeGradPrompt = trimmed.Substring("Code Grading Prompt:".Length).Trim();

                if (!string.IsNullOrWhiteSpace(taskDesc) &&
                    !string.IsNullOrWhiteSpace(codeSol) &&
                    !string.IsNullOrWhiteSpace(codeGradPrompt))
                {
                    if (type == GeneratedAssignmentType.C2RS || 
                        (type == GeneratedAssignmentType.IPEQ && !string.IsNullOrWhiteSpace(codeGenPrompt)))
                    {
                        tasks.Add(new GeneratedTaskDraft(
                            taskDesc,
                            codeSol,
                            string.IsNullOrWhiteSpace(codeGenPrompt) ? null : codeGenPrompt,
                            codeGradPrompt
                        ));
                    }
                }

                taskDesc = null;
                codeSol = null;
                codeGenPrompt = null;
                codeGradPrompt = null;
            }
        }

        return tasks.Count > 0 ? tasks : null;
    }

    public static TaskValidationResult? ParseValidationJson(string? json, GeneratedAssignmentType type)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("isValid", out var isValidElement))
                return null;

            var isValid = isValidElement.GetBoolean();
            var reason = root.TryGetProperty("reason", out var reasonElement) 
                ? reasonElement.GetString() ?? "No reason provided"
                : "No reason provided";

            GeneratedTaskDraft? fixedTask = null;

            if (!root.TryGetProperty("fixed", out var fixedElement) || fixedElement.ValueKind != JsonValueKind.Object)
                return new TaskValidationResult(isValid, reason, fixedTask);
            
            var taskDesc = fixedElement.TryGetProperty("taskDescription", out var td) ? td.GetString() : null;
            var codeSol = fixedElement.TryGetProperty("codeSolution", out var cs) ? cs.GetString() : null;
            var codeGenPrompt = fixedElement.TryGetProperty("codeGenerationPrompt", out var cgp) ? cgp.GetString() : null;
            var codeGradPrompt = fixedElement.TryGetProperty("codeGradingPrompt", out var cgrp) ? cgrp.GetString() : null;

            if (string.IsNullOrWhiteSpace(taskDesc) ||
                string.IsNullOrWhiteSpace(codeSol) ||
                string.IsNullOrWhiteSpace(codeGradPrompt))
                return new TaskValidationResult(isValid, reason, fixedTask);
            
            if (type == GeneratedAssignmentType.C2RS)
            {
                codeGenPrompt = null;
            }
            else if (type == GeneratedAssignmentType.IPEQ && string.IsNullOrWhiteSpace(codeGenPrompt))
            {
                return null;
            }

            fixedTask = new GeneratedTaskDraft(taskDesc, codeSol, codeGenPrompt, codeGradPrompt);

            return new TaskValidationResult(isValid, reason, fixedTask);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
