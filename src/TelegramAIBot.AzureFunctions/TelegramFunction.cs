using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using System.Linq;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types;

namespace TelegramAIBot.AzureFunctions
{
    public static class TelegramFunction
    {
        private static readonly TelegramBotClient Bot = new TelegramBotClient("6777999160:AAFzzBwtum4-tv_ZqOVBKobl_pkTpwwfBZ0");
        private static string TempPath = Path.GetTempPath();

        [FunctionName("TelegramFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var update = JsonConvert.DeserializeObject<Telegram.Bot.Types.Update>(requestBody);

             if(update.Message != null)
             {
                //inline first
             }

             if (update.InlineQuery != null)
             {
                if (update.InlineQuery.Query.Count() > 3)
                {
                    SendResult(update, GetLoadingInlineQueryArticle(update)); //initial loading response

                    //koda se ne izvede
                    SendResult(update,GetInlineQueryInlineArticle(update));

                }
             }
            
             return new OkResult();
        }

        private static InlineQueryResult[] GetLoadingInlineQueryArticle(Update update)
        {
            var updatedQueryResult = new InlineQueryResult[]
            {
                new InlineQueryResultArticle(
                    id: "1",
                    title: $"Loading...",
                    inputMessageContent: new InputTextMessageContent("test") //tekst ki ga posljes v chat
                ),
            };

            return updatedQueryResult;
        }

        private static InlineQueryResult[] GetInlineQueryInlineArticle(Update update)
        {
            var result = ChatGptClient.GetChatGptMockResult(1000);

            var updatedQueryResult = new InlineQueryResult[]
            {
                new InlineQueryResultArticle(
                    id: "1",
                    title: $"{result}",
                    inputMessageContent: new InputTextMessageContent("result") 
                ),
            };

            return updatedQueryResult;
        }

        private async static void SendResult(Update update, InlineQueryResult[] result)
        {
            await Bot.AnswerInlineQueryAsync(update.InlineQuery.Id, result, isPersonal: true, cacheTime: 0);
        }
    }
}
