using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using AspSecure.Models;
using AspSecure.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace AspSecure.Services
{
  public interface IAccountService
  {
    Account Authenticate(string email, string password);
    Account Register(string email, string password);
    Account Delete(string email);
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
      if (!System.IO.File.Exists(_appSettings.AccountDir))
        System.IO.File.WriteAllText(_appSettings.AccountDir, "[]");

      List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(
          File.ReadAllText(_appSettings.AccountDir));
      var account = accounts.Find(x => x.email == email && BCrypt.Net.BCrypt.EnhancedVerify(password, x.passwordHash));

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
          new Claim(ClaimTypes.Name, account.id.ToString())
        }),
        Expires = DateTime.UtcNow.AddMinutes(5),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      account.Token = tokenHandler.WriteToken(token);

      account.passwordHash = null;

      return account;
    }

    public Account Register(string email, string password)
    {
      if (!System.IO.File.Exists(_appSettings.AccountDir))
        System.IO.File.WriteAllText(_appSettings.AccountDir, "[]");

      List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(
          File.ReadAllText(_appSettings.AccountDir));
      
      if (accounts.Where(x => x.email == email).Count() != 0) // email already exists
        return null;

      string passwordHashed = BCrypt.Net.BCrypt.EnhancedHashPassword(password);

      Account newAccount = new Account
      {
        email = email,
        passwordHash = passwordHashed,
        id = accounts.Count() + 1
      };

      accounts.Add(newAccount);

      File.WriteAllText(_appSettings.AccountDir,
          JsonConvert.SerializeObject(accounts));

      return newAccount;
    }

    public Account Delete(string email)
    {
      if (!System.IO.File.Exists(_appSettings.AccountDir))
        System.IO.File.WriteAllText(_appSettings.AccountDir, "[]");

      List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(
          File.ReadAllText(_appSettings.AccountDir));
      
      Account toDelete = accounts.Find(x => x.email == email);
      
      if (toDelete != null)
        accounts.Remove(toDelete);

      File.WriteAllText(_appSettings.AccountDir,
          JsonConvert.SerializeObject(accounts));

      return toDelete;
    }

    public IEnumerable<Account> GetAll()
    {
      if (!System.IO.File.Exists(_appSettings.AccountDir))
        System.IO.File.WriteAllText(_appSettings.AccountDir, "[]");

      List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(
          File.ReadAllText(_appSettings.AccountDir));
      return accounts.Select(x => {
          x.passwordHash = null;
          return x;
          });
    }
  }
}
