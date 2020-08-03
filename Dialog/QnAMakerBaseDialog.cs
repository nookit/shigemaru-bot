using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.AI.QnA.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Uls.Shigemaru.Dialog
{
    public class QnAMakerBaseDialog : QnAMakerDialog
    {
        public const string DefaultNoAnswer = "答えが見つかりませんでしたよー";
        public const string DefaultCardTitle = "Did you mean:";
        public const string DefaultCardNoMatchText = "None of the above.";
        public const string DefaultCardNoMatchResponse = "Thanks for the feedback.";

        private readonly IBotServices _services;

        public QnAMakerBaseDialog(IBotServices services) : base()
        {
            this._services = services;
        }

        protected async override Task<IQnAMakerClient> GetQnAMakerClientAsync(DialogContext dc)
        {
            Console.WriteLine("===== QnAMakerBaseDialog#GetQnAMakerClientAsync ====");
            viewDialogContext(dc);

            return this._services?.QnAMakerService;
        }

        protected override Task<QnAMakerOptions> GetQnAMakerOptionsAsync(DialogContext dc)
        {
            Console.WriteLine("===== QnAMakerBaseDialog#GetQnAMakerOptionsAsync ====");
            viewDialogContext(dc);

            return Task.FromResult(new QnAMakerOptions
            {
                ScoreThreshold = DefaultThreshold,
                Top = DefaultTopN,
                QnAId = 0,
                RankerType = "Default",
                IsTest = false
            });
        }

        protected async override Task<QnADialogResponseOptions> GetQnAResponseOptionsAsync(DialogContext dc)
        {
            Console.WriteLine("===== QnAMakerBaseDialog#GetQnAResponseOptionsAsync ====");
            viewDialogContext(dc);

            var noAnswer = (Activity)Activity.CreateMessageActivity();
            noAnswer.Text = DefaultNoAnswer;

            var cardNoMatchResponse = (Activity)MessageFactory.Text(DefaultCardNoMatchResponse);


            var responseOptions = new QnADialogResponseOptions
            {
                ActiveLearningCardTitle = DefaultCardTitle,
                CardNoMatchText = DefaultCardNoMatchText,
                NoAnswer = noAnswer,
                CardNoMatchResponse = cardNoMatchResponse,
            };

            return responseOptions;
        }

        protected void viewDialogContext(DialogContext dc)
        {


            Console.WriteLine("KnowledgeBaseId.ExpressionText : " + KnowledgeBaseId);
            Console.WriteLine("EndpointKey : " + EndpointKey);
            Console.WriteLine("NoAnswer    : " + NoAnswer);
            Console.WriteLine("Source.Path : " + Source);

            Console.WriteLine("dc.Context.Activity.Text : " + dc.Context.Activity.Text);
            Console.WriteLine("dc.Context.Activity.ReplyToId : " + dc.Context.Activity.ReplyToId);
            Console.WriteLine("dc.Context.Activity.ValueType : " + dc.Context.Activity.ValueType);
            Console.WriteLine("dc.Context.Activity.TopicName : " + dc.Context.Activity.TopicName);
            Console.WriteLine("dc.Context.Activity.TextFormat : " + dc.Context.Activity.TextFormat);
            Console.WriteLine("dc.Context.Activity.TextHighlights : " + dc.Context.Activity.TextHighlights);
            Console.WriteLine("dc.Context.Activity.Label : " + dc.Context.Activity.Label);
            Console.WriteLine("dc.Context.Activity.ListenFor : " + dc.Context.Activity.ListenFor);
            Console.WriteLine("dc.Stack.ToString : " + dc.Stack.ToString());
            Console.WriteLine("dc.Context.Activity.Value : " + dc.Context.Activity.Value);
            Console.WriteLine("dc.ActiveDialog.Id : " + dc.ActiveDialog.Id);
        }

    }
}
