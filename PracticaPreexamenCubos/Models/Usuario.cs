using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticaPreexamenCubos.Models
{
    [Table("Usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id_user")]
        public int Id_user { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("password")]
        public string Password { get; set; }
    }
}
