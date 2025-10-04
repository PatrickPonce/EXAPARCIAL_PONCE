using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EXAPARCIAL_PONCE.Models;

namespace EXAPARCIAL_PONCE.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            // Crear rol Coordinador si no existe
            if (!await roleManager.RoleExistsAsync("Coordinador"))
                await roleManager.CreateAsync(new IdentityRole("Coordinador"));

            // Crear usuario coordinador
            var userEmail = "coordinador@usmp.edu.pe";
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                user = new IdentityUser { UserName = userEmail, Email = userEmail, EmailConfirmed = true };
                await userManager.CreateAsync(user, "Coordinador123$");
                await userManager.AddToRoleAsync(user, "Coordinador");
            }

            // Insertar cursos si no existen
            if (!context.Cursos.Any())
            {
                context.Cursos.AddRange(
                    new Curso { Codigo = "INF101", Nombre = "Introducción a la Programación", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeSpan(8, 0, 0), HorarioFin = new TimeSpan(10, 0, 0), Activo = true },
                    new Curso { Codigo = "INF201", Nombre = "Base de Datos I", Creditos = 3, CupoMaximo = 25, HorarioInicio = new TimeSpan(10, 0, 0), HorarioFin = new TimeSpan(12, 0, 0), Activo = true },
                    new Curso { Codigo = "INF301", Nombre = "Ingeniería de Software", Creditos = 3, CupoMaximo = 20, HorarioInicio = new TimeSpan(14, 0, 0), HorarioFin = new TimeSpan(16, 0, 0), Activo = true }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
