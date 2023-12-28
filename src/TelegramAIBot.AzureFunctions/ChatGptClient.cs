using OpenAI.Chat;
using OpenAI.Models;
using OpenAI;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System;

namespace TelegramAIBot.AzureFunctions
{
    public static class ChatGptClient
    {
        private static readonly OpenAIClient ApiClient = new OpenAIClient(ConfigSettings.ChatGptKey);
        public static readonly Model Model3_5_Turbo_16K = Model.GPT3_5_Turbo_16K;
        public static readonly Model Model4_32K = Model.GPT4_32K;
        
        public async static Task<string> GetGptResponseModel(string message, Model model)
        {
            try
            {
                var messages = new List<Message>
                {
                    new Message(Role.System, message),
                };
                var chatRequest = new ChatRequest(messages, model);

                var response = await ApiClient.ChatEndpoint.GetCompletionAsync(chatRequest);

                var choice = response.FirstChoice;
                //choice. = ima vec opcij, bos pogledal kaj se da inline
                return response;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public async static Task<string> GetChatGptMockResult(int ms)
        {
            await Task.Delay(ms);
            return $"Delayed respond after {ms}ms";
        }

        /// <summary>
        /// ne dela!
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async static Task<string> GetGptResponseCustom(string message)
        {
            var apiKey = @"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";
            string apiUrl = "https://api.openai.com/v1/chat/completions";
            string model = "gpt-4";

            string requestBody = $"{{\"model\": \"{model}\", \"messages\": [{{\"role\": \"assistant\", \"content\": \"{message}\"}}], \"temperature\": 0.7, \"max_tokens\": 8,192}}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return response.ToString();
                }
            }
        }
    }
}
