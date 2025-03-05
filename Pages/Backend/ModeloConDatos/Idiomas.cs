namespace mecanico_plus.Pages.Backend.ModeloConDatos
{
    public class Idiomas
    {

        public string Id { get; set; }
        public string NombreIdiomas { get; set; }



        // Constructor sin parámetros
        public Idiomas() { }

        // Constructor con parámetros
        public Idiomas(string id, string nombreIdiomas)
        {
            Id = id;
            NombreIdiomas = nombreIdiomas;

        }

        // Método estático que retorna una lista de opciones de TipoCuenta
        public static IList<Idiomas> RetornaOpciones()
        {
            return new List<Idiomas>
            {
                new Idiomas { Id = "1", NombreIdiomas = "Español" },
                new Idiomas { Id = "2", NombreIdiomas = "Inglés" },
                new Idiomas { Id = "3", NombreIdiomas = "Francés" },
                new Idiomas { Id = "4", NombreIdiomas = "Alemán" },
                new Idiomas { Id = "5", NombreIdiomas = "Italiano" },
                new Idiomas { Id = "6", NombreIdiomas = "Portugués" },
                new Idiomas { Id = "7", NombreIdiomas = "Ruso" },
                new Idiomas { Id = "8", NombreIdiomas = "Chino Mandarín" },
                new Idiomas { Id = "9", NombreIdiomas = "Japonés" },
                new Idiomas { Id = "10", NombreIdiomas = "Árabe" },
                new Idiomas { Id = "11", NombreIdiomas = "Coreano" },
                new Idiomas { Id = "12", NombreIdiomas = "Hindi" },
                new Idiomas { Id = "13", NombreIdiomas = "Turco" },
                new Idiomas { Id = "14", NombreIdiomas = "Griego" },
                new Idiomas { Id = "15", NombreIdiomas = "Sueco" },
                new Idiomas { Id = "16", NombreIdiomas = "Noruego" },
                new Idiomas { Id = "17", NombreIdiomas = "Danés" },
                new Idiomas { Id = "18", NombreIdiomas = "Polaco" },
                new Idiomas { Id = "19", NombreIdiomas = "Holandés" },
                new Idiomas { Id = "20", NombreIdiomas = "Checo" },
                new Idiomas { Id = "21", NombreIdiomas = "Tailandés" },
                new Idiomas { Id = "22", NombreIdiomas = "Vietnamita" },
                new Idiomas { Id = "23", NombreIdiomas = "Hebreo" },
                new Idiomas { Id = "24", NombreIdiomas = "Finlandés" },
                new Idiomas { Id = "25", NombreIdiomas = "Indonesio" },
                new Idiomas { Id = "26", NombreIdiomas = "Bengalí" },
                new Idiomas { Id = "27", NombreIdiomas = "Urdu" },
                new Idiomas { Id = "28", NombreIdiomas = "Tamil" },
                new Idiomas { Id = "29", NombreIdiomas = "Telugu" },
                new Idiomas { Id = "30", NombreIdiomas = "Maratí" },
                new Idiomas { Id = "31", NombreIdiomas = "Punjabi" },
                new Idiomas { Id = "32", NombreIdiomas = "Ucraniano" },
                new Idiomas { Id = "33", NombreIdiomas = "Romanés" },
                new Idiomas { Id = "34", NombreIdiomas = "Húngaro" },
                new Idiomas { Id = "35", NombreIdiomas = "Afrikáans" },
                new Idiomas { Id = "36", NombreIdiomas = "Swahili" },
                new Idiomas { Id = "37", NombreIdiomas = "Tagalo" },
                new Idiomas { Id = "38", NombreIdiomas = "Persa (Farsi)" },
                new Idiomas { Id = "39", NombreIdiomas = "Malayalam" },
                new Idiomas { Id = "40", NombreIdiomas = "Catalán" }


            };
        }
        public static string ObtenerNombrePorId(string id)
        {
            var opciones = RetornaOpciones();
            var novedad = opciones.FirstOrDefault(tc => tc.Id == id);
            return novedad != null ? novedad.NombreIdiomas : null;
        }
    }
}
