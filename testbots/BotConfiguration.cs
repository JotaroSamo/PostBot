using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostBot
{
    public class BotConfiguration
    {
        public string BotToken { get; init; } = default!;
        public long ChannelId { get; set; } = default!;
    }
}
