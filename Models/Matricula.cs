using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EXAPARCIAL_PONCE.Models
{
    public enum EstadoMatricula
    {
        Pendiente,
        Confirmada,
        Cancelada
    }

    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        public int CursoId { get; set; }

        [ForeignKey(nameof(CursoId))]
        public Curso Curso { get; set; } = null!;

        [Required]
        public string UsuarioId { get; set; } = null!;

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Required]
        public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;
    }
}
