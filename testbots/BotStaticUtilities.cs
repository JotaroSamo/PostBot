
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PostBot.App
{
    public static class BotStaticUtilities
    {
     private const string DomainFileName = "DomenFile.txt";

        
        public static string GetOrCreateDomainAsync()
        {
            if (!System.IO.File.Exists(DomainFileName))
            {
                string defaultDomain = "https://testings-domain.shop/";
                System.IO.File.WriteAllText(DomainFileName, defaultDomain);
            }

            return System.IO.File.ReadAllText(DomainFileName);
        }
      
      
    }

}
