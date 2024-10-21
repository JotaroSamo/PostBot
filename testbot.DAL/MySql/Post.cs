using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testbots.DAL.MySql
{
    public class Post
    {
        public int id { get; set; }
        public string xfields { get; set; } = string.Empty;

        public string alt_name { get; set; } = string.Empty;
    }
}
