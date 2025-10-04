using EXAPARCIAL_PONCE.Data;
using EXAPARCIAL_PONCE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EXAPARCIAL_PONCE.Controllers
{
    [Authorize(Roles = "Coordinador")]
    public class CoordinadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoordinadorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Coordinador
        public async Task<IActionResult> Index()
        {
            var cursos = await _context.Cursos.ToListAsync();
            return View(cursos);
        }

        // GET: /Coordinador/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: /Coordinador/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Curso curso)
        {
            if (curso.HorarioFin <= curso.HorarioInicio)
            {
                ModelState.AddModelError("", "El horario de fin debe ser mayor al inicio.");
            }

            if (curso.Creditos <= 0)
            {
                ModelState.AddModelError("", "Los créditos deben ser mayores a 0.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(curso);
        }

        // GET: /Coordinador/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
                return NotFound();

            return View(curso);
        }

        // POST: /Coordinador/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Curso curso)
        {
            if (id != curso.Id)
                return BadRequest();

            if (curso.HorarioFin <= curso.HorarioInicio)
                ModelState.AddModelError("", "El horario de fin debe ser mayor al inicio.");

            if (curso.Creditos <= 0)
                ModelState.AddModelError("", "Los créditos deben ser mayores a 0.");

            if (ModelState.IsValid)
            {
                _context.Update(curso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(curso);
        }

        // POST: /Coordinador/Desactivar/5
        [HttpPost]
        public async Task<IActionResult> Desactivar(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
                return NotFound();

            curso.Activo = !curso.Activo;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: /Coordinador/Matriculas/5
        public async Task<IActionResult> Matriculas(int id)
        {
            var curso = await _context.Cursos.Include(c => c.Id).FirstOrDefaultAsync(c => c.Id == id);
            var matriculas = await _context.Matriculas
                .Include(m => m.Curso)
                .Where(m => m.CursoId == id)
                .ToListAsync();

            ViewBag.CursoNombre = curso?.Nombre ?? "Curso";
            ViewBag.CursoId = id;

            return View(matriculas);
        }

        // POST: /Coordinador/Confirmar
        [HttpPost]
        public async Task<IActionResult> Confirmar(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null)
                return NotFound();

            matricula.Estado = "Confirmada";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Matriculas), new { id = matricula.CursoId });
        }

        // POST: /Coordinador/Cancelar
        [HttpPost]
        public async Task<IActionResult> Cancelar(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null)
                return NotFound();

            matricula.Estado = "Cancelada";
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Matriculas), new { id = matricula.CursoId });
        }
    }
}
