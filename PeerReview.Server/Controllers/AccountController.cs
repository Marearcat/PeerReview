using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PeerReview.Core.Account;
using PeerReview.Core.DTOs;
using PeerReview.Core.Models;
using PeerReview.Server.Data;

namespace PeerReview.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        readonly ApplicationDbContext context;
        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;

        public AccountController(ApplicationDbContext context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            this.context = context;
        }

        [Route("[action]")]
        [HttpGet]
        public bool Autorized() => User.Identity.IsAuthenticated;

        [Route("[action]")]
        [HttpGet]
        public IEnumerable<string> Register() => context.Specs.Select(x => x.Name);

        [Route("[action]")]
        [HttpPost]
        public async Task<bool> Register([FromBody]RegistrationPost model)
        {
            if (ModelState.IsValid)
            {
                var mail = context.Invites.First(x => x.Id == model.Key).Email;
                User user = new User { Email = mail, FullName = model.FullName, Nick = model.Nick };
                // добавляем пользователя
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // установка куки
                    await _signInManager.SignInAsync(user, false);
                    return true;
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return false;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<bool> Login([FromBody]LoginPost model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.Remember, false);
                return result.Succeeded;
            }
            return false;
        }

        [Route("[action]")]
        [HttpPost]
        public async void LogOff() =>
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();

        [Route("[action]")]
        [HttpGet]
        public async Task<AccountInfoGet> Info()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var model = new AccountInfoGet
            {
                Mail = user.Email,
                FullName = user.FullName,
                Nick = user.Nick
            };
            var specs = context.UserToSpecs.Where(x => x.UserId == user.Id).Select(x => x.SpecId);
            model.Specs = context.Specs.Where(x => specs.Contains(x.Id)).Select(x => x.Name);
            var inviterId = context.Invites.First(x => x.Email == user.Id).InviterId;
            model.Inviter = context.Users.First(x => x.Id == inviterId).Email;
            return model;
        }
    }
}
