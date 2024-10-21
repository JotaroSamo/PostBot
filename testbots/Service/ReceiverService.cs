using MovieBot.Abstarct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace testbots.Service
{
    public class ReceiverService : ReceiverServiceBase<UpdateHandler>
    {
        public ReceiverService(
            ITelegramBotClient botClient,
            UpdateHandler updateHandler,
            ILogger<ReceiverServiceBase<UpdateHandler>> logger)
            : base(botClient, updateHandler, logger)
        {
        }
    }
}
