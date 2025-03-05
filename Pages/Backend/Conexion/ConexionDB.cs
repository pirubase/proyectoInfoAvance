using mecanico_plus.Pages.Backend.constantes;
using Npgsql;

namespace mecanico_plus.Pages.Backend.Conexion
{
    public class ConexionDB : Iconexion
    {

        //obtiene la cadena de conexion
        public string ObtieneCadenaConexion()
        {
            try
            {
                var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfiguration configuration = builder.Build();
                    
                return configuration.GetConnectionString(Costantes.LABEL_CONECTION_STRING);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public NpgsqlConnection conexionBdDirecta()
        {
            return new NpgsqlConnection(ObtieneCadenaConexion());
        }
    }
}
