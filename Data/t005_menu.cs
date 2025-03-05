using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mecanico_plus.Data
{
    public class t005_menu
    {

        [Key]
        public int f005_rowid { get; set; }

        [Display(Name = "Fecha registro")]
        public DateTime f005_ts { get; set; }

        [Display(Name = "Id")]
        [Required(ErrorMessage = "El id es requerido.")]
        public int f005_id { get; set; }

        [Display(Name = "nombre")]
        [StringLength(maximumLength: 80)]
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string f005_nombre { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(maximumLength: 255)]
        [Required(ErrorMessage = "La descripciòn es requerida.")]
        public string f005_descripcion { get; set; }

   
    }
}
