namespace Superbots.App.Common.Models
{
    public static class OpenAiModels
    {
        public const string GPT3_TURBO = "gpt-3.5-turbo";
    }

    public abstract class OpenAiDataTransferObjects { }

    public sealed class RequestOpenAiChatCompletion : OpenAiDataTransferObjects
    {
        public string Model { get; set; }
        public ICollection<RequestMessage> Messages { get; set; }

        public sealed class RequestMessage : OpenAiDataTransferObjects
        {
            public string Role { get; set; }
            public string Content { get; set; }
        }
    }

    public sealed class ResponseOpenAiChatCompletion : OpenAiDataTransferObjects
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public int Created { get; set; }
        public ICollection<Choice> Choices { get; set; }
        public TokenUsage Usage { get; set; }

        public sealed class Choice : OpenAiDataTransferObjects
        {
            public int Index { get; set; }
            public ResponseMessage Message { get; set; }
            public string Finish_Reason { get; set; }

            public sealed class ResponseMessage : OpenAiDataTransferObjects
            {
                public string Role { get; set; }
                public string Content { get; set; }
            }
        }

        public sealed class TokenUsage : OpenAiDataTransferObjects
        {
            public int Prompt_Tokens { get; set; }
            public int Completion_Tokens { get; set; }
            public int Total_Tokens { get; set; }
        }
    }
}