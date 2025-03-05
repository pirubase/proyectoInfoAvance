using System.ComponentModel.DataAnnotations;

namespace mecanico_plus.Data
{
    public class t002_empresa_o_persona_natural
    {

        [Key]
        public int f002_rowid { get; set; }

        [Display(Name = "Fecha actualización")]
        [Required(ErrorMessage = "La fecha de actualización es requerida")]
        public DateTime f002_ts { get; set; }

        [Display(Name = "Razon social")]
        [StringLength(maximumLength: 100)]
        [Required(ErrorMessage = "La razon social es requerida.")]
        public string f002_razon_social { get; set; }

        [Display(Name = "NIT o cedula")]
        [StringLength(maximumLength: 100)]
        [Required(ErrorMessage = "El Nit es requerido.")]
        public string f002_nit { get; set; }

        [Display(Name = "Alcance")]
        [StringLength(maximumLength: 50)]
        [Required(ErrorMessage = "El alcance es requerido.")]
        public string f002_alcance { get; set; }

        [Display(Name = "Cantidad empleados")]
        [Required(ErrorMessage = "La cantidad empleados es requerida.")]
        public int f002_cantidad_empleados { get; set; }

    }
}
