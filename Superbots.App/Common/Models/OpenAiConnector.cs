using System.Net.Http.Headers;

namespace Superbots.App.Common.Models
{
    public class OpenAiConnector : IOpenAiConnector<OpenAiConnector>
    {
        private IConfiguration Configuration { get; }
        private IAppSettingsService AppSettingsService { get; }
        public HttpClient HttpClient => _httpClient;
        private readonly HttpClient _httpClient;

        public OpenAiConnector(IConfiguration configuration, IAppSettingsService appSettingsService)
        {
            Configuration = configuration;
            AppSettingsService = appSettingsService;

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new(Configuration["Api:BaseUrl:OpenAi"]!);

            //TODO migliorare organizzare
            var key = AppSettingsService.LoadCurrentSettings().Result.ApiKeys.First().Key;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        ///     * curl https://api.openai.com/v1/chat/completions \
        ///      -H "Content-Type: application/json" \
        ///      -H "Authorization: Bearer $OPENAI_API_KEY" \
        ///      -d '{
        ///        "model": "gpt-3.5-turbo",
        ///        "messages": [{"role": "user", "content": "Hello!"}]
        ///     }
        /// </summary>
        /// <param name="messages">Una lista di messaggi con ruolo e contenuto</param>
        /// <returns></returns>
        public async Task<ResponseOpenAiChatCompletion?> ChatCompletion(ICollection<RequestOpenAiChatCompletion.RequestMessage>? messages)//TODO DARE MODELLO TRAMITE ENUM
        {
            //TODO SWITCH CASE PER SCEGLIERE MODELLO

            var request = new RequestOpenAiChatCompletion()
            {
                Model = OpenAiModels.GPT3_TURBO_UPDATED,
                Messages = messages ?? new List<RequestOpenAiChatCompletion.RequestMessage>()
            };

            var response = await HttpClient.PostAsJsonAsync("chat/completions", request);
            var content = await response.Content.ReadFromJsonAsync<ResponseOpenAiChatCompletion>();

            return content;
        }
    }
}
