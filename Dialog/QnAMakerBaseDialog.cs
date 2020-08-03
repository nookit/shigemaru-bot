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
using Microsoft.Extensions.Configuration;

namespace Uls.Shigemaru.Dialog
{
    public class QnAMakerBaseDialog : QnAMakerDialog
    {
        // TODO スマートにInjectionできないの？
        public string _defaultNoAnswer;
        public string _defaultCardTitle;
        public string _defaultCardNoMatchText;
        public string _defaultCardNoMatchResponse ;

        private readonly IBotServices _services;

        public QnAMakerBaseDialog(IConfiguration configuration,IBotServices services) : base()
        {
            this._services = services;
            _defaultNoAnswer = configuration.GetValue<string>("DefaultNoAnswer");
            _defaultCardTitle = configuration.GetValue<string>("DefaultCardTitle");
            _defaultCardNoMatchText = configuration.GetValue<string>("DefaultCardNoMatchText");
            _defaultCardNoMatchResponse = configuration.GetValue<string>("DefaultCardNoMatchResponse");
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
            noAnswer.Text = _defaultNoAnswer;

            var cardNoMatchResponse = (Activity)MessageFactory.Text(_defaultCardNoMatchResponse);


            var responseOptions = new QnADialogResponseOptions
            {
                ActiveLearningCardTitle = _defaultCardTitle,
                CardNoMatchText = _defaultCardNoMatchText,
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
