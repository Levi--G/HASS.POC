namespace HASSPOC.Integrations
{
    internal interface IIntegration
    {
        /// <summary>
        /// The name of the integration
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The description of the integration
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// The view containing the configuration elements
        /// </summary>
        public IIntegrationViewModel ConfigurationViewModel { get; }

        /// <summary>
        /// The configuration object of the integration
        /// </summary>
        public IConfigurationObject Configuration { get; }

        /// <summary>
        /// Load saved configuration from ConfigurationService if applicable
        /// </summary>
        public void Load();

        /// <summary>
        /// Save a new configuration to ConfigurationService
        /// </summary>
        public void Save(IConfigurationObject configuration);

        /// <summary>
        /// Activate the IIntegration and start updating/listening to Entities
        /// </summary>
        public void Activate();

        /// <summary>
        /// Deactivate the IIntegration and clean up
        /// </summary>
        public void Deactivate();
    }
}
