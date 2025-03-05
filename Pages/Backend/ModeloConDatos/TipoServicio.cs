namespace mecanico_plus.Pages.Backend.ModeloConDatos
{
    public class TipoServicio
    {
        public string Id { get; set; }
        public string NombreTipoServicio { get; set; }

        // Constructor sin parámetros
        public TipoServicio() { }

        // Constructor con parámetros
        public TipoServicio(string id, string nombreTipoServicio)
        {
            Id = id;
            NombreTipoServicio = nombreTipoServicio;
        }

        // Método estático que retorna una lista de opciones de TipoServicio
        public static IList<TipoServicio> RetornaOpciones()
        {
            return new List<TipoServicio>
            {
                new TipoServicio { Id = "1", NombreTipoServicio = "Cambio de aceite" },
                new TipoServicio { Id = "2", NombreTipoServicio = "Alineación" },
                new TipoServicio { Id = "3", NombreTipoServicio = "Pintura" },
                new TipoServicio { Id = "4", NombreTipoServicio = "Reparación de frenos" },
                new TipoServicio { Id = "5", NombreTipoServicio = "Cambio de neumáticos" },
                new TipoServicio { Id = "6", NombreTipoServicio = "Revisión general" },
                new TipoServicio { Id = "7", NombreTipoServicio = "Reparación de motor" },
                new TipoServicio { Id = "8", NombreTipoServicio = "Reparación de transmisión" },
                new TipoServicio { Id = "9", NombreTipoServicio = "Reparación de suspensión" },
                new TipoServicio { Id = "10", NombreTipoServicio = "Diagnóstico electrónico" },
                new TipoServicio { Id = "11", NombreTipoServicio = "Cambio de batería" },
                new TipoServicio { Id = "12", NombreTipoServicio = "Reparación de aire acondicionado" },
                new TipoServicio { Id = "13", NombreTipoServicio = "Reparación de dirección" },
                new TipoServicio { Id = "14", NombreTipoServicio = "Reparación de escape" },
                new TipoServicio { Id = "15", NombreTipoServicio = "Reparación de radiador" }
            };
        }

        public static string ObtenerNombrePorId(string id)
        {
            var opciones = RetornaOpciones();
            var servicio = opciones.FirstOrDefault(tc => tc.Id == id);
            return servicio != null ? servicio.NombreTipoServicio : null;
        }
    }
}
