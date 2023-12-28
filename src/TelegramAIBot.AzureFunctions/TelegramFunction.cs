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
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System.Xml.Linq;
using Telegram.Bot.Requests;
using System.Collections.Generic;

[assembly: FunctionsStartup(typeof(TelegramAIBot.AzureFunctions.Startup))]

namespace TelegramAIBot.AzureFunctions
{
    public static class TelegramFunction
    {

        private static readonly TelegramBotClient Bot = new TelegramBotClient(ConfigSettings.TelegramBotKey);
        private static string TempPath = Path.GetTempPath();

        [FunctionName("TelegramFunction")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var update = JsonConvert.DeserializeObject<Telegram.Bot.Types.Update>(requestBody);

            if(update.CallbackQuery != null)
            {
                //kadar user interact-a z inline stvarmi
            }
            if(update.Message != null)
            {
                //inline first
                //await Bot.SendTextMessageAsync(update.Message.Chat.Id, "asfas");
            }

            if (update.InlineQuery != null)
            {
               if (update.InlineQuery.Query.Count() > 3)
               {
                    var inlineQuery = update.InlineQuery;

                    try
                    {
                        await Bot.AnswerInlineQueryAsync(inlineQuery.Id, await GetInlineQueryResult(update), isPersonal: true, cacheTime: 0, nextOffset: null);
                    }
                    catch(Exception ex)
                    {
                         
                    }
                    finally
                    {

                    }

                    #region SecondCall
                    //try
                    //{
                    //    var realResponse = new InlineQueryResult[]
                    //    {
                    //        new InlineQueryResultArticle
                    //        (
                    //            id: "2",
                    //            title: $"fasdsda",
                    //            inputMessageContent: new InputTextMessageContent("test") //tekst ki ga posljes v chat
                    //        ),
                    //    };

                    //    await Bot.AnswerInlineQueryAsync(inlineQuery.Id, realResponse, isPersonal: true, cacheTime: 0, nextOffset: null);
                    //}
                    //catch (Exception ex) 
                    //{

                    //}
                    //finally
                    //{

                    //}
                    #endregion SecondCall
                }
            }
            
             return new OkResult();
        }

        private static async Task<InlineQueryResult[]> GetInlineQueryResult(Update update)
        {
            var result = await ChatGptClient.GetGptResponseModel(update.InlineQuery.Query, ChatGptClient.Model3_5_Turbo_16K);

            InlineQueryResult[] results =
            {
                new InlineQueryResultArticle(
                    id: "article:bot-api",
                    title: update.InlineQuery.Query,
                    inputMessageContent: new InputTextMessageContent(result))
                {
                    Description = result,
                },
            };

            return results;
        }

        private async static Task SendResult(Update update, InlineQueryResult[] result)
        {
            await Bot.AnswerInlineQueryAsync(update.InlineQuery.Id, result, isPersonal: true, cacheTime: 0);
        }
    }
}
