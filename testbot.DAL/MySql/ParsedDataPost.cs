using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostBot.DAL.MySql
{
    public class ParsedDataPost
    {
        public int id { get; set; }
        public string Kpid { get; set; }
        public string Image { get; set; }

        public string Desc { get; set; }
        public string Seria { get; set; }
        public string Season { get; set; }

        public string alt_name { get; set; }
    }
}
