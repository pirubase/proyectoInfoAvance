using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mecanico_plus.Data
{
    public class t003_permisos
    {
        [Key]
        public int f003_rowid { get; set; }

        [Display(Name = "Fecha registro")]
        public DateTime f003_ts { get; set; }

        [Display(Name = "Id")]
        [Required(ErrorMessage = "El id es requerido.")]
        public int f003_id { get; set; }

        [Display(Name = "Permiso para consultar")]
        [Required(ErrorMessage = "Permiso para consultar es requerido")]
        public bool f003_permiso_consultar { get; set; }

        [Display(Name = "Permiso para crear")]
        [Required(ErrorMessage = "Permiso para crear es requerido")]
        public bool f003_permiso_crear { get; set; }

        [Display(Name = "Permiso para editar")]
        [Required(ErrorMessage = "Permiso para editar es requerido")]
        public bool f003_permiso_editar { get; set; }

        [Display(Name = "Permiso para detalle")]
        [Required(ErrorMessage = "Permiso para detalle es requerido")]
        public bool f003_permiso_detalle { get; set; }

        [Display(Name = "Permiso para eliminar")]
        [Required(ErrorMessage = "Permiso para eliminar es requerido")]
        public bool f003_permiso_eliminar { get; set; }

        [Display(Name = "Permiso menu")]
        [Required(ErrorMessage = "Permiso menu es requerido")]
        public bool f003_permiso_uso_menu { get; set; }


        //foranea

        [Display(Name = "Empresa")]
        public int f003_rowid_empresa_o_persona_natural { get; set; }

        [Display(Name = "Menu")]
        public int f003_rowid_menu { get; set; }

        [Display(Name = "Perfil")]
        public int f003_rowid_perfil {  get; set; }


        [Display(Name = "Empresa")]
        [ForeignKey("f003_rowid_empresa_o_persona_natural")]
        public t002_empresa_o_persona_natural vObjEmpresa { get; set; }

        [Display(Name = "Menu")]
        [ForeignKey("f003_rowid_menu")]
        public t005_menu vObjMenu { get; set; }


        [Display(Name = "Perfil")]
        [ForeignKey("f003_rowid_perfil")]
        public t004_perfil vObjPerfil { get; set; }

    }
}
