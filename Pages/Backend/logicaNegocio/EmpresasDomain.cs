using mecanico_plus.Pages.Backend.accesoDatos;

namespace mecanico_plus.Pages.Backend.logicaNegocio
{
    public class EmpresasDomain
    {

        /// <summary>
        /// Actualiza la empresa seleccionada. Es el campo f001_empresa_seleccionada (int4) de la tabla t001_usuario.
        /// </summary>
        /// <param name="empresaSeleccionada">empresa seleccionada</param>
        /// <param name="idusuario">id usuario</param>
        /// <returns></returns>
        public async Task<bool> actualizaEmpresaSeleccionada(String idusuario, int empresaSeleccionada)
        {
            try
            {
                EmpresasDAO empresasDAO = new EmpresasDAO();
                return await empresasDAO.actualizaEmpresaSeleccionada(empresaSeleccionada, idusuario);
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public async Task<bool> actualizaEmpresaSeleccionadaPorNit(String correoUsuario, string nit)
        {
            try
            {
                EmpresasDAO empresasDAO = new EmpresasDAO();
                return await empresasDAO.actualizaEmpresaSeleccionadaPorNit(nit, correoUsuario);
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// Obtiene la empresa seleccionada. Es el campo f001_empresa_seleccionada (int4) de la tabla t001_usuario.
        /// </summary>
        /// <returns></returns>
        public async Task<int> obtieneEmpresaSeleccionada(String usuario)
        {
            try
            {
                EmpresasDAO empresasDAO = new EmpresasDAO();
                return await empresasDAO.obtieneEmpresaSeleccionada(usuario);
            }
            catch (Exception ex)
            {
                return 0;
                throw;
            }
        }
    }
}
