using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class ExchangeRateGetter
    {
        private ITelegramBotClient client;
        private InlineKeyboardMarkup markup;
        private List<string> availableCurrency;
        private JsonReader jsonReader;
        private DateTime date;
        public ExchangeRateGetter(ITelegramBotClient client)
        {
            client.OnMessage += BotMessageGetDate;
            client.OnCallbackQuery += BotOnCallbackQueryReceived;

            this.client = client;
            client.StartReceiving();
        }

        private async void BotMessageGetDate(object sender, MessageEventArgs e)
        {
            string text = e?.Message?.Text;
            if (e.Message == null || e.Message.Type != MessageType.Text) return;
            if (text == "/start")
            {
                await client.SendTextMessageAsync(chatId: e.Message.Chat, text: $"Type date",
                        ParseMode.Markdown, false, false, 0, replyMarkup: new ReplyKeyboardRemove())
                    .ConfigureAwait(false);
            }
            else if (DateTime.TryParse(text, out date))
            {
                jsonReader = new JsonReader();
                availableCurrency = new List<string>();
                BotMessageGetCurrency(e);
            }
            else
            {
                await client.SendTextMessageAsync(chatId: e.Message.Chat, text: $"Wrong data input",
                        ParseMode.Markdown, false, false, 0, replyMarkup: new ReplyKeyboardRemove())
                    .ConfigureAwait(false);
            }
        }

        private async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            if (date == DateTime.MinValue) return;
            if (!availableCurrency.Contains(callbackQuery.Data)) return;

            string rate = jsonReader.GetRate(callbackQuery.Data);
            Console.WriteLine($"{callbackQuery.Message.From} asked for exchange rate on {date.ToShortDateString()} for {callbackQuery.Data} is {rate}");

            await client.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: $"Exchange rate on {date.ToShortDateString()} for {callbackQuery.Data} is {rate}")
                .ConfigureAwait(false);
        }

        private async void BotMessageGetCurrency(MessageEventArgs e)
        {
            availableCurrency = jsonReader.GetCurrencyList(date.ToShortDateString());
            var buttonCreator = new ButtonCreator(availableCurrency);
            markup = buttonCreator.GetMarkup();
            if (markup.InlineKeyboard.Any())
            {
                await client.SendTextMessageAsync(chatId: e.Message.Chat, text: $"Choose currency",
                        ParseMode.Markdown, false, false, 0, markup)
                    .ConfigureAwait(false);
            }
            else
            {
                await client.SendTextMessageAsync(chatId: e.Message.Chat, text: $"Seems like on this date I don't have any exchange rates",
                        ParseMode.Markdown, false, false, 0, markup)
                    .ConfigureAwait(false);
                await client.SendTextMessageAsync(chatId: e.Message.Chat, text: $"Try again",
                        ParseMode.Markdown, false, false, 0, markup)
                    .ConfigureAwait(false);
            }
        }
    }
}
