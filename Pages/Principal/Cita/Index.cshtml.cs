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

namespace mecanico_plus.Pages.Principal.Cita
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly mecanico_plus.Data.local _context;

        
        public IList<t009_cita> t009_cita { get;set; }

       private readonly DbContextOptions<local> _contextOptions;

        public IndexModel(mecanico_plus.Data.local context, DbContextOptions<local> contextOptions)
        {
            _context = context;
            _contextOptions = contextOptions;
        }


  

        public async Task<IActionResult> OnGetAsync()
        {

            
            try
            {
                if (HttpContext.Session.GetString("SessionUser") != null)
                {

                    PermisoDomain permisos = new PermisoDomain();
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_CITAS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_CONSULTAR))
                    {

                          // Obtén la empresa seleccionada
                        int currentEmpresaId = await ObtenerEmpresaSeleccionada();
                        
                        t009_cita = await _context.t009_cita
                .Include(t => t.vObjMecanico)
                .Include(t => t.vObjEmpresa)
                .Include(t => t.vObjCliente)
                .Include(t => t.vObjEspecialidad)
                 .Include(t => t.vObjServicio)
                 .Where(t => t.f009_rowid_empresa_o_persona_natural == currentEmpresaId)
                .ToListAsync();

                        return null;
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para consultar citas.";
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

        public async Task<IActionResult> OnGetCitaDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.t009_cita
                .Include(c => c.vObjMecanico)
                .Include(c => c.vObjCliente)
                .Include(c => c.vObjEspecialidad)
                 .Include(t => t.vObjServicio)
                .FirstOrDefaultAsync(m => m.f009_rowid == id);

            if (cita == null)
            {
                return NotFound();
            }

            return new JsonResult(cita);
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
