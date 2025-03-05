namespace mecanico_plus.Pages.Backend.ModeloConDatos
{
    public class MedioDePago
    {


        public string Id { get; set; }
        public string NombreMetodoDePago { get; set; }



        // Constructor sin parámetros
        public MedioDePago() { }

        // Constructor con parámetros
        public MedioDePago(string id, string nombreMetodoDePago)
        {
            Id = id;
            NombreMetodoDePago = nombreMetodoDePago;

        }

        // Método estático que retorna una lista de opciones de TipoCuenta
        public static IList<MedioDePago> RetornaOpciones()
        {
            return new List<MedioDePago>
            {
                new MedioDePago { Id = "1", NombreMetodoDePago = "Efectivo"},
                new MedioDePago { Id = "2", NombreMetodoDePago = "Tarjeta credito"},
                new MedioDePago { Id = "3", NombreMetodoDePago = "Tarjeta debito"},
                new MedioDePago { Id = "4", NombreMetodoDePago = "Cheque"},
                new MedioDePago { Id = "5", NombreMetodoDePago = "Otro"}
               

            };
        }
        public static string ObtenerNombrePorId(string id)
        {
            var opciones = RetornaOpciones();
            var novedad = opciones.FirstOrDefault(tc => tc.Id == id);
            return novedad != null ? novedad.NombreMetodoDePago : null; 
        }
    }
}
