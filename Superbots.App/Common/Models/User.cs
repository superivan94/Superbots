namespace Superbots.App.Common.Models
{
    public class User : IEntity
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;//TODO criptare
        public byte[]? Userimage { get; set; }
    }
}
