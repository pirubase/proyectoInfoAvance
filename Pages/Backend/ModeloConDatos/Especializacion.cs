namespace mecanico_plus.Pages.Backend.ModeloConDatos
{
    public class Especializacion
    {

        public string Id { get; set; }
        public string NombreTipoEspecializacion { get; set; }



        // Constructor sin parámetros
        public Especializacion() { }

        // Constructor con parámetros
        public Especializacion(string id, string nombreTipoEspecializacion)
        {
            Id = id;
            NombreTipoEspecializacion = nombreTipoEspecializacion;

        }

        // Método estático que retorna una lista de opciones de TipoCuenta
        public static IList<Especializacion> RetornaOpciones()
        {
            return new List<Especializacion>
            {
                new Especializacion { Id = "1", NombreTipoEspecializacion = "Ginecología"},
                new Especializacion { Id = "2", NombreTipoEspecializacion = "Obstetricia"},
                new Especializacion { Id = "3", NombreTipoEspecializacion = "Psiquiatría"},
                new Especializacion { Id = "4", NombreTipoEspecializacion = "Dermatología"},
                new Especializacion { Id = "5", NombreTipoEspecializacion = "Medicina Física y Rehabilitación"},
                new Especializacion { Id = "6", NombreTipoEspecializacion = "Terapia Ocupacional"},
                new Especializacion { Id = "7", NombreTipoEspecializacion = "Radioterapia"},
                new Especializacion { Id = "8", NombreTipoEspecializacion = "Medicina Estética"},
                new Especializacion { Id = "9", NombreTipoEspecializacion = "Otorrinolaringología"},
                new Especializacion { Id = "10", NombreTipoEspecializacion = "Anestesiología"},
                new Especializacion { Id = "11", NombreTipoEspecializacion = "Odontología general"},
                new Especializacion { Id = "12", NombreTipoEspecializacion = "Ortodoncia"},
                new Especializacion { Id = "13", NombreTipoEspecializacion = "Endodoncia"},
                new Especializacion { Id = "14", NombreTipoEspecializacion = "Periodoncia"},
                new Especializacion { Id = "15", NombreTipoEspecializacion = "Cirugía bucal"},
                new Especializacion { Id = "16", NombreTipoEspecializacion = "Odontopediatría"},
                new Especializacion { Id = "17", NombreTipoEspecializacion = "Estomatología"},
                new Especializacion { Id = "18", NombreTipoEspecializacion = "Prostodoncia"},
                new Especializacion { Id = "19", NombreTipoEspecializacion = "Odontología estética"},
                new Especializacion { Id = "20", NombreTipoEspecializacion = "Radiología dental"},
                 new Especializacion { Id = "21", NombreTipoEspecializacion = "Temporomandibular y Dolor Orofacial"},

            };
        }
        public static string ObtenerNombrePorId(string id)
        {
            var opciones = RetornaOpciones();
            var novedad = opciones.FirstOrDefault(tc => tc.Id == id);
            return novedad != null ? novedad.NombreTipoEspecializacion : null;
        }
    }
}

