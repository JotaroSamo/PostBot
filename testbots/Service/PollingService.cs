﻿
using Microsoft.Extensions.Options;
using PostBot.Abstarct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostBot.Service
{
    public class PollingService : PollingServiceBase<ReceiverService>
    {
        public PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger, IOptions<BotConfiguration> options)
            : base(serviceProvider, logger, options)
        {
        }
    }
}
