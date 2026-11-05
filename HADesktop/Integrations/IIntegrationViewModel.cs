using HADesktop.Services;

namespace HADesktop.Integrations
{
    public interface IIntegrationViewModel : IDisposable
    {
        public IIntegration Integration { get; set; }
        public IConfigurationObject Configuration { get; }
        public bool IsDirty { get; }
        public bool IsLoading { get; }
        public bool IsSaving { get; }
        public bool IsConfigValid { get; }
        public void Activate();
        public Task Discard();
        public Task Apply();
        public Task Enable();
        public Task Disable();
    }
}
