using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Train_Car_Inventory_App.Models;

namespace Train_Car_Inventory_App.Context
{
    public class TrainCarDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public DbSet<TrainCar> TrainCars { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public TrainCarDbContext(DbContextOptions<TrainCarDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TrainCarConfiguration());
            modelBuilder.ApplyConfiguration(new LocationConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UsersWithRolesConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}