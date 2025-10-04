using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EXAPARCIAL_PONCE.Data;
using EXAPARCIAL_PONCE.Models;

namespace EXAPARCIAL_PONCE.Controllers
{
    public class CursosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CursosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? nombre, int? creditosMin, int? creditosMax, TimeSpan? horaInicio, TimeSpan? horaFin)
        {
            var query = _context.Cursos.AsQueryable();

            query = query.Where(c => c.Activo);

            if (!string.IsNullOrWhiteSpace(nombre))
                query = query.Where(c => c.Nombre.Contains(nombre));

            if (creditosMin.HasValue)
                query = query.Where(c => c.Creditos >= creditosMin.Value);
            if (creditosMax.HasValue)
                query = query.Where(c => c.Creditos <= creditosMax.Value);

            if (horaInicio.HasValue)
                query = query.Where(c => c.HorarioInicio >= horaInicio.Value);
            if (horaFin.HasValue)
                query = query.Where(c => c.HorarioFin <= horaFin.Value);

            var cursos = await query.OrderBy(c => c.Nombre).ToListAsync();
            return View(cursos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetInt32("UltimoCursoId", curso.Id);
            HttpContext.Session.SetString("UltimoCursoNombre", curso.Nombre);

            return View(curso);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarCurso(Curso curso)
        {
            if (curso.Creditos < 0)
                ModelState.AddModelError("Creditos", "Los créditos no pueden ser negativos.");

            if (curso.HorarioFin <= curso.HorarioInicio)
                ModelState.AddModelError("HorarioFin", "El horario de fin no puede ser anterior o igual al inicio.");

            if (!ModelState.IsValid)
                return View(curso);

            _context.Update(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
