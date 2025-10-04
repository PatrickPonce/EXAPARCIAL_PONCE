using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EXAPARCIAL_PONCE.Models;

namespace EXAPARCIAL_PONCE.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Curso> Cursos { get; set; } = null!;
        public DbSet<Matricula> Matriculas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Curso>()
                .HasIndex(c => c.Codigo)
                .IsUnique();

            builder.Entity<Curso>()
                .HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");

            builder.Entity<Curso>()
                .HasCheckConstraint("CK_Curso_Creditos", "Creditos > 0");

            builder.Entity<Matricula>()
                .HasIndex(m => new { m.CursoId, m.UsuarioId })
                .IsUnique();

        }
    }
}
