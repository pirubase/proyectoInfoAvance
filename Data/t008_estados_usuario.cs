using System.ComponentModel.DataAnnotations;

namespace mecanico_plus.Data
{
    public class t008_estados_usuario
    {

        [Key]
        public int f008_rowid { get; set; }

        [Display(Name = "Id")]
        public int f008_id { get; set; }

        [Display(Name = "Estado")]
        [StringLength(maximumLength: 80)]
        public string f008_nombre_estado { get; set; }
    }
}
