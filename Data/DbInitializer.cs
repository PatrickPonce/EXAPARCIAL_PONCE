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

            // 🧱 Aplicar migraciones pendientes
            await context.Database.MigrateAsync();

            // 🧩 1️⃣ Crear rol "Coordinador" si no existe
            const string rolCoordinador = "Coordinador";
            if (!await roleManager.RoleExistsAsync(rolCoordinador))
            {
                await roleManager.CreateAsync(new IdentityRole(rolCoordinador));
            }

            // 🧩 2️⃣ Crear usuario Coordinador por defecto
            var email = "coordinador@usmp.edu.pe";
            var password = "Coordinador123*"; // puedes usar tu versión con $
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, rolCoordinador);
                }
            }
            else
            {
                // Asegurar que el usuario tenga el rol Coordinador asignado
                if (!await userManager.IsInRoleAsync(user, rolCoordinador))
                    await userManager.AddToRoleAsync(user, rolCoordinador);
            }

            // 🧩 3️⃣ Insertar cursos iniciales si la tabla está vacía
            if (!context.Cursos.Any())
            {
                context.Cursos.AddRange(
                    new Curso
                    {
                        Codigo = "INF101",
                        Nombre = "Introducción a la Programación",
                        Creditos = 4,
                        CupoMaximo = 30,
                        HorarioInicio = new TimeSpan(8, 0, 0),
                        HorarioFin = new TimeSpan(10, 0, 0),
                        Activo = true
                    },
                    new Curso
                    {
                        Codigo = "INF201",
                        Nombre = "Base de Datos I",
                        Creditos = 3,
                        CupoMaximo = 25,
                        HorarioInicio = new TimeSpan(10, 0, 0),
                        HorarioFin = new TimeSpan(12, 0, 0),
                        Activo = true
                    },
                    new Curso
                    {
                        Codigo = "INF301",
                        Nombre = "Ingeniería de Software",
                        Creditos = 3,
                        CupoMaximo = 20,
                        HorarioInicio = new TimeSpan(14, 0, 0),
                        HorarioFin = new TimeSpan(16, 0, 0),
                        Activo = true
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
