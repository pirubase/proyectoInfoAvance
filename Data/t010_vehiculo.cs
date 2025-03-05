using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mecanico_plus.Data
{
    public class t010_vehiculo
    {
        [Key]
        public int f010_rowid { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime f010_ts { get; set; }

 [Display(Name = "Id")]
        public int f010_id { get; set;}

        [Display(Name = "Marca")]
        [StringLength(maximumLength:255)]
        public string f010_nombre { get; set; }

        [Display(Name = "Modelo")]
        [StringLength(maximumLength: 500)]
        public string? f010_descripcion { get; set; }

        [Display(Name = "Cliente")]
        public int f010_rowid_cliente { get; set; }

        [Display(Name = "Cliente")]
        [ForeignKey("f010_rowid_cliente")]
        public t007_cliente vObjCliente { get; set; }

        [Display(Name = "Empresa")]
        public int f010_rowid_empresa_o_persona_natural { get; set; }


        [Display(Name = "Empresa")]
        [ForeignKey("f010_rowid_empresa_o_persona_natural")]
        public t002_empresa_o_persona_natural vObjEmpresa { get; set; }

    }
}
