using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PeerReview.Core.Account;
using PeerReview.Core.Models;
using PeerReview.Core.Unite;


namespace PeerReview.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Invite> Invites { get; set; }
        public DbSet<Bank> Bank { get; set; }
        public DbSet<Loop> Loop { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Spec> Specs { get; set; }
        public DbSet<UserToSpec> UserToSpecs { get; set; }
        public DbSet<BlackList> BlackList { get; set; }
    }
}
