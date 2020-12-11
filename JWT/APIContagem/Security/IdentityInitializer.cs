using System;
using Microsoft.AspNetCore.Identity;

namespace APIContagem.Security
{
    public class IdentityInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityInitializer(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_context.Database.EnsureCreated())
            {
                if (!_roleManager.RoleExistsAsync(Roles.ROLE_API_CONTAGEM).Result)
                {
                    var resultado = _roleManager.CreateAsync(
                        new IdentityRole(Roles.ROLE_API_CONTAGEM)).Result;
                    if (!resultado.Succeeded)
                    {
                        throw new Exception(
                            $"Erro durante a criação da role {Roles.ROLE_API_CONTAGEM}.");
                    }
                }

                CreateUser(
                    new ApplicationUser()
                    {
                        UserName = "usr01_apicontagem",
                        Email = "usr01_apicontagem@teste.com.br",
                        EmailConfirmed = true
                    }, "Usr01ApiContagem01!", Roles.ROLE_API_CONTAGEM);

                CreateUser(
                    new ()
                    {
                        UserName = "usr02_apicontagem",
                        Email = "usr02_apicontagem@teste.com.br",
                        EmailConfirmed = true
                    }, "Usr02ApiContagem02!");
            }
        }
        private void CreateUser(
            ApplicationUser user,
            string password,
            string initialRole = null)
        {
            if (_userManager.FindByNameAsync(user.UserName).Result == null)
            {
                var resultado = _userManager
                    .CreateAsync(user, password).Result;

                if (resultado.Succeeded &&
                    !String.IsNullOrWhiteSpace(initialRole))
                {
                    _userManager.AddToRoleAsync(user, initialRole).Wait();
                }
            }
        }
    }
}