namespace Superbots.App.Features.Chat.Models
{
    public interface IChatServices//TODO rivalutare i nomi, tipo readMessage in realtà restituisce una lista... il nome è ambiguo
    {
        /// <summary>
        /// Crea una nuova conversazione
        /// </summary>
        /// <param name="conversation"></param>
        public Task<int> CreateConversation(Conversation conversation);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="conversation"></param>
        /// <returns></returns>
        public Task<bool> UpdateConversation(Conversation conversation);

        /// <summary>
        /// Restituisce tutte le conversazioni senza i messaggi associati.
        /// </summary>
        /// <returns>Restituisce tutte le conversazioni</returns>
        public Task<IEnumerable<Conversation>> LoadConversations();

        /// <summary>
        /// Carica i messaggi di una conversazione tramite Id della conversazione
        /// </summary>
        /// <param name="conversation">Con Id della conversazione come riferimento è possibile caricare i messaggi</param>
        /// <returns>Restituisce la conversazione con la lista dei messaggi valorizzata</returns>
        public Task<Conversation> LoadConversation(Conversation conversation);

        /// <summary>
        /// Crea un nuovo messaggio di chat, solitamente al suo interno è contenuta
        /// l'informazione a queale conversazione appartiene.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<bool> CreateMessage(Message message);
    }
}
