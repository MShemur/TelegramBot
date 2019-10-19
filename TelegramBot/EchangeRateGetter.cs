using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class EchangeRateGetter
    {
        private ITelegramBotClient client;
        private InlineKeyboardMarkup markup;
        private List<string> availableCurrency;
        private JsonReader jsoner;
        private DateTime date;
        public EchangeRateGetter(ITelegramBotClient client)
        {
            client.OnMessage += Bot_Message_Get_Date;
            client.OnCallbackQuery += BotOnCallbackQueryReceived;

            this.client = client;
            client.StartReceiving();
        }

        async void Bot_Message_Get_Date(object sender, MessageEventArgs e)
        {
            string text = e?.Message?.Text;
            if (e.Message == null || e.Message.Type != MessageType.Text) return;

            if (text == "/start")
                await client.SendTextMessageAsync(chatId: e.Message.Chat, text: $"Type date",
                        ParseMode.Markdown, false, false, 0, replyMarkup: new ReplyKeyboardRemove())
                    .ConfigureAwait(false);

            else if (DateTime.TryParse(text, out date))
            {
                jsoner = new JsonReader();
                availableCurrency = new List<string>();
                Bot_Message_Get_Currency(e);
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

            if (date != DateTime.MinValue)
                if (availableCurrency.Contains(callbackQuery.Data))
                {
                    string rate = jsoner.GetRate(callbackQuery.Data);
                    await client.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: $"Exchange rate on {date.ToShortDateString()} for {callbackQuery.Data} is {rate}")
                        .ConfigureAwait(false);
                }
        }

        async void Bot_Message_Get_Currency(MessageEventArgs e)
        {
            availableCurrency = jsoner.GetCurrencyList(date.ToShortDateString());
            ButtonCreator buttonCreator = new ButtonCreator(availableCurrency);
            markup = buttonCreator.GetMurkup();
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
