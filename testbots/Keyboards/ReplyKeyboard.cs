using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace PostBot.Keyboards
{
    public class ReplyKeyboard
    {
        public static readonly ReplyKeyboardMarkup ReplyBtnPanel = new(new[] {

               new KeyboardButton[] { "Кнопка 1"},
               new KeyboardButton[] { "Кнопка 2"},
            });
    }
}
