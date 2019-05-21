using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PeerReview.Core.Account;
using PeerReview.Core.Models;
using PeerReview.Server.Data;

namespace PeerReview.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        readonly ApplicationDbContext context;
        readonly UserManager<User> _userManager;

        public ArticleController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            this.context = context;
        }

        [Route("[action]")]
        [HttpGet]
        public IEnumerable<Bank> Index() => context.Bank;

        [Route("[action]")]
        [HttpGet]
        public Bank GetBank(string id) => context.Bank.First(x => x.Id == id);

        [Route("[action]")]
        [HttpPost]
        public async Task<bool> PostCreate([FromBody]Loop loopEntity)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (context.UserToSpecs.Any(x => x.SpecId == loopEntity.SpecId && x.UserId != user.Id))
                {
                    var count = context.UserToSpecs.Where(x => x.SpecId == loopEntity.SpecId && x.UserId != user.Id).Count();
                    if (count > 25)
                        count = 25;
                    if (count < 5)
                        return false;
                    loopEntity.NeedReviews = count;
                    var path = "~/feautures/" + Guid.NewGuid();
                    while (context.Bank.Select(x => x.Path).Contains(path) || context.Loop.Select(x => x.Path).Contains(path))
                        path = "~/feautures/" + Guid.NewGuid();
                    loopEntity.Path = path;
                    context.Loop.Add(loopEntity);
                    var loop = context.Loop.First(x => x.Path == path);
                    context.BlackList.Add(new Core.Unite.BlackList { UserId = user.Id, ArticleId = loop.Id });
                    while (count > 0)
                    {
                        var length = context.UserToSpecs.Where(x => x.SpecId == loopEntity.SpecId && x.UserId != user.Id).Count() - 1;
                        var enumerator = _userManager.Users.GetEnumerator();
                        Random rnd = new Random();
                        var index = rnd.Next(0, length);
                        while(index > 0)
                        {
                            enumerator.MoveNext();
                            index--;
                        }
                        user = enumerator.Current;
                        if (!context.Orders.Any(x => x.UserId == user.Id && x.ArticleId == loop.Id) && !context.BlackList.Any(x => x.UserId == user.Id && x.ArticleId == loop.Id))
                        {
                            var time = DateTime.Now;
                            time.AddDays(5);
                            context.Orders.Add(new Order { Name = loop.Name, UserId = user.Id, ArticleId = loop.Id, Deadline = time });
                            count--;
                        }
                    }
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
