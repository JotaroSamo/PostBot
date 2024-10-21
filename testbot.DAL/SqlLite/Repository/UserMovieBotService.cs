using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testbots.DAL.SqlLite.Repository
{
    public class UserMovieBotService : IUserMovieBotService
    {
        protected readonly SqlLiteContext _context;
        public UserMovieBotService(SqlLiteContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<TUser>> GetAllAsync()
        {
            var usersB = await _context.UsersMovieBot.AsNoTracking().ToListAsync();
            return usersB;
        }

        public async Task<TUser> GetByIdUser(long id)
        {
            var user = await _context.UsersMovieBot.SingleOrDefaultAsync(i => i.UserMovieId == id);
            return user;
        }
        public async Task InsertAsync(long id)
        {
            if (id != 0)
            {
                var user = new TUser(id);
                await _context.UsersMovieBot.AddAsync(user);
                await _context.SaveChangesAsync();
            }

        }




    }
}
