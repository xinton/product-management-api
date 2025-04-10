using Microsoft.EntityFrameworkCore;
using apiB2e.Entities;
using apiB2e.Interfaces;
using apiB2e.Infrastructure;
using System.Security.Cryptography;
using System.Text;

namespace apiB2e.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByLoginAsync(string login)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<bool> ValidateUserAsync(string login, string password)
        {
            var user = await GetUserByLoginAsync(login);
            
            if (user == null)
                return false;
                
            // Verify password hash
            string hashedPassword = HashPassword(password);
            return user.Senha == hashedPassword;
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}