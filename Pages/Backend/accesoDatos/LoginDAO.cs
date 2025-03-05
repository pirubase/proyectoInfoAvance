using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.Conexion;
using mecanico_plus.Pages.Backend.genericos;
using Microsoft.EntityFrameworkCore;

namespace mecanico_plus.Pages.Backend.accesoDatos
{
    public class LoginDAO
    {

        private local _context;

        private ConexionDB conexion;

        public IList<t001_usuario> t001_usuario { get; set; }
        public IList<t002_empresa_o_persona_natural> t002_empresa_o_persona_natural { get; set; }

        public LoginDAO()
        {
            conexion = new ConexionDB();

            //creacion del contex
            var optionsBuilder = new DbContextOptionsBuilder<local>();
            optionsBuilder.UseNpgsql(conexion.ObtieneCadenaConexion());

            _context = new local(optionsBuilder.Options);
        }

        /// <summary>
        /// Obtiene usuario con credenciales en estado activo
        /// </summary>
        /// <param name="strUsuario"> Id de usuario</param>
        /// <param name="strPassword">Clave de usuario</param>
        /// <param name="rowidEmpresa">rowid de la empresa a la que pertenece el usuario</param>
        /// <returns></returns>
        public async Task<IList<t001_usuario>> obtieneUsuarioConCredenciales(String correo,
                                                                             String strPassword
                                                                             )
        {
            String claveEncriptada = String.Empty;
            try
            {
                FuncionesGenericas funcionesGenericas = new FuncionesGenericas();
                claveEncriptada = funcionesGenericas.Encrypt(strPassword);

                //Se consulta la existencia del usuario con el id de usuario,
                //el password y el estado sea activo
                t001_usuario = await _context.t001_usuario
                    .Include(t => t.vObjEstado)
                    .Where(t => t.f001_correo_electronico.Equals(correo))
                    .Where(t => t.f001_clave.Equals(claveEncriptada))
                    //.Where(t => t.f001_rowid_empresa_o_persona_natural == rowidEmpresa)
                    .ToListAsync();

                return t001_usuario;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public IList<t001_usuario> obtieneUsuarioPorCorreo(string correoUsuario)
        {
            const int ACTIVO = 1;
            try
            {
                 t001_usuario = _context.t001_usuario
                    .Include(t => t.vObjEstado)
                   .Include(t => t.vObjEmpresa)
                    .Include(t => t.vObjPerfil)
                    .Where(t => t.f001_correo_electronico.Equals(correoUsuario) &&
                                t.f001_rowid_estado == ACTIVO)
                    .ToList();

                return t001_usuario;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public string obtieneClaveUsuarioPorCorreo(string destinatario)
        {
            try
            {
                FuncionesGenericas funcionesGenericas = new FuncionesGenericas();
                t001_usuario = obtieneUsuarioPorCorreo(destinatario);

                if (t001_usuario.Count > 0)
                {
                    return funcionesGenericas.Decrypt(t001_usuario[0].f001_clave);
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
                throw;
            }
        }

        public async Task<bool> persistirUsuario(t001_usuario usuario)
        {
            try
            {
                _context.t001_usuario.Add(usuario);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.ToString());
                return false;
                throw;
            }
        }

        //public async Task<bool> actualizaCadenaConexion(string cadenaConexion)
        //{
        //    try
        //    {
        //        // Inicia una transacción en el contexto
        //        using var transaction = await _context.Database.BeginTransactionAsync();

        //        // Elimina todos los registros de la tabla t072_cadena_conexion
        //        var todosRegistros = _context.t007_cadena_conexion.ToList();
        //        _context.t007_cadena_conexion.RemoveRange(todosRegistros);
        //        await _context.SaveChangesAsync();

        //        // Reinicia la secuencia de la columna auto-incrementable (ej. "t072_cadena_conexion_id_seq")
        //        await _context.Database.ExecuteSqlRawAsync("ALTER SEQUENCE t007_cadena_conexion_f007_rowid_seq RESTART WITH 1;");

        //        // Crea una nueva instancia para la cadena de conexión a insertar
        //        t007_cadena_conexion nuevaCadenaConexion = new t007_cadena_conexion
        //        {
        //            f007_cadena_conexion = cadenaConexion
        //        };

        //        // Agrega el nuevo registro
        //        _context.t007_cadena_conexion.Add(nuevaCadenaConexion);
        //        await _context.SaveChangesAsync();

        //        // Confirma la transacción
        //        await transaction.CommitAsync();

        //        // Retorna true si se ejecuta exitosamente
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Opcional: puedes hacer logging del error si tienes un sistema de logs
        //        Console.WriteLine($"Error actualizando la cadena de conexión: {ex.Message}");

        //        // Retorna false en caso de que ocurra una excepción
        //        return false;
        //    }
        //}

    }
}
