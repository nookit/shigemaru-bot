using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.AI.QnA.Dialogs;
using Microsoft.Bot.Builder.Dialogs;

namespace Uls.Shigemaru.Dialog
{
    public class RootDialog : ComponentDialog
    {
        private const string InitialDialog = "initial-dialog";

        public RootDialog(IBotServices services)
            : base("root")
        {
            AddDialog(new QnAMakerBaseDialog(services));

            AddDialog(new WaterfallDialog(InitialDialog)
               .AddStep(InitialStepAsync));

            // The initial child Dialog to run.
            InitialDialogId = InitialDialog;
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(QnAMakerDialog), null, cancellationToken);
        }
    }
}
