using Microsoft.Bot.Builder.AI.QnA;

namespace Uls.Shigemaru
{
    public interface IBotServices
    {
        QnAMaker QnAMakerService { get; }
    }
}
