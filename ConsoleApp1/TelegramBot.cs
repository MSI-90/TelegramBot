using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp1
{
    //[Serializable]
    static class TelegramBot
    {
        static string token = "5331687709:AAEXvmiQ1qObjihZeUh85cs903W7s4wKJok";
        static long lastUpdates = 0;
        static readonly HttpClient client = new HttpClient();
        static long lastUpdateId { get; set; }
        static int lastMessageId { get; set; }
        public static string lastMessageText { get; set; }
        static int lastChatId { get; set; }
        static string ?Responce { get; set; }
        static long ?BotId { get; set; }
        static string ?FirstNameOfBot { get; set; }
        static string ?UserNameOfBot { get; set; }
        static bool CanJoinGroups { get; set; }
        static bool CanReadAllGroupMessages { get; set; }
        static bool SupportsInlineQueries { get; set; }
        public static async Task getMeAsync()
        {
            try
            {
                using (HttpResponseMessage response = await client.GetAsync(@"https://api.telegram.org/bot" + token + "/getMe"))
                {
                    Responce = await response.Content.ReadAsStringAsync();
                }
                //response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException re)
            {
                Console.WriteLine($"Неудалось получить ответ от сервера, необходимо проверить идентификатор токена и способ соединения.\n{re.Message}");
            }
            if (Responce != null)
            {   
                var outputFromJson = JSON.Parse(Responce);
                BotId = Convert.ToInt64(outputFromJson["result"]["id"].Value);
                FirstNameOfBot = outputFromJson["result"]["first_name"].Value;
                UserNameOfBot = outputFromJson["result"]["username"].Value;
                CanJoinGroups = Convert.ToBoolean(outputFromJson["result"]["can_join_groups"].Value);
                CanReadAllGroupMessages = Convert.ToBoolean(outputFromJson["result"]["can_join_groups"].Value);
                SupportsInlineQueries = Convert.ToBoolean(outputFromJson["result"]["supports_inline_queries"].Value);

               
            }
            else
            {
                Console.WriteLine("Невозможно обработать полученные данные от сервера Telegram");
            }
        }
        public static async Task GetUpdates()
        {   
            Console.WriteLine($"Запрос на обновление информации в боте: {lastUpdateId + 1}");
            try
            {
                using (HttpResponseMessage message = await client.GetAsync($@"https://api.telegram.org/bot" + token + "/getUpdates" + "?offset=" + (lastUpdateId + 1)))
                {
                    Responce = await message.Content.ReadAsStringAsync();

                    if (Responce != null)
                    {   
                        var outputFromJson = JSON.Parse(Responce);

                        List<string> messageIdInto = new List<string>();
                        List<string> messageTextInfo = new List<string>();

                        foreach (JSONNode node in outputFromJson["result"].AsArray)
                        {   
                            if (node.Value != null)
                            {
                                lastUpdateId = node["update_id"].AsLong;
                                lastUpdates = node["update_id"].AsInt;
                                lastChatId = node["message"]["chat"]["id"].AsInt;
                                
                                messageIdInto.Add(node["message"]["message_id"]);
                                messageTextInfo.Add(node["message"]["text"]);
                                lastMessageId = Convert.ToInt32(messageIdInto[messageIdInto.Count - 1]);
                                lastMessageText = messageTextInfo[messageTextInfo.Count - 1];
                            }

                            if (lastMessageText == "/start")
                            {
                                //SendMessage("Приветствуем Вас в нашем чате", lastChatId);
                                SendMessageWithReply("Приветствуем Вас в нашем чате", lastMessageId, lastChatId);
                            }

                            if (lastMessageText.ToLower().Contains("здаров") || lastMessageText.ToLower().Contains("здоров"))
                            {
                                //SendMessage("Здоровее видали!", lastChatId);
                                SendMessageWithReply("Здоровее видали!", lastMessageId, lastChatId);
                            }
                            else
                            {
                                //SendMessage("Сообщение получено!", lastChatId);
                                SendMessageWithReply("Сообщение получено!", lastMessageId, lastChatId);
                            }
                        }   
                    }
                }
            }
            catch(HttpRequestException re)
            {
                Console.WriteLine($"Неудалось получить ответ от сервера, необходимо проверить идентификатор токена и способ соединения.\n{re.Message}");
            }
        }
        public static void SendMessage(string message, int chatId)
        {
            using (var webclient = new WebClient())
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("message", message);
                values.Add("chatId", chatId.ToString());

                string postRequest = @$"https://api.telegram.org/bot{token}/sendMessage?chat_id={chatId}&text={message}";

                webclient.DownloadString(postRequest);
            }
        }
        public static void SendMessageWithReply(string message, int messageId, int chatId)
        {
            using (var webclient = new WebClient())
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("message", message);
                values.Add("reply_to_message_id", messageId.ToString());
                values.Add("chatId", chatId.ToString());

                string postRequest = @$"https://api.telegram.org/bot{token}/sendMessage?chat_id={chatId}&reply_to_message_id={messageId}&text={message}";

                webclient.DownloadString(postRequest);
            }
        }
    }
}
