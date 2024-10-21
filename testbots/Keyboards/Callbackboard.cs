using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace testbots.Keyboards
{
    public class CallbackBoard
    {
        public static readonly InlineKeyboardMarkup InlineBtnPanel = new(new[] {
                new []{
                    InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Поиск"),
                },

            });

    }
}
