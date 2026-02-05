namespace HASSPOC.Integrations.Mouse;

internal class MouseConfiguration : IConfigurationObject
{
    /// <summary>
    /// Update interval of the mouse position in seconds
    /// </summary>
    public int UpdateIntervals { get; set; } = 30;

    public bool Enabled { get; set; }
}
