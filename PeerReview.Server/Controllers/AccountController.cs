using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PeerReview.Core.Account;
using PeerReview.Core.DTOs;
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

        [HttpPost]
        public async Task<bool> Register(RegistrationDTO model)
        {
            if (ModelState.IsValid)
            {
                User user = new User { Email = model.Email, UserName = model.Email, FullName = model.FullName, Nick = model.Nick };
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
    }
}
