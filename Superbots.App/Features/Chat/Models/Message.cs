using Superbots.App.Common.Models;

namespace Superbots.App.Features.Chat.Models
{
    public class Message : IEntity
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreationDateTime { get; set; } = DateTime.MinValue;
    }
}
