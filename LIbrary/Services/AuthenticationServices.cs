using LIbrary.Command;
using LIbrary.Models;
using MongoDB.Driver;

namespace LIbrary.Services
{
    public class AuthenticationServices
    {
        private static readonly Operations<User> _useroperations = new();

        public async static Task<bool> Register(string username, string name, string password)
        {
            var alreadyExists = await _useroperations.Find(Builders<User>.Filter.Eq(u => u.UserName, username));
            if (alreadyExists != null) return false;

            var newUser = new User
            {
                Name = name,
                UserName = username,
                Password = BCrypt.Net.BCrypt.HashPassword(password)
            };

            await _useroperations.AddOne(newUser);

            return true;
        }

        public async static Task<User?> Authenticate(string username, string password)
        {
            var user = await _useroperations.Find(Builders<User>.Filter.Eq(u => u.UserName, username));
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) return user;

            return null;
        }
    }
}
