using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    //NOTE: this is the ORM dealing with the DB
    public class DataContext : IdentityDbContext<
        AppUser, AppRole, int,
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        // passing some options in the startup class to add it to the DI Container
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected DataContext()
        {
        }
        public DbSet<UserFollow> Followings { get; set; }

        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            
            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            builder.Entity<UserFollow>()
                .HasKey(k => new {k.SourceUserId, k.FollowedUserId});
            
            builder.Entity<UserFollow>()
                .HasOne(s => s.SourceUser)
                .WithMany(f => f.FollowedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.Entity<UserFollow>()
                .HasOne(s => s.FollowedUesr)
                .WithMany(f => f.UserFollowings)
                .HasForeignKey(s => s.FollowedUserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}