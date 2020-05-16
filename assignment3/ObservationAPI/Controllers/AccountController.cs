using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ObservationAPI.Models;
using ObservationAPI.Services;

namespace ObservationAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] Account accountParam)
        {
            if (accountParam.email == null)
                return BadRequest("No email provided");

            if (accountParam.password == null)
                return BadRequest("No password provided");

            var account = _accountService.Authenticate(accountParam.email, accountParam.password);

            if (account == null)
                return BadRequest("Email or password is incorrect");

            return Ok(account);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] Account accountParam)
        {
            if (accountParam.email == null)
                return BadRequest("No email provided");

            if (accountParam.password == null)
                return BadRequest("No password provided");

            var account = _accountService.Register(accountParam.email, accountParam.password);

            if (account == null)
                return BadRequest("Email already taken");

            return Ok(account);
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] Account accountParam)
        {
            if (accountParam.email == null)
                return BadRequest("No email provided");

            if (accountParam.password == null)
                return BadRequest("No password provided");

            Account deleted = _accountService.Delete(accountParam.email, accountParam.password);

            if (deleted == null)
                return BadRequest("Email or password is incorrect");

            return Ok(deleted);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var accounts = _accountService.GetAll();
            return Ok(accounts);
        }
    }
}
