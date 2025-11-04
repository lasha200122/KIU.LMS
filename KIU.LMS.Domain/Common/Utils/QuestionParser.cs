using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using KIU.LMS.Domain.Common.Models.Generating;

namespace KIU.LMS.Domain.Common.Utils;

using System.Text.RegularExpressions;

public static class QuestionParser
{
    public static List<GeneratedQuestionDraft> Parse(string raw)
    {
        var result = new List<GeneratedQuestionDraft>();
        if (string.IsNullOrWhiteSpace(raw))
            return result;

        var blocks = Regex.Split(raw.Trim(), @"(?m)^\d+\.\s*")
            .Where(b => !string.IsNullOrWhiteSpace(b))
            .ToList();

        foreach (var block in blocks)
        {
            var question = Extract(block, @"Question:\s*(.+)");
            var a = Extract(block, @"A\)\s*(.+)");
            var b = Extract(block, @"B\)\s*(.+)");
            var c = Extract(block, @"C\)\s*(.+)");
            var d = Extract(block, @"D\)\s*(.+)");

            var explainCorrect = Extract(block, @"explainCorrectAnswerDescription:\s*(.+)");
            var explainIncorrect = Extract(block, @"explainIncorrectAnswersDescription:\s*(.+)");

            if (string.IsNullOrWhiteSpace(question) || string.IsNullOrWhiteSpace(a))
                continue;

            result.Add(new GeneratedQuestionDraft(
                QuestionText: question.Trim(),
                OptionA: a.Trim(),
                OptionB: b?.Trim() ?? "",
                OptionC: c?.Trim() ?? "",
                OptionD: d?.Trim() ?? "",
                ExplanationCorrect: explainCorrect?.Trim() ?? "",
                ExplanationIncorrect: explainIncorrect?.Trim() ?? ""
            ));
        }

        return result;
    }

    public static QuestionValidationResult? ParseValidationJson(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        raw = raw.Trim()
            .Trim('`')
            .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "", StringComparison.OrdinalIgnoreCase)
            .Trim();

        try
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            bool isValid = root.GetProperty("isValid").GetBoolean();
            string reason = root.GetProperty("reason").GetString() ?? "";

            if (root.TryGetProperty("fixed", out var fixedEl) && fixedEl.ValueKind != JsonValueKind.Null)
            {
                var fixedQ = new GeneratedQuestionDraft(
                    QuestionText: fixedEl.GetProperty("question").GetString() ?? "",
                    OptionA: fixedEl.GetProperty("A").GetString() ?? "",
                    OptionB: fixedEl.GetProperty("B").GetString() ?? "",
                    OptionC: fixedEl.GetProperty("C").GetString() ?? "",
                    OptionD: fixedEl.GetProperty("D").GetString() ?? "",
                    ExplanationCorrect: fixedEl.GetProperty("explainCorrect").GetString() ?? "",
                    ExplanationIncorrect: fixedEl.GetProperty("explainIncorrect").GetString() ?? ""
                );

                return new QuestionValidationResult(isValid, reason, fixedQ);
            }

            return new QuestionValidationResult(isValid, reason, null);
        }
        catch
        {
            return null;
        }
    }

    
    private static string? Extract(string text, string pattern)
    {
        var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }
}
