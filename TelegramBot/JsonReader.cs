﻿using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace TelegramBot
{
    public class JsonReader
    {
        private string json;

        public string GetRate(string currency)
        {
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            foreach (var variable in obj.exchangeRate)
            {
                if (variable.currency == currency)
                {
                    return variable.saleRateNB + "/" + variable.purchaseRateNB;
                }
            }
            return null;
        }

        public List<string> GetCurrencyList(string date)
        {
            string url = @"https://api.privatbank.ua/p24api/exchange_rates?json&date=";
            url += date;
            List<string> currencyList = new List<string>();
            json = new WebClient().DownloadString(url);

            dynamic obj = JsonConvert.DeserializeObject<dynamic>(json);
            foreach (var variable in obj.exchangeRate)
            {
                if (variable?.currency != null)
                    currencyList.Add(variable?.currency?.ToString());
            }
            return currencyList;
        }
    }
}
