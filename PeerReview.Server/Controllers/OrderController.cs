using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class OrderController : ControllerBase
    {
        readonly ApplicationDbContext context;
        readonly UserManager<User> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _userManager = userManager;
            this.context = context;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<bool> Create([FromBody]CreateReview review)
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            if (review.Accept)
            {
                var order = context.Orders.First(x => x.UserId == user.Id && x.ActicleId == review.ArticleId);
                context.Reviews.Add(new Core.Models.Review { Mark = review.Rating, ArticleId = review.ArticleId, UserId = user.Id });
                if (context.Loop.First(x => x.Id == review.ArticleId).NeedReviews == context.Reviews.Where(x => x.ArticleId == review.ArticleId).Count())
                {
                    var loop = context.Loop.First(x => x.Id == review.ArticleId);
                    var bank = new Bank
                    {
                        Name = loop.Name,
                        Path = loop.Path,
                        Rating = context.Reviews.Where(x => x.ArticleId == loop.Id).Sum(x => x.Mark) / loop.NeedReviews,
                        SpecId = loop.SpecId,
                        UserId = loop.UserId
                    };
                    context.Bank.Add(bank);
                    context.Loop.Remove(loop);
                    context.Reviews.RemoveRange(context.Reviews.Where(x => x.ArticleId == review.ArticleId));
                }
                context.Orders.Remove(order);
                context.SaveChanges();
                return true;
            }
            else
            {
                context.BlackList.Add(new Core.Unite.BlackList { UserId = user.Id, ArticleId = review.ArticleId });
                context.Orders.Remove(context.Orders.First(x => x.UserId == user.Id && x.ActicleId == review.ArticleId));
                if (_userManager.Users.Any(x => !context.BlackList.Any(a => a.UserId == x.Id && a.ArticleId == review.ArticleId)
                     && !context.Orders.Any(b => b.ActicleId == review.ArticleId && b.UserId == x.Id)
                     && context.UserToSpecs.Any(c => c.SpecId == context.Loop.First(d => d.Id == review.ArticleId).SpecId & c.UserId == x.Id)))
                {
                    bool flag = false;
                    while (!flag)
                    {
                        var length = context.UserToSpecs.Where(x => x.SpecId == context.Loop.First(y => y.Id == review.ArticleId).SpecId && x.UserId != user.Id).Count() - 1;
                        var enumerator = _userManager.Users.GetEnumerator();
                        var loop = context.Loop.First(x => x.Id == review.ArticleId);
                        Random rnd = new Random();
                        var index = rnd.Next(0, length);
                        while (index > 0)
                        {
                            enumerator.MoveNext();
                            index--;
                        }
                        user = enumerator.Current;
                        if (!context.Orders.Any(x => x.UserId == user.Id && x.ActicleId == loop.Id) && !context.BlackList.Any(x => x.UserId == user.Id && x.ArticleId == loop.Id))
                        {
                            var time = DateTime.Now;
                            time.AddDays(5);
                            context.Orders.Add(new Order { Name = loop.Name, UserId = user.Id, ActicleId = loop.Id, Deadline = time });
                            flag = true;
                        }
                    }
                }
                else
                {
                    context.Loop.Remove(context.Loop.First(x => x.Id == review.ArticleId));
                    context.Orders.RemoveRange(context.Orders.Where(x => x.ActicleId == review.ArticleId));
                    context.Reviews.RemoveRange(context.Reviews.Where(x => x.ArticleId == review.ArticleId));
                }
                context.SaveChanges();
                return false;
            }
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<IEnumerable<Order>> OrdersAsync()
        {
            var user = await _userManager.FindByEmailAsync(User.Identity.Name);
            return context.Orders.Where(x => x.UserId == user.Id);
        }
    }
}
