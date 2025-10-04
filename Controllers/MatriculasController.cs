using EXAPARCIAL_PONCE.Data;
using EXAPARCIAL_PONCE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EXAPARCIAL_PONCE.Controllers
{
    [Authorize] // el usuario debe estar autenticado
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Acción para inscribirse a un curso
        [HttpPost]
        public async Task<IActionResult> Inscribirse(int cursoId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var curso = await _context.Cursos.FirstOrDefaultAsync(c => c.Id == cursoId && c.Activo);
            if (curso == null)
                return NotFound("El curso no existe o está inactivo.");

            // Validación: ya matriculado
            bool yaMatriculado = await _context.Matriculas.AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == user.Id);
            if (yaMatriculado)
            {
                TempData["Error"] = "Ya estás matriculado en este curso.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            // Validación: cupo máximo
            int inscritos = await _context.Matriculas.CountAsync(m => m.CursoId == cursoId);
            if (inscritos >= curso.CupoMaximo)
            {
                TempData["Error"] = "El curso ya alcanzó su cupo máximo.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            // Validación: conflicto de horario
            var matriculasUsuario = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.UsuarioId == user.Id)
                .ToListAsync();

            bool conflictoHorario = matriculasUsuario.Any(m =>
                (curso.HorarioInicio < m.Curso.HorarioFin && curso.HorarioFin > m.Curso.HorarioInicio));

            if (conflictoHorario)
            {
                TempData["Error"] = "Ya tienes una matrícula en un curso que se cruza en horario.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            // Si todo OK -> registrar matrícula
            var matricula = new Matricula
            {
                CursoId = cursoId,
                UsuarioId = user.Id,
                Estado = "Pendiente",
                FechaRegistro = DateTime.Now
            };

            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Te has inscrito exitosamente en el curso.";
            return RedirectToAction("Details", "Cursos", new { id = cursoId });
        }
    }
}
