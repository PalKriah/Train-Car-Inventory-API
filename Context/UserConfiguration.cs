using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Train_Car_Inventory_App.Models;

namespace Train_Car_Inventory_App.Context
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(appuser => appuser.Id);

            builder.HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            
            var admin = new ApplicationUser
            {
                Id = 1,
                UserName = "DefaultAdmin",
                NormalizedUserName = "DEFAULTADMIN",
                Email = "admin@admin.com",
                NormalizedEmail = "ADMIN@ADMIN.COM",
                EmailConfirmed = true,
                BirthDate = new DateTime(1980,1,1),
                SecurityStamp = new Guid().ToString()
            };

            admin.PasswordHash = PasswordGenerate(admin, "SuperSecretPass11;");
            
            builder.HasData(admin);
            
            var user = new ApplicationUser
            {
                Id = 2,
                UserName = "John Doe",
                NormalizedUserName = "JOHN DOE",
                Email = "john.doe@mav.hu",
                NormalizedEmail = "JOHN.DOE@MAV.HU",
                EmailConfirmed = true,
                BirthDate = new DateTime(1980,1,1),
                SecurityStamp = new Guid().ToString(),
                IsRailwayWorker = true,
                RailwayCompany = "MÁV"
            };

            user.PasswordHash = PasswordGenerate(user, "Password123");

            builder.HasData(user);
        }
        
        private static string PasswordGenerate(ApplicationUser user, string password)
        {
            var passHash = new PasswordHasher<ApplicationUser>();
            return passHash.HashPassword(user, password);
        }
    }
}