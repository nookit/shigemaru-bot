using System;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Linq;

namespace Uls.Shigemaru.Dialog
{
    public class NotificationDialog : ComponentDialog
    {
        private static Dictionary<string, string> menus = new Dictionary<string, string>(){
            { "天気を確認", "foo" },
            { "予定を確認", "bar" }
        };

        private static IList<Choice> choices = ChoiceFactory.ToChoices(menus.Select(x => x.Key).ToList());

        public NotificationDialog()
            : base (nameof(NotificationDialog))
        {
            var waterfallStep = new WaterfallStep[]
            {
                showMenuAsync,
                messageAsync
            };

            AddDialog(new WaterfallDialog("menu", waterfallStep));
            AddDialog(new ChoicePrompt("choise"));
        }

        public static  async Task<DialogTurnResult> showMenuAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Console.WriteLine("===== ShowMenuAsync ====");

            return await stepContext.PromptAsync(
                "choise",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("今日は何をしますか？"),
                    Choices = choices

                },
                cancellationToken
                ); 
        }

        private async Task<DialogTurnResult> messageAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Console.WriteLine("===== messageAsync ====");

            var choised = (FoundChoice)stepContext.Result;

            Console.WriteLine("Notify choised : " + choices);

            await stepContext.Context.SendActivityAsync("今日の天気は晴れです : " + choised);
            return await stepContext.EndDialogAsync(true, cancellationToken);
        }

    }
}
