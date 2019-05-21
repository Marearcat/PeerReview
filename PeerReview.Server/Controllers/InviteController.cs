using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PeerReview.Core.Account;
using PeerReview.Server.Data;

namespace PeerReview.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InviteController : ControllerBase
    {
        readonly ApplicationDbContext context;
        readonly UserManager<User> _userManager;
        
        public InviteController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            this.context = context;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<bool> Create(string mail)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            try
            {
                if (context.Invites.Any(x => x.Email == mail))
                {
                    var invite = context.Invites.First(x => x.Email == mail);
                    if (invite.InviterId == user.Id || invite.Confirmed)
                        return false;
                    else
                    {
                        invite.InviterId = user.Id;
                        context.Invites.Update(invite);
                    }
                }
                else
                    context.Invites.Add(new Invite { Confirmed = false, Email = mail, InviterId = user.Id });
                context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IEnumerable<Invite>> GetInvites()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            return context.Invites.Where(x => x.InviterId == user.Id);
        }
    }
}
