using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Train_Car_Inventory_App.Models;
using Train_Car_Inventory_App.Models.Payloads;

namespace Train_Car_Inventory_App.Services
{
    public interface IUserService
    {
        Task<LoginResponse> LoginAsync(LoginRequest data);

        Task Logout();

        Task<ApplicationUser> RegisterAsync(RegisterRequest registerRequest);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUser> RegisterAsync(RegisterRequest registerRequest)
        {
            var user = new ApplicationUser()
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                EmailConfirmed = true,
                BirthDate = registerRequest.BirthDate,
                IsRailwayWorker = registerRequest.IsRailwayWorker,
                RailwayCompany = registerRequest.RailwayCompany
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return user;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest data)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == data.UserName);
            if (user == null) throw new ApplicationException("INVALID_LOGIN_ATTEMPT");

            var result = await _signInManager.PasswordSignInAsync(data.UserName, data.Password, false, true);
            if (result.Succeeded)
            {
                var token = await GenerateJwtToken(user);
                return new LoginResponse() {Token = token};
            }
            else
            {
                throw new ApplicationException("LOGIN_FAILED");
            }
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretKey_123456"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(30));

            var id = await GetClaimsIdentity(user);
            var token = new JwtSecurityToken("train_car_app", "train_car_app", id.Claims, expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("BirtDate", user.BirthDate.ToString(CultureInfo.InvariantCulture)),
                new Claim("IsRailwayWorker", user.IsRailwayWorker.ToString()),
                new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.AuthTime, DateTime.Now.ToString(CultureInfo.InvariantCulture))
            };

            if (user.IsRailwayWorker)
            {
                claims.Add(new Claim("RailwayCompany", user.RailwayCompany));
            }

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");
            return claimsIdentity;
        }
    }
}