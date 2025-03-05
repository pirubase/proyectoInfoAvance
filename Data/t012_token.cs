using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mecanico_plus.Data
{
    public class t012_token
    {
        [Key]
        public int f012_rowid { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime f012_ts { get; set; }

        public string f012_token { get; set; }

        [Display(Name = "Fecha Expiración")]
        public DateTime f012_expiracion { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.Now > f012_expiracion;

        //foranea

        [Display(Name = "Empresa")]
        public int f012_rowid_empresa_o_persona_natural { get; set; }

        [Display(Name = "Empresa")]
        [ForeignKey("f012_rowid_empresa_o_persona_natural")]
        public t002_empresa_o_persona_natural vObjEmpresa { get; set; }

        [Display(Name = "Cita")]
        public int? f012_rowid_cita { get; set; }

        [ForeignKey("f012_rowid_cita")]
        public t009_cita vObjCita { get; set; }
    }
}
