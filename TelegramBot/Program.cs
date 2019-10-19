using System;
using Telegram.Bot;

namespace TelegramBot
{
    class Program
    {
        private static ITelegramBotClient BotClient;
        static void Main(string[] args)
        {
            BotClient = new TelegramBotClient("960297039:AAEOKdVUhdXO4YCk3hlZoThTcxjx3HurV1s") { Timeout = TimeSpan.FromSeconds(10) };
            _ = new EchangeRateGetter(BotClient);
            Console.ReadLine();
        }
    }
}

