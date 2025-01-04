using System.Text.Json;

namespace KIU.LMS.Infrastructure.Services;

public class EmailProcessingService : IEmailProcessingService
{
    public async Task<string> ProcessTemplateAsync(string templateBody, string variables)
    {
        var variablesDictionary = await GetTemplateVariablesAsync(variables);

        var processedTemplate = templateBody;
        foreach (var variable in variablesDictionary!)
        {
            processedTemplate = processedTemplate.Replace($"{{{{{variable.Key}}}}}", variable.Value);
        }
        return processedTemplate;
    }

    private async Task<Dictionary<string, string>> GetTemplateVariablesAsync(string variables)
    {
        return await Task.Run(() => JsonSerializer.Deserialize<Dictionary<string, string>>(variables)!);
    }

    public async Task<(string subject, string body)> PrepareEmailAsync(EmailTemplate template, string variables)
    {
        var processedBody = await ProcessTemplateAsync(template.Body, variables);
        var processedSubject = await ProcessTemplateAsync(template.Subject, variables);

        return (processedSubject, processedBody);
    }
}