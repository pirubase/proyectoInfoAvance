using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mecanico_plus.Data
{
    public class t013_documento
    {

        [Key]
        public int f013_rowid { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime f013_ts { get; set; }

        [Display(Name = "Id")]
        public int f013_id { get; set; }

        [Display(Name = "Nombre")]
        [StringLength(maximumLength: 80)]
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string f013_nombre { get; set; }

        [Display(Name = "Documento")]
        public byte[] f013_docuemnto { get; set; }

        [Display(Name = "Descripcion")]
        public string f013_descripcion { get; set; }

        [Display(Name = "Categoria")]
        public string f013_categoria { get; set; }


        [Display(Name = "Empresa o persona natural")]
        public int f013_rowid_empresa_o_persona_natural { get; set; }

        [Display(Name = "Historial Médico")]
        public int? f013_rowid_historial_medico { get; set; }

     

        [Display(Name = "Empresa")]
        [ForeignKey("f013_rowid_empresa_o_persona_natural")]
        public t002_empresa_o_persona_natural ObjEmpresa { get; set; }
    }
}
