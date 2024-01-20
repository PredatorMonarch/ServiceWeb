
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using Konscious.Security.Cryptography;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Security.Cryptography;

namespace UserService.Entities
{
    
    public class User
    {
        [MaxLength(128)]
        public string Id { get; private set; }
        [MaxLength(30)]
        public string Username { get; set; }
        [MaxLength(254)]
        public string Email { get; set; }
        [MaxLength(88)] 
        private string Hash { get; set; }
        private byte[] Salt { get; set; } = new byte[16];
        [MaxLength(200)]
        public string? Token { get; private set; } = null;

        private User()
        {
            
        }
        public User(string username, string email, string password)
        {
            this.Username = username;
            this.Email = email;
            this.Id = User.ComputeId(username, email);
            new Random().NextBytes(this.Salt);
            using var hasher = new Argon2d(Encoding.UTF8.GetBytes(password));
            hasher.Salt = this.Salt;
            hasher.Iterations = 5;
            hasher.DegreeOfParallelism = 1;
            hasher.MemorySize = 65536;
            this.Hash = Convert.ToBase64String(hasher.GetBytes(64));
        }
        
        public bool CheckPassword(string password)
        {
            using var hasher = new Argon2d(Encoding.UTF8.GetBytes(password));
            hasher.Salt = this.Salt;
            hasher.Iterations = 5;
            hasher.DegreeOfParallelism = 1;
            hasher.MemorySize = 65536;
            return this.Hash == Convert.ToBase64String(hasher.GetBytes(64));
        }
        
        public void SetPassword(string password)
        {
            new Random().NextBytes(this.Salt);
            using var hasher = new Argon2d(Encoding.UTF8.GetBytes(password));
            hasher.Salt = this.Salt;
            hasher.Iterations = 5;
            hasher.DegreeOfParallelism = 1;
            hasher.MemorySize = 65536;
            this.Hash = Convert.ToBase64String(hasher.GetBytes(64));
        }
        public static string ComputeId(string username, string email)
        {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(username)).ToString() + SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(email)).ToString();
        }
    }

    public class UserUpdateModel(string username, string email, string pass)
    {
        public required string Pass { get; set; } = pass;
        public required string Username { get; set; } = username;
        public required string Email { get; set; } = pass;
    }
    public class UserLoginModel(string login, string pass)
    {
        public required string Login { get; set; } = login;
        public required string Pass { get; set; } = pass;
    }
    public class UserDTO(User user)
    {
        public string Id { get; set; } = user.Id;
        public string Username { get; set; } = user.Username;
        public string Email { get; set; } = user.Email;
    }
}
