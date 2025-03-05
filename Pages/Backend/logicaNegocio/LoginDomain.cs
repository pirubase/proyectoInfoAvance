using System.Text.RegularExpressions;
using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.accesoDatos;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Login;

namespace mecanico_plus.Pages.Backend.logicaNegocio
{
    public class LoginDomain
    {


        public IList<t001_usuario> t001_usuario { get; set; }
        public IList<t002_empresa_o_persona_natural> t002_empresas { get; set; }

        /// <summary>
        /// Valida el login realizado por un usuario.
        /// </summary>
        /// <param name="strUsuario">Id usuario</param>
        /// <param name="strPassword">Clave usuario</param>
        /// <returns>true cuando el login el id de usuari0,
        /// clave son correctos y su estado es activo, de lo contrario retorna false</returns>
        public async Task<bool> validaLogin(ModeloLogin modeloLogin)
        {
            bool loginExitosoUsuario = false;
            bool loginExitosoEmpresa = true;

            try
            {
                LoginDAO loginDAO = new LoginDAO();
                //EmpresasDAO empresasDAO = new EmpresasDAO();

                //// Se obtiene la empresa por NIT a la que pertenece el usuario
                //t002_empresas = await empresasDAO.obtieneEmpresaPorNIT(modeloLogin.identificacionEmpresa);

                // Se obtiene el usuario
                t001_usuario = await loginDAO.obtieneUsuarioConCredenciales(modeloLogin.correoElectronico,
                                                                            modeloLogin.pubStrClave)
                                                                           /* t002_empresas[0].f002_rowid)*/;

                //si encontro registro
                if (t001_usuario.Count > 0)
                {
                    //si su estado es activo
                    if (t001_usuario[0].vObjEstado.f008_id == Costantes.ESTADO_ACTIVO)
                    {
                        loginExitosoUsuario = true;
                    }
                }

                // Validar si esto es necesario
                //await empresasDAO.actualizaEmpresaSeleccionada(t002_empresas[0].f002_rowid, modeloLogin.correoElectronico);

                return loginExitosoUsuario && loginExitosoEmpresa;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:" + ex.ToString());
                return false;
                throw;
            }
        }

        public bool correoUsuarioExistente(string correoUsuario)
        {
            bool resultado = true;

            LoginDAO loginDAO = new LoginDAO();
            t001_usuario = loginDAO.obtieneUsuarioPorCorreo(correoUsuario);

            if (t001_usuario == null)
            {
                return false;
            }

            if (t001_usuario.Count == 0)
            {
                return false;
            }
            else if (t001_usuario.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public async Task<bool> actualizaCadenaConexion(string cadenaConexion)
        //{
        //    LoginDAO loginDAO = new LoginDAO();
        //    return await loginDAO.actualizaCadenaConexion(cadenaConexion);
        //}

        public bool validaNombreApellido(string nombre, string apellido)
        {
            string pattern = @"^[a-zA-Z]+$";
            return Regex.IsMatch(nombre, pattern) && Regex.IsMatch(apellido, pattern);
        }
        
    }
}
