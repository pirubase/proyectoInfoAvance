using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mecanico_plus.Data
{
    public class t007_cliente
    {
        [Key]
        public int f007_rowid { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime f007_ts { get; set; }

        [Display(Name = "ID")]
        public int f007_id { get; set; }

        // Client Data
        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(80)]
        [Display(Name = "Nombre")]
        public string f007_nombre { get; set; }

        [Required(ErrorMessage = "El apellido es requerido.")]
        [StringLength(80)]
        [Display(Name = "Apellido")]
        public string f007_apellido { get; set; }

        [Required(ErrorMessage = "El correo es requerido.")]
        [StringLength(80)]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        [Display(Name = "Correo")]
        public string f007_correo { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido.")]
        [Phone(ErrorMessage = "El teléfono no es válido.")]
        [Display(Name = "Teléfono")]
        public string f007_telefono { get; set; }

        [Required(ErrorMessage = "La dirección es requerida.")]
        [StringLength(80)]
        [Display(Name = "Dirección")]
        public string f007_direccion { get; set; }

        [Display(Name = "Mecánico de Familia")]
        public int? f007_rowid_mecanico_familia { get; set; }


        [Display(Name = "Mecánico de Familia")]
        [ForeignKey("f007_rowid_mecanico_familia")]
        public t006_mecanico vObjMecanicoFamilia { get; set; }

        [Display(Name = "Empresa o Persona Natural")]
        public int f007_rowid_empresa_o_persona_natural { get; set; }

        [ForeignKey("f007_rowid_empresa_o_persona_natural")]
        [Display(Name = "Empresa")]
        public t002_empresa_o_persona_natural vObjEmpresa { get; set; }
    }
}
