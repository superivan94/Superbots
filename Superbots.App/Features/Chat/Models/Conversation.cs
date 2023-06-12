using Superbots.App.Common.Models;
using Superbots.App.Features.Chat.Models;

public class Conversation : IEntity
{
    public static class Error
    {
        public const string INVALID_ID = "Conversation ID is invalid!";
    }

    public static string DefaultName
    {
        get
        {
            return $"Conversation of {DateTime.Now}";
        }
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreationDateTime { get; set; } = DateTime.MinValue;
    public ICollection<Message>? Messages { get; set; }
}