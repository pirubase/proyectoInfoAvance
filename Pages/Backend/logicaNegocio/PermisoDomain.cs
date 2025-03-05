using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.accesoDatos;
using mecanico_plus.Pages.Backend.constantes;

namespace mecanico_plus.Pages.Backend.logicaNegocio
{
    public class PermisoDomain
    {

        private const bool NO_TIENE_PERMISO = false;
        private const bool TIENE_PERMISO = true;

        public IList<t003_permisos> t003_permisos { get; set; }

        public PermisoDomain()
        {

        }

        public async Task<bool> usuarioTienePermisoMenu(string strMenu, string strUsuario, int intPermisoAccion)
        {
            bool blnUsuarioTienePermisos = NO_TIENE_PERMISO;
            try
            {
                PermisosDAO permisosDAO = new PermisosDAO();
                t003_permisos = await permisosDAO.usuarioTienePermisoMenu(strMenu, strUsuario);

                //se valida si el usuario tiene permisos para consultar, eliminar, editar, crear 
                //segun como se requiera
                switch (intPermisoAccion)
                {
                    case Costantes.PERMISO_CONSULTAR:
                        if (t003_permisos[0].f003_permiso_consultar == TIENE_PERMISO &&
                            t003_permisos[0].f003_permiso_uso_menu == TIENE_PERMISO)
                        {
                            blnUsuarioTienePermisos = TIENE_PERMISO;
                        }

                        break;
                    case Costantes.PERMISO_EDITAR:
                        if (t003_permisos[0].f003_permiso_editar == TIENE_PERMISO &&
                            t003_permisos[0].f003_permiso_uso_menu == TIENE_PERMISO)
                        {
                            blnUsuarioTienePermisos = TIENE_PERMISO;
                        }

                        break;
                    case Costantes.PERMISO_CREAR:
                        if (t003_permisos[0].f003_permiso_crear == TIENE_PERMISO &&
                            t003_permisos[0].f003_permiso_uso_menu == TIENE_PERMISO)
                        {
                            blnUsuarioTienePermisos = TIENE_PERMISO;
                        }

                        break;
                    case Costantes.PERMISO_ELIMINAR:
                        if (t003_permisos[0].f003_permiso_eliminar == TIENE_PERMISO &&
                            t003_permisos[0].f003_permiso_uso_menu == TIENE_PERMISO)
                        {
                            blnUsuarioTienePermisos = TIENE_PERMISO;
                        }

                        break;
                    case Costantes.PERMISO_DETALLE:
                        if (t003_permisos[0].f003_permiso_detalle == TIENE_PERMISO &&
                            t003_permisos[0].f003_permiso_uso_menu == TIENE_PERMISO)
                        {
                            blnUsuarioTienePermisos = TIENE_PERMISO;
                        }

                        break;
                    case Costantes.PERMISO_MENU:
                        if (t003_permisos[0].f003_permiso_uso_menu == TIENE_PERMISO)
                        {
                            blnUsuarioTienePermisos = TIENE_PERMISO;
                        }

                        break;
                    default:
                        blnUsuarioTienePermisos = NO_TIENE_PERMISO;
                        break;
                }

                return blnUsuarioTienePermisos;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

       
    }
}
