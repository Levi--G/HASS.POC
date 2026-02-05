namespace HASSPOC.Integrations
{
    internal interface IIntegrationViewModel
    {
        public IIntegration? Integration { get; init; }
        public IConfigurationObject? Configuration { get; init; }
        public bool IsDirty { get; }
        public void Activate();
        public void Revert();
        public void Save();
    }
}
