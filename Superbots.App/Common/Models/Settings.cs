namespace Superbots.App.Common.Models
{
    public class Settings : IEntity
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public bool Enabled { get; set; } = true;
        public ICollection<ApiKey>? ApiKeys { get; set; }
    }
}