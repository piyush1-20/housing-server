using API.Models;
using System.Globalization;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        Task<User> Authenticate(string UserEmail, string UserPassword);
        void Register(string userEmail, string password);
        Task<bool> UserAlreadyExists(string userEmail);
    }
}
