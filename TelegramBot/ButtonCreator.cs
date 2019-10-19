using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    class ButtonCreator
    {
        private Queue<string> buttons;
        private const int BUTTONS_IN_ROW = 5;
        public ButtonCreator(List<string> buttons)
        {
            this.buttons = new Queue<string>(buttons);

        }

        public InlineKeyboardMarkup GetMurkup()
        {
            int rows = (int)Math.Ceiling((double)buttons.Count / BUTTONS_IN_ROW);
         //   markup = new IEnumerable<KeyboardButton>[rows];

            var board = new List<InlineKeyboardButton>[rows];
            var markup = new InlineKeyboardMarkup(board);


            for (int i = 0; i < rows; i++)
            {
                var button = new InlineKeyboardButton[BUTTONS_IN_ROW < buttons.Count ? BUTTONS_IN_ROW : buttons.Count];

                for (int j = 0; j < button.Length; j++)
                {
                    // if(buttons.Count>0)
                    button[j]= InlineKeyboardButton.WithCallbackData(buttons.Dequeue());
                }

                board[i] = button.ToList();

            }

          //  markup = board.AsEnumerable();
            return markup;
            /*
            markup.Keyboard = new IEnumerable<KeyboardButton>[2]{ new KeyboardButton[4]
                {
                    new KeyboardButton(){
                        Text = "CHF"},
                    new KeyboardButton(){
                        Text = "EUR"},
                    new KeyboardButton(){
                        Text = "RUR"},
                    new KeyboardButton(){
                        Text = "GBP"},

                },
                new KeyboardButton[4]
                {
                    new KeyboardButton(){
                        Text = "USD"},
                    new KeyboardButton(){
                        Text = "EUR"},
                    new KeyboardButton(){
                        Text = "RUR"},
                    new KeyboardButton(){
                        Text = "GBP"},

                }};
                */
        }

    }
}
