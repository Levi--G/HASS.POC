namespace HADesktop.Integrations.Mouse
{
    internal class MouseIntegrationRegistration : IntegrationRegistrationBase<MouseIntegrationRegistration, MouseIntegration, MouseConfiguration, MouseConfigurationViewModel, MouseConfigurationView>
    {
        public override string Code => "MOU";

        public override bool SingleInstance => true;
        public static void Register()
        {
            //example only supported on windows
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                RegisterDefaults();
            }
        }
    }
}