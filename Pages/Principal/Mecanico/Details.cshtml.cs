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

namespace mecanico_plus.Pages.Principal.Mecanico
{
    public class DetailsModel : PageModel
    {
        private readonly mecanico_plus.Data.local _context;

        public DetailsModel(mecanico_plus.Data.local context)
        {
            _context = context;
        }

        public t006_mecanico t006_mecanico { get; set; } = default!;

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
                    if (await permisos.usuarioTienePermisoMenu(nombresMenus.PERMISO_MECANICOS,
                                                               HttpContext.Session.GetString(Costantes.SESION_USUARIO),
                                                               Costantes.PERMISO_DETALLE))
                    {
                        if (id == null)
                        {
                            return NotFound();
                        }

                        var t006_mecanico = await _context.t006_mecanico.FirstOrDefaultAsync(m => m.f006_rowid == id);
                        if (t006_mecanico == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            this.t006_mecanico = t006_mecanico;
                        }
                        return Page();
                       
                    }
                    else
                    {
                        // Mostrar mensaje de error
                        TempData["ErrorMessage"] = "No tienes permiso para ver detalles de mecanicos.";
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
    }
}
