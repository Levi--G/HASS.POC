using HADesktop.Services;

namespace HADesktop.Integrations
{
    public interface IIntegrationViewModel : IDisposable
    {
        public IIntegration? Integration { get; init; }
        public IConfigurationObject Configuration { get; init; }
        public bool IsDirty { get; }
        public bool IsLoading { get; }
        public bool IsSaving { get; }
        public bool IsConfigValid { get; }
        public void Activate();
        public Task Load();
        public Task Save();
        public Task Enable();
        public Task Disable();
    }
}
