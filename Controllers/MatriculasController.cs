using EXAPARCIAL_PONCE.Data;
using EXAPARCIAL_PONCE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EXAPARCIAL_PONCE.Controllers
{
    [Authorize]
    public class MatriculasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MatriculasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Inscribirse(int cursoId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Debes iniciar sesión para inscribirte.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            var curso = await _context.Cursos.FindAsync(cursoId);
            if (curso == null)
            {
                TempData["Error"] = "El curso no existe.";
                return RedirectToAction("Index", "Cursos");
            }

            int matriculados = await _context.Matriculas
                .CountAsync(m => m.CursoId == cursoId && m.Estado != "Cancelada");
            if (matriculados >= curso.CupoMaximo)
            {
                TempData["Error"] = "El curso ya alcanzó su cupo máximo.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            bool yaMatriculado = await _context.Matriculas
                .AnyAsync(m => m.CursoId == cursoId && m.UsuarioId == user.Id);
            if (yaMatriculado)
            {
                TempData["Error"] = "Ya estás matriculado en este curso.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            var cursosActuales = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.UsuarioId == user.Id && m.Estado != "Cancelada")
                .ToListAsync();

            bool solapado = cursosActuales.Any(m =>
                curso.HorarioInicio < m.Curso.HorarioFin &&
                curso.HorarioFin > m.Curso.HorarioInicio);

            if (solapado)
            {
                TempData["Error"] = "Tienes un curso en el mismo horario.";
                return RedirectToAction("Details", "Cursos", new { id = cursoId });
            }

            var nuevaMatricula = new Matricula
            {
                CursoId = cursoId,
                UsuarioId = user.Id,
                Estado = "Pendiente",
                FechaRegistro = DateTime.Now
            };

            _context.Matriculas.Add(nuevaMatricula);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Te has inscrito correctamente en el curso. Estado: Pendiente.";
            return RedirectToAction("Details", "Cursos", new { id = cursoId });
        }
    }
}
