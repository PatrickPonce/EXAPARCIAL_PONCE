using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EXAPARCIAL_PONCE.Models
{
    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        public int CursoId { get; set; }

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmada, Cancelada

        [ForeignKey(nameof(CursoId))]
        public Curso? Curso { get; set; }
    }
}
