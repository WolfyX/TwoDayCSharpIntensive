using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using TwoDaysCSharpIntensive;

namespace SingleDayCSharpIntensive
{
    public class CatchNewMessageFeature
    {
        private const string S_ID = "id";
        private const string S_TEXT = "text";
        private const string S_FROM = "from";
        private const string S_RESULT = "result";
        private const string S_MESSAGE = "message";
        private const string S_UPDATE_ID = "update_id";
        private const string S_FIRST_NAME = "first_name";
        private const string S_TELEGRAM_BOT_URL = "https://api.telegram.org/bot";

        private int UpdateId { get; set; }
        private string FirstName { get; set; }
        private string MessageText { get; set; }
        private string MessageFromId { get; set; }
        private string TelegramBotToken { get; }
        private WeatherFeature WeatherFeature { get; }
        private string StartUrl => $"{S_TELEGRAM_BOT_URL}{TelegramBotToken}";
        private string ResponseUrl => $"{StartUrl}/getUpdates?offset={UpdateId + 1}";
        private string PushUrl => $"{StartUrl}/sendMessage?chat_id={MessageFromId}&text={WeatherFeature.GetWeather()}";

        public CatchNewMessageFeature(string telegramBotToken)
        {
            UpdateId = 0;
            FirstName = string.Empty;
            MessageText = string.Empty;
            MessageFromId = string.Empty;

            WeatherFeature = new WeatherFeature();
            TelegramBotToken = telegramBotToken;
        }

        public void Catch()
        {
            WebClient webClient = new WebClient();

            while (true)
            {
                string response = webClient.DownloadString(ResponseUrl);

                List<JToken> messageList = JObject.Parse(response)[S_RESULT].ToList();

                foreach (JToken message in messageList)
                {
                    try
                    {
                        InitializeData(message);
                        Console.WriteLine($"{DateTime.Now} {FirstName}({MessageFromId}):{MessageText}");
                        webClient.DownloadString(PushUrl);
                    }
                    catch
                    {
                    }

                    Thread.Sleep(100);
                }
            }
        }

        private void InitializeData(JToken jToken)
        {
            JToken jTokenUpdateId = jToken[S_UPDATE_ID];
            UpdateId = Convert.ToInt32(jTokenUpdateId);

            JToken jTokenMessage = jToken[S_MESSAGE];
            MessageText = jTokenMessage[S_TEXT].ToString();

            JToken jTokenFrom = jTokenMessage[S_FROM];
            MessageFromId = jTokenFrom[S_ID].ToString();
            FirstName = jTokenFrom[S_FIRST_NAME].ToString();
        }
    }
}
