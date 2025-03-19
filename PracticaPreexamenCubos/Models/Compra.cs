using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticaPreexamenCubos.Models
{
    [Table("COMPRA")]
    public class Compra
    {
        [Key]
        [Column("id_compra")]
        public int Id_compra { get; set; }

        [Column("nombre_cubo")]
        public string Nombre { get; set; }

        [Column("precio")]
        public int precio { get; set; }

        [Column("fechapedido")]
        public DateTime fecha { get; set; }
    }
}
