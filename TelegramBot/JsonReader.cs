using System.Collections.Generic;
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
                    return variable.saleRate + "/" + variable.purchaseRate;
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
            foreach (var item in obj.exchangeRate)
            {
                if (item?.currency != null)
                    currencyList.Add(item?.currency?.ToString());
            }
            return currencyList;
        }
    }
}
