using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using mecanico_plus.Data;
using Microsoft.EntityFrameworkCore;
using mecanico_plus.Pages.Backend.constantes;
using mecanico_plus.Pages.Backend.logicaNegocio;
using mecanico_plus.Pages.Backend.menus;
using Microsoft.AspNetCore.Authorization;

namespace mecanico_plus.Pages.Principal.Especialidad
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly mecanico_plus.Data.local _context;

        public CreateModel(mecanico_plus.Data.local context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                if (HttpContext.Session.GetString("SessionUser") != null)
                {

                    PermisoDomain permisos = new PermisoDomain();
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_VEHICULOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_CREAR))
                    {
                        // Obtener el ID de la empresa asociada al usuario logueado
                        string sessionUser = HttpContext.Session.GetString("SessionUser");
                        if (string.IsNullOrEmpty(sessionUser))
                        {
                            throw new Exception("Usuario no encontrado en la sesión.");
                        }

                        int empresaId = await (from use in _context.t001_usuario
                                               where use.f001_correo_electronico == sessionUser
                                               select use.f001_rowid_empresa_o_persona_natural).FirstAsync();

                        ViewData["f010_rowid_empresa_o_persona_natural"] = new SelectList(_context.t002_empresa_o_persona_natural.Where(e => e.f002_rowid == empresaId), "f002_rowid", "f002_razon_social");
                        ViewData["f010_rowid_cliente"] = new SelectList(_context.t007_cliente, "f007_rowid", "f007_nombre");
                        return Page();
                    }
                    else
                    {

                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para crear especialidades.";
                        return RedirectToPage("./Index");
                    }

                }
                else
                {
                    HttpContext.Session.SetString("ExpiredSession", "true");
                    return RedirectToPage("../../Login/Index");
                };
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "No tienes permiso para crear permisos.";
                return RedirectToPage("../../Login/Index");
            }

          
        }

        [BindProperty]
        public t010_vehiculo t010_vehiculo { get; set; }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            t010_vehiculo.f010_ts = DateTime.Now;
            _context.t010_vehiculo.Add(t010_vehiculo);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
