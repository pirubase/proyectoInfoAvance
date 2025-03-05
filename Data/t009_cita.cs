using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using mecanico_plus.Pages.Backend.ModeloConDatos;

namespace mecanico_plus.Data
{
    public class t009_cita
    {
        [Key]
        public int f009_rowid { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime f009_ts { get; set; }

        [Display(Name = "Fecha Asignación")]
        public DateTime f009_hora { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida.")]
        [Display(Name = "Fecha Inicio")]
        public DateTime f009_fecha_inicio { get; set; }

        [Required(ErrorMessage = "La fecha de finalización es requerida.")]
        [Display(Name = "Fecha Finalización")]
        public DateTime f009_fecha_finalizacion { get; set; }

        [Required(ErrorMessage = "El nombre del servicio es requerido.")]
        [Display(Name = "Servicio")]
        public int f009_rowid_servicio { get; set; }

        [Required(ErrorMessage = "La descripción del servicio es requerida.")]
        [StringLength(500)]
        [Display(Name = "Descripción")]
        public string f009_descripcion { get; set; }

        [Required(ErrorMessage = "El estado del servicio es requerido.")]
        [StringLength(50)]
        [Display(Name = "Estado")]
        public string f009_estado { get; set; }

        [Required(ErrorMessage = "El ID del mecánico es requerido.")]
        [Display(Name = "ID del Mecánico")]
        public int f009_rowid_mecanico { get; set; }

        public int f009_rowid_especialidad { get; set; }

        public int f009_rowid_cliente { get; set; }

        public int f009_rowid_empresa_o_persona_natural { get; set; }

        [ForeignKey("f009_rowid_empresa_o_persona_natural")]
        [Display(Name = "Empresa o persona natural")]
        public t002_empresa_o_persona_natural vObjEmpresa { get; set; }

        [ForeignKey("f009_rowid_servicio")]
        [Display(Name = "Servicio")]
        public t014_servicio vObjServicio { get; set; }

        [ForeignKey("f009_rowid_cliente")]
        [Display(Name = "Cliente")]
        public t007_cliente vObjCliente { get; set; }

        [ForeignKey("f009_rowid_especialidad")]
        [Display(Name = "Especialidad")]
        public t010_vehiculo vObjEspecialidad { get; set; }

        [ForeignKey("f009_rowid_mecanico")]
        [Display(Name = "Mecánico")]
        public t006_mecanico vObjMecanico { get; set; }

        [Display(Name = "Usuario Creador")]
        public int f009_rowid_usuario_creador { get; set; }

        [Display(Name = "Usuario Aprobador")]
        public int? f009_rowid_usuario_aprobador { get; set; }

        [ForeignKey("f009_rowid_usuario_creador")]
        public t001_usuario vObjUsuarioCreador { get; set; }

        [ForeignKey("f009_rowid_usuario_aprobador")]
        public t001_usuario vObjUsuarioAprobador { get; set; }
    }
}
