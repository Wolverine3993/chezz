namespace Chezz.SMTP
{
    public static class EmailFormatter
    {
        private static string ReadTemplate(string fileName) =>
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "SMTP", "Emails", fileName));

        public static string GetResetPasswordHTML(string link)
        {
            var html = ReadTemplate("ResetPassword.html");
            return html.Replace("--replace-link--", link);
        }
        public static string GetConfirmEmailHTML(string link)
        {
            var html = ReadTemplate("ConfirmEmail.html");
            return html.Replace("--replace-link--", link);
        }
    }
}
