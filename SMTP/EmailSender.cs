using Microsoft.AspNetCore.Identity.UI.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using Chezz.SMTP;

namespace Chezz.Services
{
    public class EmailSender(ILogger<EmailSender> _logger, SmtpConfiguration _configuration) : IEmailSender
    {
        TokenResponse? _credentials;

        async Task RefreshAccessToken()
        {
            if (_credentials is null || _credentials!.IsStale)
            {
                var clientSecrets = new ClientSecrets
                {
                    ClientId = _configuration.ClientID,
                    ClientSecret = _configuration.ClientSecret
                };

                var credential = new UserCredential(
                    new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = clientSecrets,
                        Scopes = new[] { "https://mail.google.com/" }
                    }),
                    "user",
                    new TokenResponse { RefreshToken = _configuration.RefreshToken });

                if (await credential.RefreshTokenAsync(CancellationToken.None))
                {
                    _credentials = credential.Token;
                    _logger.LogInformation("SMTP client authentication success");
                    return;
                }
                _logger.LogError("SMTP client authentication failure");
            }
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await RefreshAccessToken();
            if (_credentials is null)
            {
                return;
            }

            var mimeEmail = new MimeMessage();
            mimeEmail.From.Add(new MailboxAddress("Chezz", _configuration.ClientAddress));
            mimeEmail.To.Add(new MailboxAddress(email, email));

            mimeEmail.Subject = subject;
            mimeEmail.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);

                await client.AuthenticateAsync(new SaslMechanismOAuth2(_configuration.ClientAddress, _credentials.AccessToken));
                await client.SendAsync(mimeEmail);
                _logger.LogInformation("Successfully sent an email to {0}", email);

                await client.DisconnectAsync(true);
            }
        }
    }
}
