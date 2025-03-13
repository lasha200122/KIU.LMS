//using DocumentFormat.OpenXml.Wordprocessing;
//using Microsoft.Graph;
//using Microsoft.Graph.Models;
//using Microsoft.Identity.Client;
//using BodyType = Microsoft.Graph.Models.BodyType;

//namespace KIU.LMS.Infrastructure.Services;

//public class OauthEmailSenderService(OAuthSettings _settings) : IOauthEmailSenderService
//{
//    public async Task SendEmailAsync(string to, string subject, string body)
//    {
//        try
//        {
//            // ავთენტიკაციის ტოკენის მიღება Microsoft Identity-სგან
//            var confidentialClientApplication = ConfidentialClientApplicationBuilder
//                .Create(_settings.ClientId)
//                .WithTenantId(_settings.TenantId)
//                .WithClientSecret(_settings.ClientSecret)
//                .Build();

//            // OAuth სკოუპების განსაზღვრა (Microsoft Graph-ისთვის)
//            var authResult = await confidentialClientApplication
//                .AcquireTokenForClient(_settings.Scopes)
//                .ExecuteAsync();

//            // Graph კლიენტის შექმნა
//            var graphClient = new GraphServiceClient(
//                new DelegateAuthenticationProvider(requestMessage =>
//                {
//                    requestMessage.Headers.Authorization =
//                        new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
//                    return Task.CompletedTask;
//                }));

//            var email = new Microsoft.Graph.Message
//            {
//                Subject = subject,
//                Body = new ItemBody
//                {
//                    ContentType = BodyType.Html,
//                    Content = body
//                },
//                ToRecipients = new List<Recipient>
//                {
//                    new Recipient
//                    {
//                        EmailAddress = new EmailAddress
//                        {
//                            Address = to
//                        }
//                    }
//                }
//            };

//            await graphClient.Users[_settings.FromEmail]
//                .SendMail(email, true)
//                .Request()
//                .PostAsync();
//        }
//        catch (Exception ex)
//        {
//            throw;
//        }
//    }
//}
