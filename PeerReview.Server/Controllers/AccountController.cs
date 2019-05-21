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
        public IEnumerable<string> Register()
        {
            return context.Specs.Select(x => x.Name);
        }

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
                    var invite = context.Invites.First(x => x.Id == model.Key);
                    invite.Confirmed = true;
                    context.Invites.Update(invite);
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

        [Route("[action]")]
        [HttpGet]
        public async Task<EditUser> Edit()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            var model = new EditUser
            {
                Nick = user.Nick,
                FullName = user.FullName,
                Specs = context.Specs.Select(x => x.Name)
            };   
            return model;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<bool> PostEdit(EditUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (user != null)
                {
                    user.Nick = model.Nick;
                    user.FullName = model.FullName;
                    context.UserToSpecs.RemoveRange(context.UserToSpecs.Where(x => x.UserId == user.Id));
                    var specs = context.Specs;
                    foreach (var spec in model.Specs)
                    {
                        var specId = context.Specs.First(x => x.Name == spec);
                        context.UserToSpecs.Add(new Core.Unite.UserToSpec { SpecId = spec, UserId = user.Id });
                    }
                    context.SaveChanges();
                    var result = await _userManager.UpdateAsync(user);
                    return result.Succeeded;
                }
            }
            return false;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<bool> ChangePassword(string NewPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (user != null)
                {
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(_userManager, user, NewPassword);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, NewPassword);
                        await _userManager.UpdateAsync(user);
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
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return false;
        }
    }
}
