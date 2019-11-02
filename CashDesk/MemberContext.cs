using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk
{
    class MemberContext : DbContext
    {

        public DbSet<Member> Members { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Deposit> Deposits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseInMemoryDatabase("CashDesk");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>()
                .HasIndex(m => m.LastName)
                .IsUnique();

            modelBuilder.Entity<Member>()
                .HasMany(m => m.Memberships)
                .WithOne(m => m.Member)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Membership>()
                .HasMany(m => m.Deposits)
                .WithOne(m => m.Membership)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

