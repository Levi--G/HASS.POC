using HADesktop.Services;

namespace HADesktop.Integrations
{
    public interface IIntegration
    {
        /// <summary>
        /// Reflects if the integration is currently enabled in cofiguration
        /// </summary>
        public bool Enabled { get; }

        /// <summary>
        /// The index of the integration and its configuration
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The view containing the configuration elements
        /// </summary>
        public IIntegrationViewModel ConfigurationViewModel { get; }

        /// <summary>
        /// The configuration object of the integration
        /// </summary>
        public IConfigurationObject Configuration { get; set; }

        /// <summary>
        /// Activate the IIntegration and start updating/listening to Entities
        /// </summary>
        public void Activate();

        /// <summary>
        /// Deactivate the IIntegration and clean up
        /// </summary>
        public void Deactivate();

        /// <summary>
        /// Reactivate the IIntegration if it is activated
        /// </summary>
        public void Reactivate();
        public Task Enable();
        public Task Disable();
    }
}
