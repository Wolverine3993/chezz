namespace Chezz.SMTP
{
    public static class EmailFormatter
    {
        public static string GetResetPasswordHTML(string link)
        {
            var html = File.ReadAllText(@"SMTP\Emails\ResetPassword.html");
            return html.Replace("--replace-link--", link);
        }
        public static string GetConfirmEmailHTML(string link)
        {
            var html = File.ReadAllText(@"SMTP\Emails\ConfirmEmail.html");
            return html.Replace("--replace-link--", link);
        }
    }
}
