namespace Superbots.App.Features.Chat.Models
{
    public class ChatServices : IChatServices
    {
        private const string MESSAGE_FORMAT_INVALID_ERROR = "Message properties does not respect constraints!";
        private const string CONVERSATION_FORMAT_INVALID_ERROR = "Conversation properties does not respect constraints!";

        private ChatDbContext db;

        public ChatServices(ChatDbContext db)
        {
            this.db = db;
        }

        public async Task<int> CreateConversation(Conversation conversation)
        {
            if (conversation is null)
            {
                conversation = new()
                {
                    Name = Conversation.DefaultName,
                    CreationDateTime = DateTime.Now
                };
            }
            else
            {
                conversation.Name ??= Conversation.DefaultName;
                conversation.CreationDateTime = conversation.CreationDateTime == DateTime.MinValue ? DateTime.Now : conversation.CreationDateTime;
            }

            await db.Conversations.AddAsync(conversation);
            await db.SaveChangesAsync();

            return conversation.Id;
        }

        public async Task<bool> UpdateConversation(Conversation conversation)
        {
            if (conversation is null
                || string.IsNullOrWhiteSpace(conversation.Name)
                || conversation.Id < 1) throw new Exception(CONVERSATION_FORMAT_INVALID_ERROR);

            db.Conversations.Update(conversation);
            var success = await db.SaveChangesAsync() > 0;

            return success;
        }

        public async Task<IEnumerable<Conversation>> LoadConversations()
        {
            //TODO dove c'è take 100 inserire una costante...
            return await Task.Run(() => db.Conversations.Take(100).OrderByDescending(c => c.CreationDateTime));
        }

        public async Task<Conversation> LoadConversation(Conversation conversation)
        {
            return await Task.Run(() =>
            {
                conversation.Messages = db.Messages.Where(m => m.ConversationId == conversation.Id).ToList();
                return conversation;
            });
        }

        public async Task<bool> CreateMessage(Message message)
        {
            if (message is null
                || string.IsNullOrWhiteSpace(message.Content)
                || string.IsNullOrWhiteSpace(message.Author)
                || message.ConversationId < 1) throw new Exception(MESSAGE_FORMAT_INVALID_ERROR);

            await db.Messages.AddAsync(message);
            var success = await db.SaveChangesAsync() > 0;

            return success;
        }
    }
}
