namespace HADesktop.Integrations
{
    public interface IIntegrationRegistration
    {
        /// <summary>
        /// The name of the integration
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A unique short code
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// The description of the integration
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Reflects if the integration only supports a single instance.
        /// </summary>
        public bool SingleInstance { get; }

        /// <summary>
        /// Create a new Integration instance without configuration
        /// </summary>
        public IIntegration CreateInstance();

        /// <summary>
        /// Get the type of the configuration object for this integration
        /// </summary>
        public Type GetConfigType();

        /// <summary>
        /// Get the default configuration object for this integration
        /// </summary>
        public IConfigurationObject GetDefaultConfig();

        /// <summary>
        /// Get the view model for the integration
        /// </summary>
        public IIntegrationViewModel CreateViewModel(IIntegration integration);
    }
}
