using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mecanico_plus.Data
{
    public class t004_perfil
    {
        [Key]
        public int f004_rowid { get; set; }

        [Display(Name = "Fecha registro")]
        [Required(ErrorMessage = "La fecha registro es requerida.")]
        public DateTime f004_ts { get; set; }

        [Display(Name = "Id")]
        [Required(ErrorMessage = "El id son requeridos.")]
        public int f004_id { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(maximumLength: 80)]
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string f004_nombre { get; set; }

        [Display(Name = "Descripcion")]
        [StringLength(maximumLength: 255)]
        [Required(ErrorMessage = "La descripcion es requerida.")]
        public string f004_descripcion { get; set; }

        

    }
}
