using System;
using Telegram.Bot;

namespace TelegramBot
{
    class Program
    {
        private static ITelegramBotClient botClient;
        static void Main(string[] args)
        {
            botClient = new TelegramBotClient("960297039:AAEOKdVUhdXO4YCk3hlZoThTcxjx3HurV1s") { Timeout = TimeSpan.FromSeconds(10) };
            _ = new ExchangeRateGetter(botClient);
            Console.ReadLine();
        }
    }
}

