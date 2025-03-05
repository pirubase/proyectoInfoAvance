using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mecanico_plus.Data
{
    public class t001_usuario
    {
        [Key]
        public int f001_rowid { get; set; }

        [Display(Name = "Fecha Creación")]
        public DateTime f001_ts { get; set; }


        [Display(Name = "Id Usuario")]
        [StringLength(maximumLength: 80)]
        public string f001_id_usuario { get; set; }

        [Required(ErrorMessage = "La clave es requerida")]
        [Display(Name = "Contrasena")]
        [StringLength(maximumLength: 512)]
        public string f001_clave { get; set; }

        [Required(ErrorMessage = "El campo nombres es requerido")]
        [Display(Name = "Nombres")]
        [StringLength(maximumLength: 80)]
        public string f001_nombres { get; set; }


        [Required(ErrorMessage = "El campo apellidos es requerido")]
        [Display(Name = "Apellidos")]
        [StringLength(maximumLength: 80)]
        public string f001_apellidos { get; set; }

        [Required(ErrorMessage = "El campo correo lectronico es requerido")]
        [Display(Name = "Correo")]
        [StringLength(maximumLength: 80)]
        public string f001_correo_electronico { get; set; }

        [Required(ErrorMessage = "El numero celuar es requerido")]
        [Display(Name = "Celular")]
        [StringLength(maximumLength: 100)]
        public string f001_numero_celular { get; set; }


        [Display(Name = "Empresa")]
        //Este campo no es visible en cliente
        public int f001_rowid_empresa_o_persona_natural { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        [Display(Name = "Estado")]
        public int f001_rowid_estado { get; set; }

        [Display(Name = "Perfil")]
        [Required(ErrorMessage = "El perfil es requerido")]
        public int f001_rowid_perfil { get; set; }

        [Display(Name = "id de sesion")]
        public string? f001_sesion_id { get; set; }

        [Display(Name = "Cliente")]
        public int? f001_rowid_cliente     { get; set; }

        [Display(Name = "Última Actividad")]
        public DateTime? f001_ultima_actividad { get; set; }

        [Display(Name = "Perfil")]
        [ForeignKey("f001_rowid_perfil")]
        public t004_perfil vObjPerfil { get; set; }

        [Display(Name = "Empresa")]
        [ForeignKey("f001_rowid_empresa_o_persona_natural")]
        public t002_empresa_o_persona_natural vObjEmpresa { get; set; }

        [Display(Name = "Estado")]
        [ForeignKey("f001_rowid_estado")]
        public t008_estados_usuario vObjEstado { get; set; }

        [Display(Name = "Cliente")]
        [ForeignKey("f001_rowid_cliente")]
        public t007_cliente vObjCliente { get; set; }
    }
}
