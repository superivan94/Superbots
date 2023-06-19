using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using Superbots.App.Common.Models.OpenAi;
using Superbots.App.Features.Chat.Models;
using System.Diagnostics.CodeAnalysis;

namespace Superbots.App.Features.Chat
{
    public partial class ChatPage
    {
        public class EditableFields
        {
            public string ConversationName { get; set; } = string.Empty;
            public string UserMessage { get; set; } = string.Empty;
        }

        public enum StateValues
        {
            Readonly,
            EditingConversationName
        }

        [Inject][AllowNull] private ILogger<ChatPage> Logger { get; set; }
        [Inject][AllowNull] private IJSRuntime JSRuntime { get; set; }
        [Inject][AllowNull] private IChatService ChatServices { get; set; }
        [Inject][AllowNull] private IOpenAiConnector<OpenAiConnector> OpenAi { get; set; }

        private bool UserCanSendMessage => AiComposingMessage is false;
        private bool AiComposingMessage { get; set; }
        private bool LoadingMessages { get; set; }
        private bool AllMessagesAreLoaded { get; set; }

        private Dictionary<int, Conversation> Conversations { get; set; } = new();
        private Conversation? ConversationSelected { get; set; }
        private const int MESSAGES_TO_SKIP_DEFAULT = 5;
        private const int MESSAGES_TO_TAKE_LAST_DEFAULT = 5;
        private const int MESSAGES_MAX_TO_RENDER = 25;
        private int MessagesToSkip { get; set; }
        private int MessagesToTakeLast { get; set; }
        //TODO max differenza tra recente e outdated in modo da visualizzare per esempio max 100 messaggi? Performance

        public StateValues State { get; set; } = StateValues.Readonly;
        public EditableFields Fields { get; set; } = new();

        //TODO aggiungere controllo di caricare e tenere in memoria solo i messaggi della conversazione attiva, parzialmente implementato
        //TODO inserire delle costanti per definire autore dei messaggi

        private ElementReference LastMessage { get; set; }
        private MudTextField<string>? UserMessageTextField { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadConversations();
        }

        private async Task CreateConversation()
        {
            var conversation = new Conversation()
            {
                Name = $"Doppio click per rinominare #{Random.Shared.Next(100)}",
                CreationDateTime = DateTime.Now
            };
            var holdNewConversationId = await ChatServices!.CreateConversation(conversation);
            await LoadConversations();
            ConversationSelected = Conversations[holdNewConversationId];
        }

        private async Task LoadConversations()
        {
            var result = await ChatServices!.LoadConversations();
            Conversations = result.ToDictionary(c => c.Id);
        }

        private async Task LoadConversationMessages(Conversation conversationToLoad)
        {
            if (State == StateValues.EditingConversationName) return;

            LoadingMessages = true;
            MessagesToTakeLast = 0;
            AllMessagesAreLoaded = false;
            ConversationSelected = conversationToLoad;

            var conversationLoaded = await ChatServices!.LoadMessagesConversation(conversationToLoad);
            Conversations[conversationLoaded.Id] = conversationLoaded;
            await RenderMoreMessages();

            await LastMessage.FocusAsync();
        }

        private async Task RenderMoreMessages()
        {
            LoadingMessages = true;
            await Task.Delay(1000);

            MessagesToTakeLast += MESSAGES_TO_TAKE_LAST_DEFAULT;
            var msgRange = ConversationSelected!.Messages?.Skip(MessagesToSkip).TakeLast(MessagesToTakeLast) ?? Enumerable.Empty<Message>();
            var msgCount = msgRange.Count();
            if (MessagesToTakeLast > msgCount)
            {
                MessagesToTakeLast = msgCount;
                AllMessagesAreLoaded = true;
            }

            LoadingMessages = false;
        }

        private async Task UserSendMessage()
        {
            if (ConversationSelected is null || ConversationSelected!.Id < 0) throw new NullReferenceException(Conversation.Error.INVALID_ID);

            var message = new Message()
            {
                Author = "User",
                Content = Fields.UserMessage,
                ConversationId = ConversationSelected.Id,
                CreationDateTime = DateTime.Now,
            };

            if (await ChatServices!.CreateMessage(message)) Fields.UserMessage = string.Empty;
            await UserMessageTextField!.BlurAsync();

            MessagesToTakeLast = Math.Clamp(++MessagesToTakeLast, MESSAGES_TO_TAKE_LAST_DEFAULT, MESSAGES_MAX_TO_RENDER);
            await LastMessage.FocusAsync();

            await ChatBotGenerateResponse();
        }

        private async Task UserSendMessage(KeyboardEventArgs eventKeyboard)
        {
            if (eventKeyboard.Key != "Enter") return;

            await UserSendMessage();
        }

        private async Task ChatBotGenerateResponse()
        {
            AiComposingMessage = true;
            StateHasChanged();

            if (ConversationSelected is null) throw new NullReferenceException();
            var messages = ConversationSelected.Messages?.Select(m => new RequestOpenAiChatCompletion.RequestMessage()
            {
                Role = m.Author == MessageAuthors.USER ? OpenAiRoles.USER : OpenAiRoles.ASSISTANT,
                Content = m.Content
            });
            var response = await OpenAi!.ChatCompletion(messages?.ToList());

            Logger.LogDebug($"ChatBotGenerate count {response?.Choices.Count}");
            foreach (var item in response.Choices)
            {
                Logger.LogDebug($"Message {item.Message}");
            }

            await ChatServices!.CreateMessage(new() { Author = "Chatbot", ConversationId = ConversationSelected.Id, CreationDateTime = DateTime.Now, Content = response.Choices.First().Message.Content });

            MessagesToTakeLast = Math.Clamp(++MessagesToTakeLast, MESSAGES_TO_TAKE_LAST_DEFAULT, MESSAGES_MAX_TO_RENDER);
            await LastMessage.FocusAsync();

            AiComposingMessage = false;
        }

        private void ToggleEditConversationName() => State = State == StateValues.Readonly ? StateValues.EditingConversationName : StateValues.Readonly;

        private bool UserWantChangeThisConversationName(Conversation conversation) => State == StateValues.EditingConversationName && conversation.Id == ConversationSelected!.Id;

        private async Task TryToRenameConversation(Conversation conversation)
        {
            /*
             * Questo controllo esiste in quando il componente a cui sono legati nel momento in cui
             * l'utente preme invio per cambiare il nome alla conversazione fa scatenare 2 eventi invece che 1.
             * Prima viene scatenato l'evento alla pressione del tasto e poi il componente perdendo il focus di conseguenza
             * scatena l'evento al focusout. Quindi se la variabile di appoggio Fields.ConversationName è vuota possiamo
             * dedurre che è stato premuto il tasto Enter e quindi possiamo ignorare la chiamata che arriverà 
             * dall'evento focusout.
             */
            if (string.IsNullOrWhiteSpace(Fields.ConversationName)) return;

            if (conversation.Name != Fields.ConversationName)
            {
                conversation.Name = Fields.ConversationName;
                var success = await ChatServices!.UpdateConversation(conversation);
                Fields.ConversationName = string.Empty;
                StateHasChanged();
            }
            ToggleEditConversationName();
        }

        private async Task TryToRenameConversation(KeyboardEventArgs keyboardEvent, Conversation conversation)
        {
            if (keyboardEvent.Key == "Enter") await TryToRenameConversation(conversation);
        }
    }
}
