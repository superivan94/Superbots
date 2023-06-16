namespace Superbots.App.Common.Models
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly AppDbContext db;

        public AppSettingsService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<int> CreateSettings()
        {
            var settings = new Settings();
            db.Settings.First(s => s.Enabled == true).Enabled = false;
            db.Settings.Add(settings);
            await db.SaveChangesAsync();

            return settings.Id;
        }

        public async Task<Settings> LoadCurrentSettings()
        {
            return await Task.Run(() => db.Settings.First(s => s.Enabled == true));
        }

        public async Task<IEnumerable<Settings>> LoadSettings()
        {
            //TODO dove c'è take 100 inserire una costante...
            return await Task.Run(() => db.Settings.Take(100));
        }

        public async Task<bool> UpdateSettings(Settings settings)
        {
            //TODO controlli di validità
            db.Settings.Update(settings);
            await db.SaveChangesAsync();

            var success = await db.SaveChangesAsync() > 0;

            return success;
        }
    }
}
