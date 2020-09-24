using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    public class ButtonCreator
    {
        private Queue<string> buttons;
        private const int BUTTONS_IN_ROW = 5;
        public ButtonCreator(List<string> buttons)
        {
            this.buttons = new Queue<string>(buttons);
        }

        public InlineKeyboardMarkup GetMarkup()
        {
            var rows = (int)Math.Ceiling((double)buttons.Count / BUTTONS_IN_ROW);
            var board = new List<InlineKeyboardButton>[rows];
            var markup = new InlineKeyboardMarkup(board);

            for (int i = 0; i < rows; i++)
            {
                var button = new InlineKeyboardButton[BUTTONS_IN_ROW < buttons.Count ? BUTTONS_IN_ROW : buttons.Count];

                for (int j = 0; j < button.Length; j++)
                {
                    button[j] = InlineKeyboardButton.WithCallbackData(buttons.Dequeue());
                }
                board[i] = button.ToList();
            }
            return markup;
        }
    }
}
