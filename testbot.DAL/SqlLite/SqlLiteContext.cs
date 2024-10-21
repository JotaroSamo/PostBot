using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testbots.DAL.SqlLite
{
    public class SqlLiteContext : DbContext
    {

        public const string ConnectionStrings = "SQLLiteConnection";
        public SqlLiteContext(DbContextOptions<SqlLiteContext> options) : base(options)
        {

            Database.EnsureCreated();
        }
        public DbSet<TUser> UsersMovieBot { get; set; }


    }


}
