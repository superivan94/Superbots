namespace Superbots.App.Common.Models
{
    public interface IAppSettingsService
    {
        public Task<int> CreateSettings();
        public Task<IEnumerable<Settings>> LoadSettings();
        public Task<Settings?> LoadCurrentSettings();
        public Settings? LoadCurrentSettingsSync();
        public Task<bool> UpdateSettings(Settings settings);
    }
}
