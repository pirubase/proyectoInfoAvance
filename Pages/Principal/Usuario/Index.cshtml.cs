using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using mecanico_plus.Data;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using Microsoft.AspNetCore.Authorization;

namespace mecanico_plus.Pages.Principal.Usuario
{
    [Authorize]
    public class IndexModel : PageModel
    { private readonly mecanico_plus.Data.local _context;
       
private readonly DbContextOptions<local> _contextOptions;

       public IndexModel(mecanico_plus.Data.local context, DbContextOptions<local> contextOptions)
       {
           _context = context;
           _contextOptions = contextOptions;
       }


        public IList<t001_usuario> t001_usuario { get;set; }
        public bool IsPatient { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
          
            try
            {
                if (HttpContext.Session.GetString("SessionUser") != null)
                {
                    IsPatient = HttpContext.Session.GetString("UserType") == "Cliente";

                    PermisoDomain permisos = new PermisoDomain();
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_USUARIOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_CONSULTAR))
                    {
                        // Obtén la empresa seleccionada
                        int currentEmpresaId = await ObtenerEmpresaSeleccionada();

                        if (IsPatient)
                        {
                            // Si es paciente, solo mostrar su propio registro
                            string userEmail = HttpContext.Session.GetString("SessionUser");
                            t001_usuario = await _context.t001_usuario
                                .Include(t => t.vObjEmpresa)
                                .Include(t => t.vObjEstado)
                                .Include(t => t.vObjPerfil)
                                .Include(t => t.vObjCliente)
                                .Where(t => t.f001_correo_electronico == userEmail)
                                .ToListAsync();
                        }
                        else
                        {
                            // Si no es paciente, mostrar todos los usuarios de la empresa
                            t001_usuario = await _context.t001_usuario
                                .Include(t => t.vObjEmpresa)
                                .Include(t => t.vObjEstado)
                                .Include(t => t.vObjPerfil)
                                .Include(t => t.vObjCliente)
                                .Where(t => t.f001_rowid_empresa_o_persona_natural == currentEmpresaId)
                                .ToListAsync();
                        }

                        return null;
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para consultar usuarios.";
                        return RedirectToPage("../../Principal/Base/Index");
                    }

                }
                else
                {
                    HttpContext.Session.SetString("ExpiredSession", "true");
                    return RedirectToPage("../../Login/Index");
                }
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("ExpiredSession", "true");
                return RedirectToPage("../../Login/Index");
            }
           
        }

        public async Task<int> ObtenerEmpresaSeleccionada()
        {
            try
            {
                // Verifica que el correo electrónico del usuario esté disponible en la sesión
                string sessionUser = HttpContext.Session.GetString("SessionUser");
                if (string.IsNullOrEmpty(sessionUser))
                {
                    throw new Exception("Usuario no encontrado en la sesión.");
                }

                // Obtén el rowId de la empresa asociada al usuario logueado
                int rowIdEmpresaSeleccionada = await (from use in _context.t001_usuario
                                                      where use.f001_correo_electronico == sessionUser
                                                      select use.f001_rowid_empresa_o_persona_natural).FirstAsync();

                return rowIdEmpresaSeleccionada;
            }
            catch (Exception ex)
            {
                // Manejo de excepciones (puedes personalizar el mensaje o hacer un registro de errores)
                throw new Exception("Error al obtener la empresa seleccionada.", ex);
            }
        }
        public IActionResult OnPostIrACrear()
        {
            try
            {
                return RedirectToPage("Create");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
