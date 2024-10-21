using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using testbots.DAL.MySql;
namespace testbots.DAL.MySql.Repository
{


    public class PostRepository : IPostRepository
    {
        private readonly MySqlContext _context;

        public PostRepository(MySqlContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ParsedDataPost>> SearchPostAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                var post = await _context.Posts.Take(7).ToListAsync();
                var result = new List<ParsedDataPost>();

                foreach (var item in post)
                {
                    result.Add(DataPostParse(item.xfields, item.alt_name, item.id));
                }

                return result;
            }

            IQueryable<Post> postQuery = _context.Posts.AsNoTracking();

            // Проверка, является ли query числом
            if (long.TryParse(query, out long id)) // или int.TryParse, в зависимости от вашего типа
            {
                postQuery = postQuery.Where(p => p.id == id);
            }
            var filteredPosts = await postQuery.ToListAsync();
            var parsedResults = filteredPosts.Select(item => DataPostParse(item.xfields, item.alt_name, item.id));

            return parsedResults;
        }



        // Чтение одной записи по Id
        public async Task<ParsedDataPost> GetPostById(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            return DataPostParse(post.xfields, post.alt_name, post.id);
        }
        public ParsedDataPost DataPostParse(string input, string alt_name, int id)
        {
            var data = new ParsedDataPost();
            var pairs = input.Split(new[] { "||" }, StringSplitOptions.None);

            foreach (var pair in pairs)
            {
                var keyValue = pair.Split('|');
                if (keyValue.Length == 2)
                {
                    switch (keyValue[0])
                    {
                        case "kpid":
                            data.Kpid = keyValue[1];
                            break;
                        case "image":
                            data.Image = keyValue[1];
                            break;
                        case "seria":
                            data.Seria = keyValue[1];
                            break;
                        case "season":
                            data.Season = keyValue[1];
                            break;
                        case "text_tg":
                            data.Desc = keyValue[1];
                            break;
                    }
                }
                data.id = id;
                data.alt_name = alt_name;
            }

            return data;
        }


    }

}
