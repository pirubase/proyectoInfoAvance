using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mecanico_plus.Data
{
    public class t014_servicio
    {
        [Key]
        public int f014_rowid { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime f014_ts { get; set; }

        [Display(Name = "Id")]
        public int f014_id { get; set; }

        [Display(Name = "Nombre")]
        public string f014_nombre { get; set; }

        [Display(Name = "Valor")]
        public decimal f014_valor { get; set; }

        [Display(Name = "Empresa o persona natural")]
        public int f014_rowid_empresa_o_persona_natural { get; set; }

        [ForeignKey("f014_rowid_empresa_o_persona_natural")]
        public t002_empresa_o_persona_natural vObjEmpresa { get; set; }
    }
}
