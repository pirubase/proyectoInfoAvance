using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mecanico_plus.Data
{
    public class t016_auditoria_mecanico
    {
        [Key]
        public int f016_rowid { get; set; }
        
        [Display(Name = "Fecha Creación Registro")]
        public DateTime f016_ts { get; set; }
        
        [Display(Name = "Fecha/Hora Inicio")]
        public DateTime f016_fecha_inicio { get; set; }
        
        [Display(Name = "Fecha/Hora Final")]
        public DateTime f016_fecha_finalizacion { get; set; }
        
        [Display(Name = "Descripción")]
        [StringLength(150)]
        public string f016_descripcion { get; set; }
        
        [Display(Name = "ID del Mecánico")]
        public int f016_rowid_mecanico { get; set; }

        [Display(Name = "Empresa")]
        public int  f016_rowid_empresa_o_persona_natural { get; set; }

        [ForeignKey("f016_rowid_empresa_o_persona_natural")]
        public t002_empresa_o_persona_natural vObjEmpresa { get; set; }

        [ForeignKey("f016_rowid_mecanico")]
        public t006_mecanico vObjMecanico { get; set; }
    }
}