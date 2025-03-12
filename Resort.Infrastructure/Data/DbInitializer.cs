using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resort.Application.Repository;
using Resort.Application.Utility;
using Resort.Domain.Models;

namespace Resort.Infrastructure.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public void Initialize()
        {
            try
            {

                if (_context.Database.GetPendingMigrations().Count() > 0) 
                {
                    _context.Database.Migrate();
                }

                if (!_roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).Wait(); // this way works to create role
                    _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult(); // also this way works wells   
                
                    _userManager.CreateAsync(new ApplicationUser
                        {
                        UserName = "AdminUser@Resort.com",
                        Email = "AdminUser@Resort.com",
                        Name = "Admin User",
                        NormalizedUserName = "ADMINUSER@RESORT.COM",
                        NormalizedEmail = "ADMINUSER@RESORT.COM",
                        PhoneNumber = "1112223333",
                    }, "Admin123!").GetAwaiter().GetResult();

                    ApplicationUser user = _context.ApplicationUser.FirstOrDefault(u => u.Email == "AdminUser@Resort.com");
                    _userManager.AddToRoleAsync(user, SD.AdminRole).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex) 
            {
                throw;
            }
        }
    }
}
