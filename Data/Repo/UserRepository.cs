using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Data.Repo
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext dc;

        public UserRepository(DataContext dc)
        {
            this.dc = dc;
        }
        public async Task<User> Authenticate(string userEmail, string passwordText)
        {
            var user =  await dc.Users.FirstOrDefaultAsync(x => x.UserEmail == userEmail);
            
            if (user == null || user.PasswordKey==null)
                return null;

            if (!MatchPasswordHash(passwordText, user.Password, user.PasswordKey))
                return null;

            return user;
        }

        private bool MatchPasswordHash(string passwordText, byte[] password, byte[] passwordKey)
        {
            using (var hmac = new HMACSHA512(passwordKey))
            {
             
               var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(passwordText));

                for(int i = 0; i < passwordHash.Length; i++)
                {
                    if (passwordHash[i] != password[i])
                        return false;
                }

                return true;
            }

        }

        public void Register(string userEmail, string password)
        {
            byte[] passwordHash, passwordKey;
            using(var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            User user = new User();
            user.UserEmail = userEmail;
            user.Password = passwordHash;
            user.PasswordKey = passwordKey;

            dc.Users.Add(user);
        }

        public async Task<bool> UserAlreadyExists(string userEmail)
        {
            return await dc.Users.AnyAsync(x => x.UserEmail == userEmail);
        }
    }
}
