using Contactos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contactos.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetByUsername(string username);
        User Create(User user, string password);
        User Update(User user, string password = null);
        bool Delete(string username);
    }

    public class UserService : IUserService
    {
        private ContactosContext _context;

        public UserService(ContactosContext ctx)
        {
            _context = ctx;
        }

        public User Authenticate(string username, string password)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            if (user == null)
                return null;

            if (!ValidarPasowordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public User Create(User user, string password)
        {
            byte[] passwordHash = null;
            byte[] passwordSalt = null;

            GenerarPasowrdHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);

            _context.SaveChanges();

            return user;
        }

        public bool Delete(string username)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == username);

            if (user == null)
                return false;

            _context.Users.Remove(user);
            _context.SaveChanges();

            return true;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetByUsername(string username)
        {
           return  _context.Users.SingleOrDefault(x => x.Username == username);
        }

        public User Update(User user, string password = null)
        {
            throw new NotImplementedException();
        }

        private static void GenerarPasowrdHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool ValidarPasowordHash(string password,  byte[] storedHash, byte[] storeSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storeSalt))
            {
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < passwordHash.Length ; i++)
                {
                    if (passwordHash[i] != storedHash[i]) return false;
                }

            }

            return true;
        }
    }
}
