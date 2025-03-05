using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.Conexion;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace mecanico_plus.Pages.Backend.accesoDatos
{
    public class PermisosDAO : PageModel
    {
        private local _context;

        public IList<t001_usuario> t001_usuario { get; set; }

        public IList<t003_permisos> t003_permisos { get; set; }

        private ConexionDB conexion;

        public PermisosDAO()
        {
            conexion = new ConexionDB();

            //creacion del contex
            var optionsBuilder = new DbContextOptionsBuilder<local>();
            optionsBuilder.UseNpgsql(conexion.ObtieneCadenaConexion());
            _context = new local(optionsBuilder.Options);
        }

        public async Task<IList<t003_permisos>> usuarioTienePermisoAccionEnMenu(String strMenu, String strUsuario)
        {
            try
            {
                //Se obtiene la data t001 del usuario logeado
                //nota: retorna un solo registro debido a que el nombre de usuario 
                t001_usuario = await _context.t001_usuario
                   .Include(t => t.vObjPerfil)
                   .Where(t => t.f001_correo_electronico.Equals(strUsuario))
                   .ToListAsync();

                //se obtiene los permisos que tiene el usuario logeado
                t003_permisos = await _context.t003_permisos
                    .Include(t => t.vObjMenu)
                    .Where(t => t.f003_rowid_perfil == t001_usuario[0].f001_rowid_perfil)
                    .Where(t => t.vObjMenu.f005_nombre.Equals(strMenu))
                    .ToListAsync();

                return t003_permisos;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }

        public async Task<IList<t003_permisos>> usuarioTienePermisoMenu(String strMenu, String strUsuario)
        {
            try
            {
                //Se obtiene la data t001 del usuario logeado
                //nota: retorna un solo registro debido a que el nombre de usuario es unico
                t001_usuario = await _context.t001_usuario
                   .Include(t => t.vObjPerfil)
                        .Include(t => t.vObjEmpresa)
                             .Include(t => t.vObjEstado)
                   .Where(t => t.f001_correo_electronico.Equals(strUsuario))
                   .ToListAsync();

                //se obtiene los permisos que tiene el usuario logeado
                t003_permisos = await _context.t003_permisos
                    .Include(t => t.vObjMenu)
                   
                    .Where(t => t.f003_rowid_perfil == t001_usuario[0].f001_rowid_perfil)
                    .Where(t => t.vObjMenu.f005_nombre.Equals(strMenu))
                    .ToListAsync();

                return t003_permisos;
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }
    }
}
