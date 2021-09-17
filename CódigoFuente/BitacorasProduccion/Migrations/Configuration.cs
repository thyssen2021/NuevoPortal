namespace Portal_2_0.Migrations
{
    using IdentitySample.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<IdentitySample.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(IdentitySample.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            const string email = "admin@admin.com";
            const string userName = "USERS";
            const string password = "Pass123*";
            const string roleName = "Admin";
            const string roleNameU = "Usuarios";

            //crea el Role Admin en caso de no existir
            if (!context.Roles.Any(r => r.Name == roleName))
            {
                var store = new RoleStore<IdentityRole>(context);
                var manager = new RoleManager<IdentityRole>(store);
                var role = new IdentityRole { Name = roleName };
                //crea segundo role
                var roleU = new IdentityRole { Name = roleNameU };
                
                manager.Create(role);
                manager.Create(roleU);
            }

            //crea el usuario en casod e no existir
            if (!context.Users.Any(u => u.UserName == userName))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = userName, Email = email, FechaCreacion = DateTime.Now };
                
                manager.Create(user, password);
                manager.AddToRole(user.Id, roleName);
                manager.AddToRole(user.Id, roleNameU);
            }
        }
    }
}
