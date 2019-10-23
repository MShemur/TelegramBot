using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class ExchangeRate
    {
        private ITelegramBotClient client;
        private InlineKeyboardMarkup markup;
        private List<string> availableCurrency;
        private DateTime date;
        private string json;

        public ExchangeRate(ITelegramBotClient client)
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
                json = GetJson(date);
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

            string rate = GetRate(callbackQuery.Data, json);
            Console.WriteLine($"{callbackQuery.Message.From} asked for exchange rate on {date.ToShortDateString()} for {callbackQuery.Data} is {rate}");

            await client.SendTextMessageAsync(chatId: callbackQuery.Message.Chat.Id, text: $"Exchange rate on {date.ToShortDateString()} for {callbackQuery.Data} is {rate}")
                .ConfigureAwait(false);
        }

        private async void BotMessageGetCurrency(MessageEventArgs e)
        {
            availableCurrency = GetCurrencyList(json);
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

        public string GetRate(string currency, string json)
        {
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            foreach (var variable in obj.exchangeRate)
            {
                if (variable.currency == currency)
                {
                    if (variable?.saleRate != null)
                    {
                        return variable.saleRate + "/" + variable.purchaseRate;
                    }
                    else
                    {
                        return variable.saleRateNB + "/" + variable.purchaseRateNB + " (NB rating)";
                    }
                }
            }
            return null;
        }

        private string GetJson(DateTime dateTime)
        {
            string date = dateTime.ToShortDateString();
            string url = @"https://api.privatbank.ua/p24api/exchange_rates?json&date=";
            url += date;
            string json = new WebClient().DownloadString(url);
            return json;
        }

        public List<string> GetCurrencyList(string json)
        {
            List<string> currencyList = new List<string>();

            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            foreach (var item in obj.exchangeRate)
            {
                if (item?.currency != null)
                    currencyList.Add(item?.currency?.ToString());
            }
            return currencyList;
        }
    }
}