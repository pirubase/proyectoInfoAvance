using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mecanico_plus.Data
{
    public class t006_mecanico
    {
        [Key]
        public int f006_rowid { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime f006_ts { get; set; }

        // Mechanic Data
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(80)]
        [Display(Name = "Nombre")]
        public string f006_nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido.")]
        [StringLength(80)]
        [Display(Name = "Apellido")]
        public string f006_apellido { get; set; }

        [Required(ErrorMessage = "El correo es requerido.")]
        [StringLength(80)]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        [Display(Name = "Correo")]
        public string f006_correo { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido.")]
        [Phone(ErrorMessage = "El teléfono no es válido.")]
        [Display(Name = "Teléfono")]
        public string f006_telefono { get; set; }

        [Required(ErrorMessage = "La dirección es requerida.")]
        [StringLength(80)]
        [Display(Name = "Dirección")]
        public string f006_direccion { get; set; }

        [Required(ErrorMessage = "Los años de experiencia son requeridos.")]
        [Range(0, 100, ErrorMessage = "Los años de experiencia no son válidos.")]
        [Display(Name = "Años de Experiencia")]
        public int f006_años_experiencia { get; set; }

        [Required(ErrorMessage = "La tarifa por hora es requerida.")]
        [Range(0, double.MaxValue, ErrorMessage = "La tarifa por hora no es válida.")]
        [Display(Name = "Tarifa por Hora")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal f006_tarifa_hora { get; set; }

        [Display(Name = "Empresa o Persona Natural")]
        public int f006_rowid_empresa_o_persona_natural { get; set; }

        [ForeignKey("f006_rowid_empresa_o_persona_natural")]
        [Display(Name = "Empresa o Persona Natural")]
        public t002_empresa_o_persona_natural vObjEmpresa { get; set; }

        [NotMapped]
        public string NombreCompleto => $"Dr. {f006_nombre} {f006_apellido}";
    }
}
