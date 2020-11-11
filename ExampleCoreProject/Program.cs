using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace Example
{
    internal class Program
    {
        private static IConfiguration Configuration { get; set; }

        private static async Task Main()
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
            Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile($"appsettings.{env}.json", optional: true)
                    .Build();
            var services = ConfigureServices(new ServiceCollection()).BuildServiceProvider();
            var client = services.GetRequiredService<ISendGridClient>();
            var from = new EmailAddress(Configuration.GetValue("SendGrid:From", "sts@vcs.com.pk"), "STS User");
            var to = new EmailAddress(Configuration.GetValue("SendGrid:To", "tahiralvi@vcs.com.pk"), "Tahir User");
            var msg = new SendGridMessage
            {
                From = from,
                Subject = "Sending a Test Email for STS"
            };
            msg.AddContent(MimeType.Text, "and easy to do anywhere, even with C#");
            msg.AddTo(to);
            if (Configuration.GetValue("SendGrid:SandboxMode", false))
            {
                msg.MailSettings = new MailSettings
                {
                    SandboxMode = new SandboxMode
                    {
                        Enable = true
                    }
                };
            }
            Console.WriteLine($"Sending email with payload: \n{msg.Serialize()}");
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine(response.Headers);
        }

        private static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // services.AddSendGrid(options => { options.ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ?? Configuration["SendGrid:ApiKey"]; });
            services.AddSendGrid(options => { options.ApiKey = "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"; });

            return services;
        }
    }
}