using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using ObservationAPI.Models;
using ObservationAPI.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ObservationAPI.Services
{
    public interface IAccountService
    {
        Account Authenticate(string email, string password);
        Account Register(string email, string password);
        Account Delete(string email, string password);
        IEnumerable<Account> GetAll();
    }

    public class AccountService : IAccountService
    {
        private readonly AppSettings _appSettings;

        public AccountService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public Account Authenticate(string email, string password)
        {
            if (!File.Exists(_appSettings.AccountDir))
                File.WriteAllText(_appSettings.AccountDir, "[]");

            List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(
                    File.ReadAllText(_appSettings.AccountDir));
            var account = accounts.Find(x => x.Email == email && BCrypt.Net.BCrypt.EnhancedVerify(password, x.PasswordHash));

            // Return if either email or password is wrong
            if (account == null)
                return null;

            // Generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                        {
                        new Claim(ClaimTypes.Name, account.Id.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddMinutes(5),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            account.Token = tokenHandler.WriteToken(token);

            account.PasswordHash = null;

            return account;
        }

        public Account Register(string email, string password)
        {
            if (!File.Exists(_appSettings.AccountDir))
                File.WriteAllText(_appSettings.AccountDir, "[]");

            List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(
                    File.ReadAllText(_appSettings.AccountDir));

            if (accounts.Find(x => x.Email == email) != null) // email already exists
                return null;

            string passwordHashed = BCrypt.Net.BCrypt.EnhancedHashPassword(password);

            // Find lowest available id
            int lowestId = 1;
            while (accounts.Find(x => x.Id == lowestId) != null)
                lowestId++;

            Account newAccount = new Account
            {
                Email = email,
                PasswordHash = passwordHashed,
                Id = lowestId
            };

            accounts.Add(newAccount);

            File.WriteAllText(_appSettings.AccountDir,
                    JsonConvert.SerializeObject(accounts));

            newAccount.PasswordHash = null;

            return newAccount;
        }

        public Account Delete(string email, string password)
        {
            if (!File.Exists(_appSettings.AccountDir))
                File.WriteAllText(_appSettings.AccountDir, "[]");

            List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(
                    File.ReadAllText(_appSettings.AccountDir));

            Account toDelete = accounts.Find(x => x.Email == email && BCrypt.Net.BCrypt.EnhancedVerify(password, x.PasswordHash));

            if (toDelete == null)
                return null;

            accounts.Remove(toDelete);

            File.WriteAllText(_appSettings.AccountDir,
                    JsonConvert.SerializeObject(accounts));

            toDelete.PasswordHash = null;

            return toDelete;
        }

        public IEnumerable<Account> GetAll()
        {
            if (!File.Exists(_appSettings.AccountDir))
                File.WriteAllText(_appSettings.AccountDir, "[]");

            List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(
                    File.ReadAllText(_appSettings.AccountDir));
            return accounts.Select(x => {
                    x.PasswordHash = null;
                    return x;
                    });
        }
    }
}
