using System;
using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Uls.Shigemaru.Dialog;

namespace Uls.Shigemaru.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        protected readonly NotificationDialog dialog;

        private IServiceProvider serviceProvider;
//        private IStringLocalizer<NotificationController> localizer;
//        private ScheduleNotificationStore scheduleNotificationStore;
        private BotFrameworkAdapter adapter;
        //private BotConfiguration botConfig;
        private readonly string _appId;
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;
        protected readonly BotState conversationState;
        protected readonly UserState userState;
        private DialogSet dialogs;

        public NotificationController(
            IServiceProvider serviceProvider,
  //          IStringLocalizer<NotificationController> localizer,
//            ScheduleNotificationStore scheduleNotificationStore,
//            BotConfiguration botConfig,
            IConfiguration configuration,
            ConcurrentDictionary<string, ConversationReference> conversationReferences,
            NotificationDialog dialog,
            ConversationState conversationState,
            UserState userState
            )
        {
            this.serviceProvider = serviceProvider;
 //           this.localizer = localizer;
//            this.scheduleNotificationStore = scheduleNotificationStore;
//            this.botConfig = botConfig;
            this.adapter = (BotFrameworkAdapter)serviceProvider.GetService(typeof(IAdapterIntegration));
            this._conversationReferences = conversationReferences;
            this.dialog = dialog;
            this.conversationState = conversationState;
            this.userState = userState;

            _appId = configuration["MicrosoftAppId"];

            // If the channel is the Emulator, and authentication is not in use,
            // the AppId will be null.  We generate a random AppId for this case only.
            // This is not required for production, since the AppId will have a value.
            if (string.IsNullOrEmpty(_appId))
            {
                _appId = Guid.NewGuid().ToString(); //if no AppId, use a random Guid
            }

            var dialogState = conversationState.CreateProperty<DialogState>("DialogState");
            dialogs = new DialogSet(dialogState);

            dialogs.Add(new NotificationDialog());
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await SendProactiveMessage();

            return new ContentResult()
            {
                Content = "<html><body><h1>Proactive messages have been sent.</h1></body></html>",
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK,
            };
        }

        private async Task SendProactiveMessage()
        {
            foreach ( var conversationReference in _conversationReferences.Values) {
                await adapter.ContinueConversationAsync(
                    _appId,
                    conversationReference,
                    BotCallback,
                    default(CancellationToken)
                    );
            }
        }

        private async Task BotCallback(ITurnContext turnContext, CancellationToken cancellationToken)
        {
//            var dialogState = conversationState.CreateProperty<DialogState>("DialogState");
//            var dialogs = new DialogSet(dialogState);
            var dialogContext = await dialogs.CreateContextAsync(turnContext, cancellationToken);

            await turnContext.SendActivityAsync("fadffafa");

            await dialogContext.ReplaceDialogAsync(nameof(NotificationDialog), null, cancellationToken);

//            await dialog.RunAsync(
//                turnContext,
//                conversationState.CreateProperty<DialogState>(nameof(DialogState)),
//                cancellationToken);
        }
    }
}
