using KIU.LMS.Domain.Common.Settings.Gemini;
using System.Net.Http;

namespace KIU.LMS.Application.Features.Prompts.Queries;

public sealed record GetAntropicBalanceQuery() : IRequest<Result<string>>;

public sealed record GetAntropicBalanceQueryHandler(ClaudeSettings settings) : IRequestHandler<GetAntropicBalanceQuery, Result<string>>
{
    public async Task<Result<string>> Handle(GetAntropicBalanceQuery request, CancellationToken cancellationToken)
    {
		try
		{
            string apiUrl = $"https://console.anthropic.com/api/organizations/a5bddf41-90f7-4d5e-9ca1-8e8835fc466d/prepaid/credits";

            using (var httpClient = new HttpClient())
            {
                var req = new HttpRequestMessage(HttpMethod.Get, apiUrl);

                // ჰედერების დამატება დოკუმენტიდან
                req.Headers.Add("accept", "*/*");
                req.Headers.Add("accept-language", "en-US,en;q=0.9");
                req.Headers.Add("anthropic-anonymous-id", "0789c314-173c-4ea8-aaab-d3bf588c3b46");
                req.Headers.Add("anthropic-client-platform", "web_console");
                req.Headers.Add("anthropic-client-sha", "8063bc6c4b73461145edefecb5ca403b50dc07d7");
                req.Headers.Add("anthropic-client-version", "1");
                //req.Headers.Add("content-type", "application/json");

                // ქუქის დამატება სადაც შედის sessionKey
                req.Headers.Add("Cookie", $"sessionKey=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");

                // მოთხოვნის გაგზავნა
                var response = await httpClient.SendAsync(req);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"API აბრუნებს შეცდომის კოდს: {(int)response.StatusCode}. მიზეზი: {response.ReasonPhrase}");
                }

                // შედეგის დამუშავება
                var content = await response.Content.ReadAsStringAsync();

                return Result<string>.Success(content);
            }
        }
		catch (Exception ex)
		{

			throw;
		}
    }
}