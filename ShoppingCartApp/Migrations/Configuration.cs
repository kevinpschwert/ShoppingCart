namespace ShoppingCartApp.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web;


    internal sealed class Configuration : DbMigrationsConfiguration<ShoppingCartApp.Models.ApplicationDbContext>
    {

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            var userManager = new UserManager<ApplicationUser>(
             new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "kevinpschwert@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "kevinpschwert@gmail.com",
                    Email = "kevinpschwert@gmail.com",
                    FirstName = "Kevin",
                    LastName = "Schwert",
                    DisplayName = "kschwert"
                }, "1th@nkGod");
            }

            var userId = userManager.FindByEmail("kevinpschwert@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");
        }
    }
}