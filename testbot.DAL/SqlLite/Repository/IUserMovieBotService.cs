namespace testbots.DAL.SqlLite.Repository
{
    public interface IUserMovieBotService
    {
        Task<TUser> GetByIdUser(long id);
        Task<IEnumerable<TUser>> GetAllAsync();
        Task InsertAsync(long id);
    }
}