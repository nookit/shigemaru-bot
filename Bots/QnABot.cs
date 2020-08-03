// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Builder.Azure;
using System.Linq;
using System;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class QnABot<T> : ActivityHandler where T : Microsoft.Bot.Builder.Dialogs.Dialog
    {
        protected readonly Microsoft.Bot.Builder.Dialogs.Dialog Dialog;
        protected readonly BotState ConversationState;
        protected readonly BotState UserState;

        // Dependency injected dictionary for storing ConversationReference objects used in NotifyController to proactively message users
        protected readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

        protected readonly IStorage _storage;

        public QnABot(ConversationState conversationState, UserState userState, T dialog, ConcurrentDictionary<string, ConversationReference> conversationReferences, IStorage storage)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
            _conversationReferences = conversationReferences;
            _storage = storage;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("=====  OnTurnAsync called =====");

            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            Console.WriteLine("=====  OnConversationUpdateActivityAsync called =====");
            Console.WriteLine(turnContext.Activity.TopicName);

            AddConversationReference(turnContext.Activity as Activity);

            return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMessageReactionActivityAsync(ITurnContext<IMessageReactionActivity> turnContext, CancellationToken cancellationToken)
        {
            Console.WriteLine("=====  OnMessageReactionActivityAsync called =====");

            Console.WriteLine(turnContext.Activity.From.Id);
            Console.WriteLine(turnContext.Activity.From.Name);
            Console.WriteLine(turnContext.Activity.Conversation.Name);
        }

        protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            Console.WriteLine("=====  OnEventActivityAsync called =====");
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Console.WriteLine("=====  OnMessageActivityAsync called =====");

            var utterance = turnContext.Activity.Text;
            UtteranceLog logItems = null;

            try
            {
                string[] utteranceList = { formatUtternceId(turnContext) };
                logItems = _storage.ReadAsync<UtteranceLog>(utteranceList).Result?.FirstOrDefault().Value;
            }
            catch
            {
                // TODO 異常系処理
                await turnContext.SendActivityAsync("Sorry, something went wrong reading your stored messages!");
            }

            // 発言ログが抽出できない場合は新規作成
            if (logItems is null)
            {
                logItems = new UtteranceLog();
                logItems.UtteranceList.Add(utterance);
                logItems.TurnNumber++;
                logItems.account = turnContext.Activity.Id;
                var changes = new Dictionary<string, object>();
                changes.Add(formatUtternceId(turnContext), logItems);
                try
                {
                    // ログを書き込み
                    await _storage.WriteAsync(changes, cancellationToken);
                }
                catch
                {
                    // TODO 異常系処理
                    await turnContext.SendActivityAsync("Sorry, something went wrong storing your message!");
                }
            }
            else
            {
                // 既存の発言ログを活用
                logItems.UtteranceList.Add(utterance);
                logItems.TurnNumber++;
                //                await turnContext.SendActivityAsync($"{logItems.TurnNumber}: The list is now: {string.Join(", ", logItems.UtteranceList)}");

                // Create Dictionary object to hold new list of messages.
                var changes = new Dictionary<string, object>();
                changes.Add(formatUtternceId(turnContext), logItems);

                try
                {
                    // ログを書き込み
                    await _storage.WriteAsync(changes, cancellationToken);
                }
                catch
                {
                    // TODO 異常系処理
                    await turnContext.SendActivityAsync("Sorry, something went wrong storing your message!");
                }
            }
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        private string formatUtternceId(ITurnContext<IMessageActivity> turnContext)
        {
            return String.Format("utteranceLog-{0:yyyyMMdd}-{1}-{2}.json",
                turnContext.Activity.LocalTimestamp,
                turnContext.Activity.From.Name,
                turnContext.Activity.ChannelId);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            Console.WriteLine("=====  OnMembersAddedAsync called =====");

            AddConversationReference(turnContext.Activity as Activity);

            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"こんにちわ、しげまるです！"), cancellationToken);
                }
            }
        }

        private void AddConversationReference(Activity activity)
        {
            var conversationReference = activity.GetConversationReference();
            _conversationReferences.AddOrUpdate(conversationReference.User.Id, conversationReference, (key, newValue) => conversationReference);
        }

        // Class for storing a log of utterances (text of messages) as a list.
        public class UtteranceLog : IStoreItem
        {
            // A list of things that users have said to the bot
            public List<string> UtteranceList { get; } = new List<string>();

            // The number of conversational turns that have occurred
            public int TurnNumber { get; set; } = 0;

            // Create concurrency control where this is used.
            public string ETag { get; set; } = "*";

            public string account { get; set; } = "-";
        }
    }
}
