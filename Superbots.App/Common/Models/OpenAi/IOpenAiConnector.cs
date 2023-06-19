namespace Superbots.App.Common.Models.OpenAi
{
    public interface IOpenAiConnector<AiConnector>
    {
        protected abstract HttpClient HttpClient { get; }
        public Task<ResponseOpenAiChatCompletion?> ChatCompletion(ICollection<RequestOpenAiChatCompletion.RequestMessage>? messages);
    }
}
