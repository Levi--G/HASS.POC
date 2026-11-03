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

        public Type GetConfigType();
    }
}
