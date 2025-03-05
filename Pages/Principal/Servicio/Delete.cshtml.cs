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

namespace mecanico_plus.Pages.Principal.Servicio
{
    public class DeleteModel : PageModel
    {
        private readonly mecanico_plus.Data.local _context;

        public DeleteModel(mecanico_plus.Data.local context)
        {
            _context = context;
        }

        [BindProperty]
        public t014_servicio t014_servicio { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            try
            {
                if (HttpContext.Session.GetString("SessionUser") != null)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    PermisoDomain permisos = new PermisoDomain();
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_SERVICIOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_ELIMINAR))
                    {
                        if (id == null)
                        {
                            return NotFound();
                        }

                        var t014_servicio = await _context.t014_servicio.FirstOrDefaultAsync(m => m.f014_rowid == id);

                        if (t014_servicio == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            t014_servicio = t014_servicio;
                        }
                        return Page();
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para eliminar mecanicos.";
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
                HttpContext.Session.SetString("ExpiredSession", "true");
                return RedirectToPage("../../Login/Index");
            }
           
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var t014_servicio = await _context.t014_servicio.FindAsync(id);
            if (t014_servicio != null)
            {
                t014_servicio = t014_servicio;
                _context.t014_servicio.Remove(t014_servicio);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
