﻿using testbots.DAL.MySql;

namespace testbots.DAL.MySql.Repository
{
    public interface IPostRepository
    {
        Task<IEnumerable<ParsedDataPost>> SearchPostAsync(string query);
        Task<ParsedDataPost> GetPostById(int id);
        ParsedDataPost DataPostParse(string input, string alt_name, int id);
    }
}