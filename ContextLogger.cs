using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Bot.Builder;
using System;

namespace Uls.Shigemaru
{
    public class ContextLogger : IMiddleware
    {
        public ContextLogger()
        {
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            Console.WriteLine("turnContext.Activity.Id : " + turnContext.Activity.Id);
            Console.WriteLine("turnContext.Activity.MembersAdded : " + turnContext.Activity.MembersAdded);
            Console.WriteLine("turnContext.Activity.MembersRemoved : " + turnContext.Activity.MembersRemoved);
            Console.WriteLine("turnContext.Activity.Recipient : " + turnContext.Activity.Recipient);
            Console.WriteLine("turnContext.Activity.ReplyToId : " + turnContext.Activity.ReplyToId);
            Console.WriteLine("turnContext.Activity.ServiceUrl : " + turnContext.Activity.ServiceUrl);
            Console.WriteLine("turnContext.Activity.Text: " + turnContext.Activity.Text);
            Console.WriteLine("turnContext.Activity.TopicName  : " + turnContext.Activity.TopicName);
            Console.WriteLine("turnContext.Activity.ValueType  : " + turnContext.Activity.ValueType);
            Console.WriteLine("turnContext.Activity.TopicName : " + turnContext.Activity.TopicName);
            Console.WriteLine("turnContext.Activity.ChannelId : " + turnContext.Activity.ChannelId);
            Console.WriteLine("turnContext.Activity.From.Id : " + turnContext.Activity.From.Id);
            Console.WriteLine("turnContext.Activity.From.Name : " + turnContext.Activity.From.Name);
            Console.WriteLine("turnContext.Activity.From.Role : " + turnContext.Activity.From.Role);
            Console.WriteLine("turnContext.Activity.Conversation.Id : " + turnContext.Activity.Conversation.Id);
            Console.WriteLine("turnContext.Activity.Conversation.Name : " + turnContext.Activity.Conversation.Name);
            Console.WriteLine("turnContext.Activity.Conversation.Role : " + turnContext.Activity.Conversation.Role);
            Console.WriteLine("turnContext.Activity.Conversation.TenantId : " + turnContext.Activity.Conversation.TenantId);


            Debug.WriteLine($"{turnContext.Activity.From}:{turnContext.Activity.Type}");
            Debug.WriteLineIf(
                !string.IsNullOrEmpty(turnContext.Activity.Text),
                turnContext.Activity.Text);

//            string file_path = System.IO.Path.Combine("/var/logs/test.txt");
            // ファイルへテキストデータを書き込む
  //          string file_data = $"{turnContext.Activity.From}:{turnContext.Activity.Type}";    // ファイルのデータ
  //          using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file_path))   // UTF-8のテキスト用
                                                                                        //using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file_path, Encoding.GetEncoding("shift-jis")))  // シフトJISのテキスト用
  //          {
  //              sw.Write(file_data); // ファイルへテキストデータを出力する
  //          }

            await next.Invoke(cancellationToken);
        }
    }
}
