namespace Chezz.SMTP
{
    public static class SMTPConfigurationExtension
    {
        public static IServiceCollection AddSmtpConfiguration(this IServiceCollection serviceCollection, IConfigurationSection SmtpConfigurationSection)
        {
            var smtpConfiguration = new SmtpConfiguration();
            SmtpConfigurationSection.Bind(smtpConfiguration);

            ArgumentNullException.ThrowIfNullOrWhiteSpace(smtpConfiguration.ClientAddress);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(smtpConfiguration.ClientID);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(smtpConfiguration.ClientSecret);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(smtpConfiguration.RefreshToken);

            serviceCollection.AddSingleton<SmtpConfiguration>(smtpConfiguration);
            return serviceCollection;
        }
    }
}
