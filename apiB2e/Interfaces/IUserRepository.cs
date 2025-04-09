using apiB2e.Entities;

namespace apiB2e.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByLoginAsync(string login);
        Task<bool> ValidateUserAsync(string login, string password);
    }
}