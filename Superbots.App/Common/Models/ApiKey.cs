using Superbots.App.Common.Models;

public class ApiKey : IEntity
{
    public int Id { get; set; }
    public int SettingsId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;//TODO criptare per sicurezza (non in chiaro)
}